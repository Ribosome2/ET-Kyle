namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    [FriendOfAttribute(typeof(ET.Server.SessionPlayerComponent))]
    public class L2G_DisconnectGateUnitHandler : MessageHandler<Scene, L2G_DisconectGateUnit, G2L_DisconnectGateUnit>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconectGateUnit request, G2L_DisconnectGateUnit response)
        {
            CoroutineLockComponent coroutineLockComponent = scene.GetComponent<CoroutineLockComponent>();
            using (await coroutineLockComponent.Wait(CoroutineLockType.LoginGate, request.AccountName.GetLongHashCode()))
            {
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
                Player player = playerComponent.GetByAccount(request.AccountName);

                if (player == null)
                {
                    return;
                }

                scene.GetComponent<GateSessionKeyComponent>().Remove(request.AccountName.GetLongHashCode());
                Session gateSession = player.GetComponent<PlayerSessionComponent>()?.Session;
                if (gateSession != null && !gateSession.IsDisposed)
                {
                    if (gateSession.GetComponent<SessionPlayerComponent>() != null)
                    {
                        gateSession.GetComponent<SessionPlayerComponent>().isLoginAgain = true;
                    }

                    A2C_Disconnect a2CDisconnect = A2C_Disconnect.Create();
                    a2CDisconnect.Error = ErrorCode.ERR_OtherAccountLogin;
                    gateSession.Send(a2CDisconnect);
                    gateSession?.Disconnect().Coroutine();
                }

                if (player.GetComponent<PlayerSessionComponent>()?.Session != null)
                {
                    player.GetComponent<PlayerSessionComponent>().Session = null;
                }

                //加这个组件，是10秒之内不进行登录到Gate就会被踢
                player.AddComponent<PlayerOfflineOutTimeComponent>();
            }

        }
    }
}