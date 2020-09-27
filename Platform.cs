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
        FLIPPER,
        PASSTHROUGH,
        ONEWAY,
        SPLITTER,
    }

    public enum InteractionType
    {
        LAND,
        STAND,
        COLLIDE,
        CHANGE_ORIENTATION,
        SPLIT,
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

        public static Platform Flipper(Point2 bottomLeft)
        {
            return new Platform(new Box2(topLeft: bottomLeft + new Vector2(0, 0.1), width: 0.1, height: 0.1))
            {
                Type = PlatformType.FLIPPER
            };
        }

        public static Platform Splitter(Point2 topLeft)
        {
            return new Platform(new Box2(topLeft: topLeft, width: 0.04, height: 1))
            {
                Type = PlatformType.SPLITTER
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
        public bool IsCollidable => Type == PlatformType.CONCRETE || Type == PlatformType.FLIPPER || Type == PlatformType.SPLITTER;

        public event Action<Actor> OnActorLanding;
        public event Action<Actor> OnActorStanding;
        public event Action<Actor> OnActorColliding;
        public event Action<Actor> OnActorChangedOrientation;
        public event Action<Actor> OnActorSplit;

        public void NotifyInteraction(Actor actor, InteractionType type)
        {
            var actorEvent = type switch
            {
                InteractionType.LAND => OnActorLanding,
                InteractionType.STAND => OnActorStanding,
                InteractionType.COLLIDE => OnActorColliding,
                InteractionType.CHANGE_ORIENTATION => OnActorChangedOrientation,
                InteractionType.SPLIT => OnActorSplit,
                _ => throw new ArgumentException(nameof(type))
            };

            actorEvent?.Invoke(actor);
        }
    }
}
