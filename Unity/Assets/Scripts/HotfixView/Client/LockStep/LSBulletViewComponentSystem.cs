using TrueSync;
using UnityEngine;

namespace ET.Client
{
    public static partial class LSBulletViewComponentSystem
    {
        public static async ETTask InitAsync(this LSBulletViewComponent self,TSVector pos,long bulletId)
        {
            Room room = self.Room();
            Scene root = self.Root();
            string assetsName = $"Assets/Bundles/Bullet/Bullet.prefab";
            GameObject bulletPrefab = await room.GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);

            GlobalComponent globalComponent = root.GetComponent<GlobalComponent>();
            GameObject unitGo = UnityEngine.Object.Instantiate(bulletPrefab, globalComponent.Unit, true);
            unitGo.transform.position = pos.ToVector();

            LSBulletView lsUnitView = self.AddChildWithId<LSBulletView, GameObject>(bulletId, unitGo);
        }
    }
}