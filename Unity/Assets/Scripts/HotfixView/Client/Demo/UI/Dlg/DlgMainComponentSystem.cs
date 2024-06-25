using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(DlgMainComponent))]
    [FriendOfAttribute(typeof(ET.Client.DlgMainComponent))]
    public static partial class DlgMainComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.DlgMainComponent self)
        {
           
        }
    }
}
