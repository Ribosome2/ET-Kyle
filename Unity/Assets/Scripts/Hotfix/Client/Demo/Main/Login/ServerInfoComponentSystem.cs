using System.Collections.Generic;

namespace ET.Client
{
    [EntitySystemOf(typeof(ServerInfoComponent))]
    [FriendOfAttribute(typeof(ET.Client.ServerInfoComponent))]
    public static partial class ServerInfoComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.ServerInfoComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Client.ServerInfoComponent self)
        {

        }

        public static void SetServerData(this ServerInfoComponent self, List<ServerInfoProto> list,string account,string token)
        {
            self.ServerInfosList = list;
            self.Account = account;
            self.Token = token;
        }
    }
}