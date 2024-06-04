﻿namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class L2G_DisconnectGateUnitHandler:MessageHandler<Scene,L2G_DisconectGateUnit,G2L_DisconnectGateUnit>
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
                    A2C_Disconnect a2CDisconnect = A2C_Disconnect.Create();
                    // a2CDisconnect.Error = ErrorCode
                }
            }
                
        }
    }
}