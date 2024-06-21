namespace ET.Server
{
	[ComponentOf(typeof(Session))]
	public class SessionPlayerComponent : Entity, IAwake, IDestroy
	{
		private EntityRef<Player> player;
		public bool isLoginAgain = false;

		public Player Player
		{
			get
			{
				return this.player;
			}
			set
			{
				this.player = value;
			}
		}
	}
}