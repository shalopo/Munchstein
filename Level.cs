using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class Level : ILevel
    {
        public Level(ILevelContext levelControl, List<Platform> platforms)
        {
            _levelControl = levelControl;
            _platforms = platforms;
            Door = platforms.Count == 0 ? null : new Door(platforms.Last().Box.TopRight - Vector2.X_UNIT / 2);
            Actor = new Actor(this, platforms.Count == 0 ? new Point2(0, 0) : platforms[0].Box.TopLeft + Vector2.X_UNIT / 2);
        }

        public Actor Actor { get; private set; }

        readonly ILevelContext _levelControl;
        readonly List<Platform> _platforms = new List<Platform>();
        public Door Door { get; private set; }
        public Munch Munch { get; set; }

        public IReadOnlyCollection<Platform> Platforms => _platforms;

        Vector2 ILevel.GetCollisionBox(Box2 box, Vector2 disposition)
        {
            foreach (Platform platform in _platforms)
            {
                if (platform.Type == PlatformType.CONCRENT)
                {
                    var collision_box = box.CalcualteCollisionBox(disposition, platform.Box);

                    //TODO: We assume there can be only one collision which is not always the case
                    if (!collision_box.IsZero)
                    {
                        return collision_box;
                    }
                }
            }

            return Vector2.ZERO;
        }

        Platform ILevel.GetSupportingPlatform(Box2 box)
        {
            foreach (Platform platform in _platforms)
            {
                if (box.Right > platform.Box.Left && box.Left < platform.Box.Right)
                {
                    if (Math.Abs(box.Bottom - platform.Box.Top) <= Platform.STANDING_THRESHOLD)
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

        void ILevel.NotifyActorDead()
        {
            _levelControl.RestartLevel();
        }

        Munch ILevel.TryEatMunch(Box2 box)
        {
            if (Munch != null && Box2.Overlap(Munch.Box, box))
            {
                var munch = Munch;
                Munch = null;
                return munch;
            }

            return null;
        }

        public void Step(double dt)
        {
            Actor.Step(dt);
        }
    }
}
