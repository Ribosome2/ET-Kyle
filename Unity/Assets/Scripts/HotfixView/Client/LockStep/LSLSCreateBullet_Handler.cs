namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSCreateBullet_Handler: AEvent<Scene, LSCreateBulletEvent>
    {
        protected override async ETTask Run(Scene clientScene, LSCreateBulletEvent createBulletEvent)
        {
            Room room = clientScene.GetComponent<Room>();
            
            await room.GetComponent<LSBulletViewComponent>().InitAsync(createBulletEvent.Pos,createBulletEvent.BulletId);
        }
        
        
    }
}