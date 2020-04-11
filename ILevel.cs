namespace Munchstein
{
    public interface ILevel
    {
        Platform GetSupportingPlatform(Box2 box);
        Collision GetCollision(Box2 box, Vector2 disposition);
        Door GetAdjacentDoor(Box2 box);
        Munch TryEatMunch(Box2 box);

        void NotifyDoorOpened(Door door);
        void NotifyActorDead();
    }
}