using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public enum PlatformType
    {
        CONCRETE,
        CONCRETE_POINT,
        PASSTHROUGH,
        ONEWAY,
    }

    public enum InteractionType
    {
        LAND,
        STAND,
        COLLIDE,
        CHANGE_ORIENTATION,
    }

    public class Platform
    {
        public static readonly double MIN_WIDTH_TO_STAND_ON = 0.2;
        public static readonly double STAND_DETECTION_THRESHOLD = 0.09;

        private Platform(Box2 box) => Box = box;

        public static Platform Concrete(Point2 topLeft, double width, double height = 0.3)
        {
            return new Platform(new Box2(topLeft: topLeft, width: width, height: height)) { Type = PlatformType.CONCRETE };
        }

        public static Platform ConcretePoint(Point2 bottomLeft)
        {
            return new Platform(new Box2(topLeft: bottomLeft + new Vector2(0, 0.1), width: 0.1, height: 0.1))
            {
                Type = PlatformType.CONCRETE_POINT
            };
        }

        public static Platform PassThrough(Point2 topLeft, double width)
        {
            return new Platform(new Box2(topLeft: topLeft, width: width, height: 0)) { Type = PlatformType.PASSTHROUGH };
        }

        public static Platform OneWay(Point2 topLeft, double width)
        {
            return new Platform(new Box2(topLeft: topLeft, width: width, height: 0)) { Type = PlatformType.ONEWAY };
        }

        public Box2 Box { get; private set; }
        public PlatformType Type { get; private set; }
        public bool IsCollidable => Type == PlatformType.CONCRETE || Type == PlatformType.CONCRETE_POINT;

        private event Action<Actor, InteractionType> OnInteract;

        public event Action<Actor> OnActorLanding
        {
            add => OnInteract += (actor, type) =>
            {
                if (type == InteractionType.LAND)
                {
                    value(actor);
                }
            };
            remove => throw new Exception("Not implemented");
        }

        public event Action<Actor> OnActorStanding
        {
            add => OnInteract += (actor, type) =>
            {
                if (type == InteractionType.STAND)
                {
                    value(actor);
                }
            };
            remove => throw new Exception("Not implemented");
        }

        public event Action<Actor> OnActorColliding
        {
            add => OnInteract += (actor, type) =>
            {
                if (type == InteractionType.COLLIDE)
                {
                    value(actor);
                }
            };
            remove => throw new Exception("Not implemented");
        }

        public event Action<Actor> OnActorChangedOrientation
        {
            add => OnInteract += (actor, type) =>
            {
                if (type == InteractionType.CHANGE_ORIENTATION)
                {
                    value(actor);
                }
            };
            remove => throw new Exception("Not implemented");
        }

        public void NotifyActorInteracting(Actor actor, InteractionType type)
        {
            OnInteract?.Invoke(actor, type);
        }
    }
}
