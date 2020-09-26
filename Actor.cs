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
        SQUARE,
    }

    public class Actor
    {
        static readonly Vector2 GRAVITY = new Vector2(0, -9.8);
        static readonly double BASE_MAX_GROUND_SPEED = 3;
        static readonly double MIN_GROUND_SPEED = 1.5;
        static readonly double DRAG_CONSTANT = 0.012;
        static readonly double BASE_JUMP_SPEED = 7;
        static readonly double BASE_GROUND_ACCELERATION = 0.02;

        static int NextId = 1;
        
        public Actor(ILevel level, Point2 location) => 
            (Location, Velocity, _level, Id) = (location, new Vector2(0, 0), level, NextId++);
        
        private readonly ILevel _level;
        public int Id { get; private set; }
        public Point2 Location { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        private int _lastWalkSign = 0;
        private int _continuousWalkTime = 0;
        private bool _preparingForJump = false;
        private bool _isJumping = false;
        public List<Actor> SplitResult { get; private set; }

        public Platform CurrentPlatform { get; private set; }
        public Platform LastPlatform { get; private set; }

        internal int Size { get; set; } = 1;
        internal ActorOrientation Orientation { get; set; } = ActorOrientation.TALL;
        public double Height => Orientation == ActorOrientation.TALL ? Size : Size / 2.0;
        public double Width => Orientation == ActorOrientation.FLAT ? Size : Size / 2.0;
        public double Mass => Height * Width;

        private double SizeFactor => 0.4 + 0.6 * Size;
        double JumpSpeed => (Orientation == ActorOrientation.TALL ? BASE_JUMP_SPEED : 1.6 * BASE_MAX_GROUND_SPEED) * SizeFactor;
        double MaxGroundSpeed => (Orientation == ActorOrientation.FLAT ? BASE_JUMP_SPEED : BASE_MAX_GROUND_SPEED) * SizeFactor;
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

                if (newPlatform == null)
                {
                    ReleaseJumpIfPreparing();
                }
                else
                {
                    Velocity = Velocity.XProjection;

                    if (newPlatform != CurrentPlatform)
                    {
                        newPlatform.NotifyActorInteracting(this, InteractionType.LAND);
                    }
                    else
                    {
                        newPlatform.NotifyActorInteracting(this, InteractionType.STAND);
                    }

                    if (Velocity.X == 0)
                    {
                        ReleaseJumpIfPreparing();
                    }
                }
            }

            if (CurrentPlatform != newPlatform)
            {
                if (newPlatform != null)
                {
                    _isJumping = false;
                }

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

            var munch = _level.TryEatMunch(this);
            if (munch != null)
            {
                Size *= 2;
                _level.NotifyActorMunch(this, munch);
            }

            if (Location.Y <= 0)
            {
                _level.NotifyActorDeath(this);
            }
        }

        private void ApplyCollisions(double dt)
        {
            if (Velocity.IsZero)
            {
                return;
            }

            Platform collidingPlatform;

            while ((collidingPlatform = _level.GetCollidingPlatform(Box)) != null)
            {
                var collision = Box.CalcualteCollision(dt * Velocity, collidingPlatform.Box);

                if (collision.IsNone)
                {
                    break;
                }

                _continuousWalkTime = 0;
                bool changedOrientation = false;

                if (collidingPlatform.Type == PlatformType.SPLITTER && collision.Vector.Y < 0 && Orientation != ActorOrientation.TALL)
                {
                    SplitResult = new int[] { 1, -1 }.Select(sign => new Actor(_level,
                        collidingPlatform.Box.TopLeft + new Vector2(sign * Width, 0))
                    {
                        Velocity = new Vector2(sign * 1, 0),
                        Orientation = Orientation == ActorOrientation.FLAT ? ActorOrientation.SQUARE : ActorOrientation.TALL,
                        Size = Orientation == ActorOrientation.FLAT ? Size : Size / 2,
                    }).ToList();
                }

                if (collidingPlatform.Type == PlatformType.FLIPPER)
                {
                    switch (Orientation)
                    {
                        case ActorOrientation.TALL:
                            if (collision.Vector.X != 0 && Math.Abs(Velocity.X) >= MaxGroundSpeed)
                            {
                                Orientation = ActorOrientation.FLAT;
                                Location -= Vector2.X_UNIT * ((Width - Height) / 2 * Math.Sign(Velocity.X) + collidingPlatform.Box.Width);
                                changedOrientation = true;
                            }
                            break;
                        case ActorOrientation.FLAT:
                            if (collision.Vector.Y != 0 && Velocity.Y <= -MaxGroundSpeed * 0.6)
                            {
                                Orientation = ActorOrientation.TALL;
                                changedOrientation = true;
                            }
                            break;
                    }
                }

                if (changedOrientation)
                {
                    collidingPlatform.NotifyActorInteracting(this, InteractionType.CHANGE_ORIENTATION);
                }
                else
                {
                    collidingPlatform.NotifyActorInteracting(this, InteractionType.COLLIDE);

                    Location -= collision.Vector;

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

            Velocity = new Vector2(sign * Math.Min(MIN_GROUND_SPEED + (GroundAcceleration * _continuousWalkTime), MaxGroundSpeed), 
                Velocity.Y);
        }

        public void Drop()
        {
            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            _level.NotifyActorDrop(this);

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
            if (door != null && Width == door.Width && Height == door.Height)
            {
                _level.NotifyDoorOpened(door);
            }
        }

        public void PrepareForJump()
        {
            if (!_level.CanActorJump)
            {
                _level.NotifyActorJump(this);
                return;
            }

            UpdateSupportingPlatform();

            if (CurrentPlatform == null)
            {
                return;
            }

            if (!_isJumping)
            {
                _preparingForJump = true;
            }
        }

        public void ReleaseJumpIfPreparing()
        {
            if (!_preparingForJump || _isJumping)
            {
                return;
            }

            _isJumping = true;
            _preparingForJump = false;

            _level.NotifyActorJump(this);

            Velocity += new Vector2(0, JumpSpeed);

            LastPlatform = CurrentPlatform;
        }

    }
}
