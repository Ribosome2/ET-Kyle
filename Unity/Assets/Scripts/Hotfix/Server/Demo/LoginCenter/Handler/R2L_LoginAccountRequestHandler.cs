namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public class R2L_LoginAccountRequestHandler:MessageHandler<Scene,R2L_LoginAccountRequest,L2R_LoginAccountRequest>
    {
        protected override async ETTask Run(Scene scene, R2L_LoginAccountRequest request, L2R_LoginAccountRequest response)
        {
            long accountId = request.AccountName.GetLongHashCode();
            CoroutineLockComponent coroutineLockComponent = scene.GetComponent<CoroutineLockComponent>();
            using (await coroutineLockComponent.Wait(CoroutineLockType.LoginCenterLock,accountId))
            {
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExist(accountId))
                {
                    return;
                }


                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);
                StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, request.AccountName);
            
                L2G_DisconectGateUnit l2GDisconectGateUnit = L2G_DisconectGateUnit.Create();
                l2GDisconectGateUnit.AccountName = request.AccountName;
                G2L_DisconnectGateUnit g2LDisconnectGateUnit = (G2L_DisconnectGateUnit)await scene.GetComponent<MessageSender>()
                        .Call(gateConfig.ActorId, l2GDisconectGateUnit);

                response.Error = g2LDisconnectGateUnit.Error;
            }

           
        }
    }
}

