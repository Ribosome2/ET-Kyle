using System;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_EnterGameHandler:MessageSessionHandler<C2G_EnterGame,G2C_EnterGame>
    {
        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response)
        {
            Log.Console("EnterGameHandler---- "+session.InstanceId);
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                return;
            }

            SessionPlayerComponent sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();
            if (sessionPlayerComponent == null)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                return;
            }

            Player player = sessionPlayerComponent.Player;
            if (player == null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                return;
            }

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            long instanceId = session.InstanceId;

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.LoginGate,player.Account.GetLongHashCode()))
                {
                    if (instanceId != session.InstanceId || player.IsDisposed)
                    {
                        response.Error = ErrorCode.ERR_SessionPlayerError;
                        return;
                    }

                    if (player.PlayerState == PlayerState.Game)  //已经在Game服务器，二次登录
                    {
                        try
                        {
                            G2M_SecondLogin g2MSecondLogin = G2M_SecondLogin.Create();
                            IResponse reqEnter = await session.Root().GetComponent<MessageLocationSenderComponent>()
                                    .Get(LocationType.Unit).Call(player.UnitId, g2MSecondLogin);
                            if (reqEnter.Error == ErrorCode.ERR_Success)
                            {
                                Log.Console("作业：二次登录逻辑，补全下发切换场景消息");
                                var unit = session.Root().GetChild<Unit>(player.UnitId);
                                Log.Console("unitState "+player.PlayerState);
                                var scene = session.Root();
                                // 通知客户端开始切场景
                                M2C_StartSceneChange m2CStartSceneChange = M2C_StartSceneChange.Create();
                                m2CStartSceneChange.SceneInstanceId = scene.InstanceId;
                                m2CStartSceneChange.SceneName = scene.Name;
                                MapMessageHelper.SendToClient(unit, m2CStartSceneChange);

                                // 通知客户端创建My Unit
                                M2C_CreateMyUnit m2CCreateUnits = M2C_CreateMyUnit.Create();
                                m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
                                MapMessageHelper.SendToClient(unit, m2CCreateUnits);
                                return;
                            }
                            

                            
                            Log.Error("二次登录失败 "+reqEnter.Error+ " | "+reqEnter.Message);
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisConnectHelper.KickPlayerNoLock(player);
                            session.Disconnect().Coroutine();
                        }
                        catch (Exception e)
                        {
                            Log.Error("二次登录失败 "+ e.ToString());
                            response.Error = ErrorCode.ERR_ReEnterGameError2;
                            await DisConnectHelper.KickPlayerNoLock(player);
                            
                            session.Disconnect().Coroutine();
                        }
                    }


                    try
                    {
                        // 在Gate上动态创建一个Map Scene, 吧Unit从DB中加载放进来，然后传送到真正的Map中， 这样登录跟传送的逻辑就完全一致了
                        // GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
                        // gateMapComponent.Scene = await GateMapFactory.Create(gateMapComponent, player.Id, IdGenerater.Instance.GenerateInstanceId(), "GateMap");

                        // Scene scene = gateMapComponent.Scene;
                        
                        //这里可以从DB中加载Unit,数据库缓存服是进阶教程内容Orz
                        // Unit unit = UnitFactory.Create(scene, player.Id, UnitType.Player);
                        
                        //从数据库或者缓存中加载出Unit实体及组件
                        (bool isNewPlayer, Unit unit) = await UnitCacheHelper.LoadUnit(player);
                        // unit.AddComponent<UnitGateComponent, long>(player.InstanceId); //todo
                        
                        //玩家Unit上线后的初始化操作
                        await UnitHelper.InitUnit(unit, isNewPlayer); //todo
                        long unitId = unit.Id;

                        StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.Zone(), "Map1");
                        
                        //等到一帧的最后再传送，先让G2C_EnterMap返回，否则传送消息可能比G2C_EnterMap还早
                        TransferHelper.TransferAtFrameFinish(unit,startSceneConfig.ActorId,startSceneConfig.Name).Coroutine();
                        player.UnitId = unitId;
                        response.MyUnitId = unitId;
                        player.PlayerState = PlayerState.Game;

                    }
                    catch (Exception e)
                    {
                       Log.Error($"角色进入游戏逻辑服出现问题 账号Id: {player.Account} 角色Id:{player.Id} 异常消息：{e}");
                       Log.Console($"角色进入游戏逻辑服出现问题 账号Id: {player.Account} 角色Id:{player.Id} 异常消息：{e}");
                       response.Error = ErrorCode.ERR_EnterGameError;
                       await DisConnectHelper.KickPlayerNoLock(player);
                       session.Disconnect().Coroutine();
                    }
                }
            }
        }
    }
}