namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class GetServerListFinish_CreateServerSelectUI: AEvent<Scene, GetServerListFinish>
	{
		protected override async ETTask Run(Scene scene, GetServerListFinish args)
		{
			await UIHelper.Remove(scene, UIType.UILogin);
			await UIHelper.Create(scene, UIType.UIServerSelect,UILayer.High);
		}
	}
}
