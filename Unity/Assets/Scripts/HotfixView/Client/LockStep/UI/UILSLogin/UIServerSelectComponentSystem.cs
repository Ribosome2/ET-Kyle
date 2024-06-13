using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIServerSelectComponent))]
    [FriendOf(typeof(UIServerSelectComponent))]
    [FriendOfAttribute(typeof(ET.Client.UILSLoginComponent))]
    [FriendOfAttribute(typeof(ET.Client.ServerInfoComponent))]
    public static partial class UIServerSelectComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.UIServerSelectComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            var serverTemplate = rc.Get<GameObject>("serverTemplate");
            var serverList = self.Root().GetComponent<ServerInfoComponent>().ServerInfosList;
            serverTemplate.SetActive(false);
            for (int i = 0; i < serverList.Count; i++)
            {
                var serverSelectUnit = GameObject.Instantiate(serverTemplate,serverTemplate.transform.parent);
                var serverData = serverList[i];
                serverSelectUnit.SetActive(true);
                serverSelectUnit.GetComponentInChildren<Text>().text = serverData.ServerName;
                
                serverSelectUnit.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickServer(self,serverData);
                });
            }
        }

        static void OnClickServer(this UIServerSelectComponent self, ServerInfoProto serverInfoProto)
        {
            var serverInfoComponent = self.Root().GetComponent<ServerInfoComponent>();
            ClientSenderCompnent clientSenderCompnent = self.Root().GetComponent<ClientSenderCompnent>();
            LoginHelper.SelectServer(self.Root(), serverInfoComponent.Account, serverInfoComponent.Token, serverInfoProto,clientSenderCompnent);
            
        }

    }
}
