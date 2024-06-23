namespace ET.Server
{
    [EntitySystemOf(typeof(SessionPlayerComponent))]
    [FriendOfAttribute(typeof(ET.Server.SessionPlayerComponent))]
    public static partial class SessionPlayerComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this SessionPlayerComponent self)
        {
            Scene root = self.Root();
            self.isLoginAgain = false;
            if (root.IsDisposed)
            {
                return;
            }
            
            // 发送断线消息
            // root.GetComponent<MessageLocationSenderComponent>().Get(LocationType.Unit).Send(self.Player.Id, G2M_SessionDisconnect.Create());

            if (!self.isLoginAgain && self.Player != null)
            {
                Log.Console("非二次登录，踢玩家下线"+self.Player.InstanceId);
                DisConnectHelper.KickPlayer(self.Player).Coroutine();
            }
            
            if (self.Player == null || self.Player.IsDisposed)
            {
                return;
            }
            Log.Console("Player Session destroy" + self.Player.Id);

            Session playerSession = self.Player.GetComponent<PlayerSessionComponent>()?.Session;
            if (playerSession == null)
            {
                return;
            }

            if (playerSession.InstanceId != self.GetParent<Session>().InstanceId)
            {

                return;
            }

            if (self.Player.GetComponent<PlayerOfflineOutTimeComponent>() == null)
            {
                self.Player.AddComponent<PlayerOfflineOutTimeComponent>();
                return;
            }
        }

        [EntitySystem]
        private static void Awake(this SessionPlayerComponent self)
        {

        }
    }
}