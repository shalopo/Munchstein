namespace Munchstein
{
    public interface ILevel
    {
        Platform GetSupportingPlatform(BoxBoundary box);
        Vector2 GetCollisionBox(BoxBoundary box, Vector2 disposition);
        Door GetAdjacentDoor(Point2 point);

        void NotifyDoorOpened(Door door);
        void NotifyActorDead();
    }
}