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
            Door = platforms.Count == 0 ? null : new Door(platforms.Last().Box.TopRight - Vector2.NUDGE_X);
            Actor = new Actor(this, platforms.Count == 0 ? new Point2(0, 0) : platforms[0].Box.TopLeft + Vector2.NUDGE_X);
        }

        public Actor Actor { get; private set; }

        readonly ILevelContext _levelControl;
        readonly List<Platform> _platforms = new List<Platform>();
        public Door Door { get; private set; }

        public IReadOnlyCollection<Platform> Platforms => _platforms;

        public Vector2 GetCollisionBox(BoxBoundary box, Vector2 disposition)
        {
            foreach (Platform platform in _platforms)
            {
                if (!platform.IsPassThrough)
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

        public Platform GetSupportingPlatform(BoxBoundary box)
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

        public Door GetAdjacentDoor(Point2 point)
        {
            if (Door != null && Door.Box.Contains(point))
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

        public void Step(double dt)
        {
            Actor.Step(dt);
        }
    }
}
