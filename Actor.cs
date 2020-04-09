using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{

    public class Actor
    {
        static readonly Vector2 GRAVITY = new Vector2(0, -9.8);
        static readonly double BASE_MAX_GROUND_SPEED = 3;
        static readonly double MIN_GROUND_SPEED = 1.5;
        static readonly double DRAG_CONSTANT = 0.012;
        static readonly double BASE_JUMP_SPEED = 7;
        static readonly double BASE_GROUND_ACCELERATION = 0.02;
        
        public Actor(ILevel level, Point2 location) => 
            (Location, Velocity, _level) = (location, new Vector2(0, 0), level);
        
        public Point2 Location { get; internal set; }
        public Vector2 Velocity { get; internal set; }
        public bool CanJump { get; set; } = true;

        private int _lastWalkSign = 0;
        private int _continuousWalkTime = 0;

        private readonly ILevel _level;
        public event Action OnDeath;
        public event Action OnJump;
        public event Action OnDrop;

        public Platform CurrentPlatform { get; private set; }
        public Platform LastPlatform { get; private set; }

        private double Size { get; set; } = 1;
        public double Height => Size;
        public double Width => Size / 2;

        private double SizeFactor => 0.4 + 0.6 * Size;
        double JumpSpeed => SizeFactor * BASE_JUMP_SPEED;
        double MaxGroundSpeed => SizeFactor * BASE_MAX_GROUND_SPEED;
        double GroundAcceleration => SizeFactor * BASE_GROUND_ACCELERATION;
        public BoxBoundary Box => new BoxBoundary(new Point2(Location.X - Width / 2, Location.Y + Height), Width, Height);

        public void ApplyAcceleration(Vector2 acceleration, double dt)
        {
            Velocity += acceleration * dt;
        }

        private void ApplyDrag(double dt)
        {
            ApplyAcceleration(-DRAG_CONSTANT * Velocity * Velocity.LengthSquared, dt);
        }

        private void UpdateSupportingPlatform()
        {
            Platform newPlatform;

            if (Velocity.Y > 0)
            {
                newPlatform = null;
            }
            else
            {
                newPlatform = _level.GetSupportingPlatform(Box);

                if (newPlatform != null)
                {
                    Velocity = Velocity.XProjection;
                    newPlatform.NotifyActorStanding(this);
                }
            }

            if (CurrentPlatform != newPlatform)
            {
                LastPlatform = CurrentPlatform;
            }

            CurrentPlatform = newPlatform;
        }

        public void Step(double dt)
        {
            bool collisions = ApplyCollisions(dt);

            if (!collisions)
            {
                UpdateSupportingPlatform();

                if (CurrentPlatform != null)
                {
                    if (Velocity.Y == 0)
                    {
                        Location = new Point2(Location.X, CurrentPlatform.Box.Top);
                    }
                    else
                    {
                        Location += dt * Velocity.YProjection;
                    }
                }
                else
                {
                    ApplyAcceleration(GRAVITY, dt);
                    ApplyDrag(dt);

                    Location += dt * Velocity.YProjection;
                }

                Location += dt * Velocity.XProjection;
            }

            if (_level.TryEatMunch(Box) != null)
            {
                Size *= 2;
            }

            if (Location.Y <= 0)
            {
                OnDeath?.Invoke();
                _level.NotifyActorDead();
            }
        }

        private bool ApplyCollisions(double dt)
        {
            var disposition = dt * Velocity;

            if (Velocity.IsZero)
            {
                return false;
            }

            var collision_box = _level.GetCollisionBox(Box, disposition);
            if (collision_box.IsZero)
            {
                return false;
            }

            Location += disposition - collision_box;

            if (collision_box.X != 0)
            {
                Velocity = Velocity.YProjection;
            }
            else
            {
                Velocity = Velocity.XProjection;
            }

            return true;
        }

        public void MoveSideways(int sign)
        {
            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            if (sign == 0 || _lastWalkSign != sign)
            {
                _continuousWalkTime = 0;
            }
            else
            {
                _continuousWalkTime++;
            }

            _lastWalkSign = sign;

            Velocity = new Vector2(sign * Math.Min(MIN_GROUND_SPEED + (GroundAcceleration * _continuousWalkTime), MaxGroundSpeed), 0);
        }

        public void Drop()
        {
            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            OnDrop?.Invoke();

            if (CurrentPlatform.IsPassThrough)
            {
                Velocity = new Vector2(Velocity.X, -1);
                Location += new Vector2(0, - Platform.STANDING_THRESHOLD - 0.01);
            }

            LastPlatform = CurrentPlatform;
            CurrentPlatform = null;
        }

        public void Stop()
        {
            MoveSideways(0);
        }

        public void Action()
        {
            if (CurrentPlatform == null)
            {
                return;
            }

            var door = _level.GetAdjacentDoor(Box);
            if (door != null)
            {
                _level.NotifyDoorOpened(door);
            }
        }

        public void Jump()
        {
            if (!CanJump)
            {
                return;
            }

            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            OnJump?.Invoke();

            Velocity += new Vector2(0, JumpSpeed);

            LastPlatform = CurrentPlatform;
            CurrentPlatform = null;
        }

    }
}
