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
        PASSTHROUGH,
        ONEWAY,
    }

    public class Platform
    {
        public static readonly double MIN_WIDTH_TO_STAND_ON = 0.2;
        public static readonly double STAND_DETECTION_THRESHOLD = 0.09;

        private Platform(Box2 box) => Box = box;

        public static Platform Concrete(Point2 topLeft, double width, double height = 0.3)
        {
            return new Platform(new Box2(topLeft: topLeft, width: width, height: height));
        }

        public static Platform ConcretePoint(Point2 bottomLeft)
        {
            return new Platform(new Box2(topLeft: bottomLeft + new Vector2(0, 0.1), width: 0.1, height: 0.1));
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

        public event Action<Actor> OnActorStanding;

        public void NotifyActorStanding(Actor actor)
        {
            OnActorStanding?.Invoke(actor);
        }
    }
}
