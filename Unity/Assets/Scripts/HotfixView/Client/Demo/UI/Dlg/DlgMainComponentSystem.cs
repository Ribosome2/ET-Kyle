using System;
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
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.RefBind = rc;

            self.Refresh().Coroutine();
            
            self.E_Role.GetComponent<Button>().onClick.AddListener(() =>
            {
                self.OnClickRoleButton().Coroutine();
            });
        }

        public static async ETTask OnClickRoleButton( this DlgMainComponent self)
        {
            try
            {
                int error = await NumericHelper.TestUpdateNumeric(self.Scene());
                if (error != ErrorCode.ERR_Success)
                {
                    return;
                }
                Log.Debug("发送更新属性测试消息成功");
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public static async ETTask Refresh(this DlgMainComponent self)
        {
            Log.Console("refresh MainUI ");
            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Scene());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            self.E_RoleLevel.GetComponent<Text>().text = $"Lv.{numericComponent.GetAsInt((int)NumericType.Level)}";
            self.E_Gold.GetComponent<Text>().text = $"Lv.{numericComponent.GetAsInt((int)NumericType.Gold)}";
            self.E_Exp.GetComponent<Text>().text = $"Lv.{numericComponent.GetAsInt((int)NumericType.Exp)}";
            await ETTask.CompletedTask;
        }
    }
}
