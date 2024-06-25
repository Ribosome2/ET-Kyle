namespace ET.Server
{
    [MessageLocationHandler(SceneType.Map)]
    public class C2M_TestUnitNumericHandler:MessageLocationHandler<Unit,C2M_TestUnitNumeric,M2C_TestUnitNumeric>
    {
        protected override async ETTask Run(Unit unit, C2M_TestUnitNumeric request, M2C_TestUnitNumeric response)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            int newGold = numericComponent.GetAsInt(NumericType.Gold) + 100;
            int newExp = numericComponent.GetAsInt(NumericType.Exp) + 50;
            int newLevel = numericComponent.GetAsInt(NumericType.Level) + 1;
            
            numericComponent.Set(NumericType.Gold,newGold);
            numericComponent.Set(NumericType.Exp,newExp);
            numericComponent.Set(NumericType.Level,newLevel);

            await ETTask.CompletedTask;
        }
    }
}