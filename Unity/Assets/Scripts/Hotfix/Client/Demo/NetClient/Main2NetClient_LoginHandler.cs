using System;
using System.Net;
using System.Net.Sockets;
using MongoDB.Bson;

namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler: MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            Log.Warning($"try login to router ip: {ConstValue.RouterHttpHost} port :{ConstValue.RouterHttpPort}");
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(routerAddressComponent.RouterManagerIPAddress.AddressFamily, NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);
            Session session = await netComponent.CreateRouterSession(realmAddress, account, password);
            R2C_LoginAccount r2CLogin;
            
            C2R_LoginAccount c2RLogin = C2R_LoginAccount.Create();
            c2RLogin.AccountName = account;
            c2RLogin.Password = password;
            r2CLogin = (R2C_LoginAccount)await session.Call(c2RLogin);

            if (r2CLogin.Error == ErrorCode.ERR_Success)
            {
                root.AddComponent<SessionComponent>().Session = session;
            }
            else
            {
                session?.Dispose();
            }

            response.Token = r2CLogin.Token;
            response.Error = r2CLogin.Error;
        }
    }
}