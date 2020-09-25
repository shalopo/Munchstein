using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Munchstein
{
    public class Level : ILevel
    {
        public Level(ILevelContext levelControl, List<Platform> platforms)
        {
            _levelControl = levelControl;
            _platforms = platforms;
            Door = platforms.Count == 0 ? null : new Door(platforms.Last().Box.TopRight - Vector2.X_UNIT / 2);
            Actors.Add(new Actor(this, platforms.Count == 0 ? new Point2(0, 0) : platforms[0].Box.TopLeft + Vector2.X_UNIT / 2));
        }

        readonly ILevelContext _levelControl;
        readonly List<Platform> _platforms = new List<Platform>();
        public Door Door { get; private set; }

        public List<Actor> Actors = new List<Actor>();
        public List<Munch> Munches = new List<Munch>();

        public bool CanActorJump { get; set; } = true;
        public bool CanActorChangeOrientation { get; set; } = false;

        private LevelState _checkpoint;

        public event Action<Actor> OnActorDeath;
        public event Action<Actor> OnActorJump;
        public event Action<Actor> OnActorDrop;
        public event Action<Actor, Munch> OnActorMunch;

        struct LevelState
        {
            public struct ActorState
            {
                public Point2 Location { get; set; }
                public int Size { get; set; }
                public ActorOrientation Orientation { get; set; }
                public Vector2 Velocity { get; set; }
            }

            public List<ActorState> Actors { get; set; }
            public List<Munch> Munches { get; set; }
        }

        public IReadOnlyCollection<Platform> Platforms => _platforms;

        public void Init()
        {
            SaveCheckpoint();
        }

        Platform ILevel.GetCollidingPlatform(Box2 box)
        {
            foreach (Platform platform in _platforms)
            {
                if (platform.IsCollidable)
                {
                    if (Box2.Overlap(box, platform.Box))
                    {
                        return platform;
                    }
                }
            }

            return null;
        }

        Platform ILevel.GetSupportingPlatform(Box2 box)
        {
            const double COLLISION_THRESHOLD = Box2.COLLISION_THRESHOLD;

            foreach (Platform platform in _platforms)
            {
                if (platform.Box.Width >= Platform.MIN_WIDTH_TO_STAND_ON - 0.001 && 
                    box.Right - platform.Box.Left >= COLLISION_THRESHOLD && 
                    platform.Box.Right - box.Left >= COLLISION_THRESHOLD)
                {
                    if (Math.Abs(box.Bottom - platform.Box.Top) <= Platform.STAND_DETECTION_THRESHOLD)
                    {
                        return platform;
                    }
                }
            }

            return null;
        }

        Door ILevel.GetAdjacentDoor(Box2 box)
        {
            if (Door != null && Box2.Overlap(Door.Box, box))
            {
                return Door;
            }

            return null;
        }

        void ILevel.NotifyDoorOpened(Door door)
        {
            _levelControl.NotifyLevelComplete();
        }

        void ILevel.NotifyActorDeath(Actor actor)
        {
            LoadLastCheckpoint();
            OnActorDeath?.Invoke(actor);
        }

        void ILevel.NotifyActorMunch(Actor actor, Munch munch) => OnActorMunch?.Invoke(actor, munch);
        void ILevel.NotifyActorJump(Actor actor) => OnActorJump?.Invoke(actor);
        void ILevel.NotifyActorDrop(Actor actor) => OnActorDrop?.Invoke(actor);

        Munch LocateOverlappingMunch(Box2 box) => Munches.FirstOrDefault(munch => Box2.Overlap(munch.Box, box));

        Munch ILevel.TryEatMunch(Actor actor)
        {
            var munch = LocateOverlappingMunch(actor.Box);

            if (munch == null)
            {
                return null;
            }

            Munches.Remove(munch);
            return munch;
        }

        public void Step(double dt)
        {
            foreach (var actor in Actors)
            {
                actor.Step(dt);
            }
        }

        public void SaveCheckpoint()
        {
            _checkpoint = new LevelState
            {
                Actors = Actors.Select(actor => new LevelState.ActorState
                {
                    Location = actor.Location,
                    Orientation = actor.Orientation,
                    Size = actor.Size,
                    Velocity = actor.Velocity,
                }).ToList(),
                Munches = new List<Munch>(Munches),
            };
        }

        private void LoadLastCheckpoint()
        {
            Actors = _checkpoint.Actors.Select(actorState => new Actor(this, actorState.Location)
            {
                Orientation = actorState.Orientation,
                Size = actorState.Size,
                Velocity = actorState.Velocity,
            }).ToList();

            Munches = new List<Munch>(_checkpoint.Munches);
        }

    }
}
