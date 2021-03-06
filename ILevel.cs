﻿namespace Munchstein
{
    public interface ILevel
    {
        bool CanActorJump { get; }

        Platform GetSupportingPlatform(Box2 box);
        Platform GetCollidingPlatform(Box2 box);
        Door GetAdjacentDoor(Box2 box);
        Munch TryEatMunch(Actor actor);

        void NotifyDoorOpened(Door door);
        void NotifyActorDeath(Actor actor);
        void NotifyActorMunch(Actor actor, Munch munch);
        void NotifyActorJump(Actor actor);
        void NotifyActorDrop(Actor actor);
    }
}