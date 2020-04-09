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
        static readonly double MAX_GROUND_SPEED = 3;
        static readonly double MIN_GROUND_SPEED = 1.5;

        public Actor(ILevel level, Point2 location) => 
            (Location, Velocity, _level) = (location, new Vector2(0, 0), level);
        
        public Point2 Location { get; internal set; }
        public Vector2 Velocity { get; internal set; }
        private int _lastWalkSign = 0;
        private int _continuousWalkTime = 0;

        private readonly ILevel _level;
        public event Action OnDeath;
        public event Action OnJump;
        public event Action OnDrop;

        public Platform CurrentPlatform { get; private set; }
        public Platform LastPlatform { get; private set; }

        public double Height => 1;
        public double Width => Height / 2;
        public BoxBoundary Box => new BoxBoundary(new Point2(Location.X - Width / 2, Location.Y + Height), Width, Height);

        public void ApplyAcceleration(Vector2 acceleration, double dt)
        {
            Velocity += acceleration * dt;
        }

        private void ApplyDrag(double dt)
        {
            ApplyAcceleration(-0.012 * Velocity * Velocity.LengthSquared, dt);
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

            Velocity = new Vector2(sign * Math.Min(MIN_GROUND_SPEED + (0.02 * _continuousWalkTime), MAX_GROUND_SPEED), 0);
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

            var door = _level.GetAdjacentDoor(Location);
            if (door != null)
            {
                _level.NotifyDoorOpened(door);
            }
        }

        public void Jump()
        {
            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            OnJump?.Invoke();

            Velocity += new Vector2(0, 7);

            LastPlatform = CurrentPlatform;
            CurrentPlatform = null;
        }

    }
}
