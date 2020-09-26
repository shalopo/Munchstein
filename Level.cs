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
            //todo: this should be based on velocity vector to make sure we are not missing a platform if moving too fast

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

        Munch LocateOverlappingMunch(Actor actor) => Munches.FirstOrDefault(munch => Box2.Overlap(munch.Box, actor.Box));

        Munch ILevel.TryEatMunch(Actor actor)
        {
            var munch = LocateOverlappingMunch(actor);

            if (munch == null)
            {
                return null;
            }

            Munches.Remove(munch);
            return munch;
        }

        public void Step(double dt)
        {
            List<Actor> actorsToSplit = new List<Actor>();

            CalculateActorsCollisions(dt);

            foreach (var actor in Actors)
            {
                actor.Step(dt);

                if (actor.SplitResult != null)
                {
                    actorsToSplit.Add(actor);
                }
            }

            foreach (var actor in actorsToSplit)
            {
                Actors.AddRange(actor.SplitResult);
                Actors.Remove(actor);
            }
        }

        private void CalculateActorsCollisions(double dt)
        {
            if (Actors.Count == 1)
            {
                return;
            }

            Dictionary<Actor, List<Actor>> actorsToUnite = new Dictionary<Actor, List<Actor>>();

            foreach (var actor1 in Actors)
            {
                foreach (var actor2 in Actors)
                {
                    if (actor1.Id < actor2.Id && Box2.Overlap(actor1.Box, actor2.Box))
                    {
                        var collision = actor1.Box.CalcualteCollision(dt * (actor1.Velocity - actor2.Velocity), actor2.Box);

                        if (collision.IsNone)
                        {
                            continue;
                        }

                        bool isXCollision = collision.Vector.X != 0;

                        if (actor1.Size == actor2.Size && actor1.Orientation == actor2.Orientation)
                        {
                            var size = actor1.Size;
                            var orientation = actor1.Orientation;
                            var midpoint = actor1.Location + (actor2.Location - actor1.Location) / 2;

                            if (isXCollision && orientation != ActorOrientation.FLAT)
                            {
                                actorsToUnite.Add(new Actor(this, midpoint){
                                    Orientation = orientation == ActorOrientation.TALL ? ActorOrientation.SQUARE : ActorOrientation.FLAT,
                                    Size = orientation == ActorOrientation.TALL ? size * 2 : size,
                                },
                                new List<Actor> { actor1, actor2 });

                                continue;
                            }
                            else if (!isXCollision && orientation != ActorOrientation.TALL)
                            {
                                actorsToUnite.Add(new Actor(this, midpoint)
                                {
                                    Orientation = orientation == ActorOrientation.FLAT ? ActorOrientation.SQUARE : ActorOrientation.TALL,
                                    Size = orientation == ActorOrientation.FLAT ? size * 2 : size,
                                },
                                new List<Actor> { actor1, actor2 });

                                continue;
                            }
                        }

                        var unit = isXCollision ? Vector2.X_UNIT : Vector2.Y_UNIT;

                        var m1 = actor1.Mass;
                        var m2 = actor2.Mass;
                        var v1 = actor1.Velocity * unit;
                        var v2 = actor2.Velocity * unit;
                        
                        var u2 = (2 * m1 * v1 + v2 * (m2 - m1)) / (m1 + m2);
                        var u1 = u2 + v2 - v1;

                        actor1.Velocity += unit * (u1 - v1);
                        actor2.Velocity += unit * (u2 - v2);

                        //todo: this is definitely wrong
                        actor1.Location -= collision.Vector / 2;
                        actor2.Location += collision.Vector / 2;
                    }
                }
            }

            foreach (var item in actorsToUnite)
            {
                Actors.Add(item.Key);

                foreach (var oldActor in item.Value)
                {
                    Actors.Remove(oldActor);
                }
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
