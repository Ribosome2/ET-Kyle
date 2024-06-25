using System;

namespace ET.Client
{
    public static class NumericHelper
    {
        public static async ETTask<int> TestUpdateNumeric(Scene zoneScene)
        {
            M2C_TestUnitNumeric m2CTestUnitNumeric = null;
            try
            {
                var sessionComponent = zoneScene.GetComponent<ClientSenderComponent>();
                // Log.Error("ddddd "+ sessionComponent);
                // if (sessionComponent == null || sessionComponent.Session == null)
                // {
                //     Log.Error("got null ----  "+ sessionComponent);
                // }
                m2CTestUnitNumeric = (M2C_TestUnitNumeric) await zoneScene.Root().GetComponent<ClientSenderComponent>().Call(C2M_TestUnitNumeric.Create());
                Log.Error("test----result "+ m2CTestUnitNumeric.Error);
                // m2CTestUnitNumeric = (M2C_TestUnitNumeric)await sessionComponent.Session.Call(C2M_TestUnitNumeric.Create());
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            return ErrorCode.ERR_Success;
        }
    }
}