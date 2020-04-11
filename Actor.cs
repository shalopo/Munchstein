using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    enum ActorOrientation
    {
        TALL,
        FLAT,
    }

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
        public bool CanChangeOrientation { get; set; } = false;

        private int _lastWalkSign = 0;
        private int _continuousWalkTime = 0;

        private readonly ILevel _level;
        public event Action OnDeath;
        public event Action OnJump;
        public event Action OnDrop;
        public event Action<Munch> OnMunch;

        public Platform CurrentPlatform { get; private set; }
        public Platform LastPlatform { get; private set; }

        internal int Size { get; set; } = 1;
        internal ActorOrientation Orientation { get; set; } = ActorOrientation.TALL;
        public double Height => Orientation == ActorOrientation.TALL ? Size : Size / 2.0;
        public double Width => Orientation == ActorOrientation.TALL ? Size / 2.0 : Size;

        private double SizeFactor => 0.4 + 0.6 * Size;
        double JumpSpeed => (Orientation == ActorOrientation.TALL ? BASE_JUMP_SPEED : 1.6 * BASE_MAX_GROUND_SPEED) * SizeFactor;
        double MaxGroundSpeed => (Orientation == ActorOrientation.TALL ? BASE_MAX_GROUND_SPEED : BASE_JUMP_SPEED) * SizeFactor;
        double GroundAcceleration => SizeFactor * BASE_GROUND_ACCELERATION;
        public Box2 Box => new Box2(new Point2(Location.X - Width / 2, Location.Y + Height), Width, Height);

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
            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                ApplyAcceleration(GRAVITY, dt);
                ApplyDrag(dt);

                Location += dt * Velocity.YProjection;
            }
            else
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

            Location += dt * Velocity.XProjection;

            ApplyCollisions(dt);

            var munch = _level.TryEatMunch(Box);
            if (munch != null)
            {
                Size *= 2;
                OnMunch?.Invoke(munch);
            }

            if (Location.Y <= 0)
            {
                OnDeath?.Invoke();
                _level.NotifyActorDead();
            }
        }

        private void ApplyCollisions(double dt)
        {
            if (Velocity.IsZero)
            {
                return;
            }

            Collision collision;

            while (!(collision = _level.GetCollision(Box, dt * Velocity)).IsNone)
            {
                _continuousWalkTime = 0;

                Location -= collision.Vector;

                if (CanChangeOrientation && collision.LineLength != double.NaN && 
                    collision.LineLength <= Platform.MIN_WIDTH_TO_STAND_ON + 0.001)
                {
                    switch (Orientation)
                    {
                        case ActorOrientation.TALL:
                            if (collision.Vector.X != 0 && Math.Abs(Velocity.X) >= MaxGroundSpeed)
                            {
                                Orientation = ActorOrientation.FLAT;
                            }
                            break;
                        case ActorOrientation.FLAT:
                            if (collision.Vector.Y != 0 && Math.Abs(Velocity.Y) >= MaxGroundSpeed * 0.5)
                            {
                                Orientation = ActorOrientation.TALL;
                            }
                            break;
                    }
                }


                const double COLLISION_VELOCITY_LOSS_FACTOR = 0.75;

                if (collision.Vector.X != 0)
                {
                    Velocity = Velocity.YProjection - Velocity.XProjection * (1 - COLLISION_VELOCITY_LOSS_FACTOR);
                }

                if (collision.Vector.Y != 0)
                {
                    Velocity = Velocity.XProjection - Velocity.YProjection * (1 - COLLISION_VELOCITY_LOSS_FACTOR);
                }
            }
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

            if (CurrentPlatform.Type == PlatformType.PASSTHROUGH)
            {
                Velocity = new Vector2(Velocity.X, -1);
                Location += new Vector2(0, - Platform.STAND_DETECTION_THRESHOLD - 0.01);
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
            if (door != null && Size == door.Size)
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
