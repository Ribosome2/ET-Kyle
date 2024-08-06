using System.Numerics;
using ET.Client;
using TrueSync;

namespace ET
{
    [EntitySystemOf(typeof(LSBulletComponent))]
    [LSEntitySystemOf(typeof(LSBulletComponent))]
    [FriendOfAttribute(typeof(ET.LSBulletComponent))]
    public static partial class LSBulletComponentSystem
    {
        [LSEntitySystem]
        private static void Awake(this ET.LSBulletComponent self)
        {

        }

        [LSEntitySystem]
        private static void LSUpdate(this LSBulletComponent self)
        {

        }

        public static int GetBulletId(this LSBulletComponent self)
        {
            return self._autoId++;
        }

        public static void CreateBullet(this LSBulletComponent self, TSVector pos)
        {
            var lsWorld = self.GetParent<LSWorld>();
            LSBulletComponent lsBulletComponent = lsWorld.GetComponent<LSBulletComponent>();
            LSBullet lsBullet = lsBulletComponent.AddChildWithId<LSBullet>(self.GetBulletId());
            lsBullet.Position = pos;
            lsBullet.Rotation = TSQuaternion.identity;
            
            // 等待表现层订阅的事件完成
            EventSystem.Instance.Publish(self.Root(), new LSCreateBulletEvent() {Pos = pos,BulletId = lsBullet.Id});

        }
    }
}