using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf]
    public class ServerInfoComponent:Entity,IAwake,IDestroy
    {
        public List<ServerInfoProto> ServerInfosList;
        public string Account ;
        public string Token ;
    }
}