namespace Munchstein
{
    public interface ILevel
    {
        Platform GetSupportingPlatform(Box2 box);
        Platform GetCollidingPlatform(Box2 box);
        Door GetAdjacentDoor(Box2 box);
        Munch TryEatMunch(Box2 box);

        void NotifyDoorOpened(Door door);
        void NotifyActorDeath(Actor actor);
    }
}