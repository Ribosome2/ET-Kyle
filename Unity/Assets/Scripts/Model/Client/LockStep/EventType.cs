using TrueSync;

namespace ET.Client
{
    public struct LSSceneChangeStart
    {
        public Room Room;
    }
    
    public struct LSSceneInitFinish
    {
    }

    public struct LSCreateBulletEvent
    {
        public TSVector Pos;
        public long BulletId;
    }
}