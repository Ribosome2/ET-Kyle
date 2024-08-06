namespace ET
{
	[ComponentOf(typeof(Room))]
	public class LSBulletViewComponent: Entity, IAwake, IDestroy
	{
		public EntityRef<LSBulletView> myUnitView;
	}
}