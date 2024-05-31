namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderCompnent>();
            ClientSenderCompnent clientSenderCompnent = root.AddComponent<ClientSenderCompnent>();

            NetClient2Main_Login response = await clientSenderCompnent.LoginAsync(account, password);

            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"请求登录失败，返回错误码:{response.Error}");
                return;
            }
            Log.Debug("请求登录成功 !!!");

            string Token = response.Token;
            
            //获取服务器列表
            C2R_GetServerInfos c2RGetServerInfos = C2R_GetServerInfos.Create();
            c2RGetServerInfos.Account = account;
            c2RGetServerInfos.Token = response.Token;
            R2C_GetServerInfos r2CGetServerInfos = await clientSenderCompnent.Call(c2RGetServerInfos) as R2C_GetServerInfos;
            if (r2CGetServerInfos.Error != ErrorCode.ERR_Success)
            {
                Log.Error("请求服务器列表失败");
                return;
            }

            ServerInfoProto serverInfoProto = r2CGetServerInfos.ServerInfosList[0]; //这里为了演示，我们用第一个，todo：选服UI
            Log.Debug($"请求服务器列表成功，区服名称:{serverInfoProto.ServerName} 区服ID:{serverInfoProto.Id}");
            
            //获取区服角色列表
            C2R_GetRoles c2RGetRoles = C2R_GetRoles.Create();
            c2RGetRoles.Token = Token;
            c2RGetRoles.Account = account;
            c2RGetRoles.ServerId = serverInfoProto.Id;
            R2C_GetRoles r2CGetRoles = await clientSenderCompnent.Call(c2RGetRoles) as R2C_GetRoles;
            if (r2CGetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error("请求区服角色列表失败");
                return;
            }

            RoleInfoProto roleInfoProro = default;
            if (r2CGetRoles.RoleInfo.Count <= 0)
            {
                //无角色信息，则创建角色信息
                
            }
            
            
            

            // root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}