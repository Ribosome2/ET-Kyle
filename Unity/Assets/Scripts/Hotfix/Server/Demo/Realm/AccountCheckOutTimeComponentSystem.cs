namespace ET.Server
{

    [Invoke(TimerInvokeType.AccountSessionCheckOutTime)]
    public class AccountSessionCheckOutTimer: ATimer<AccountCheckOutTimeComponent>
    {
        protected override void Run(AccountCheckOutTimeComponent t)
        {
            t?.DeleteSession();
        }
    }

    [EntitySystemOf(typeof (AccountCheckOutTimeComponent))]
    [FriendOf(typeof (AccountCheckOutTimeComponent))]
    public static partial class AccountCheckOutTimeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.AccountCheckOutTimeComponent self, string account)
        {
            self.Account = account;
            self.Root().GetComponent<TimerComponent>().Remove(ref self.Timer);
            //十分钟后执行定时器
            self.Timer = self.Root().GetComponent<TimerComponent>()
                    .NewOnceTimer(TimeInfo.Instance.ServerNow() + 600000, TimerInvokeType.AccountSessionCheckOutTime, self);
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.AccountCheckOutTimeComponent self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.Timer);
        }

        public static void DeleteSession(this AccountCheckOutTimeComponent self)
        {
            Session session = self.GetParent<Session>();

            Session orininSession = session.Root().GetComponent<AccountSessionComponent>().Get(self.Account);
            if (orininSession != null && session.InstanceId == orininSession.InstanceId)
            {
                session.Root().GetComponent<AccountSessionComponent>().Remove(self.Account);
            }
            
            
            A2C_Disconnect a2CDisconnect = A2C_Disconnect.Create();
            a2CDisconnect.Error = 1;
            session?.Send(a2CDisconnect);
            session?.Disconnect().Coroutine();

        }
    }
}