namespace ET.Client
{
    [MessageHandler(SceneType.Demo)]
    public class M2C_NoficeUnitNumericHandler:MessageHandler<Scene,M2C_NoticeUnitNumeric>
    {
        protected override async ETTask Run(Scene scene, M2C_NoticeUnitNumeric message)
        {
            var unitComponent = scene.Root().GetComponent<UnitComponent>();
            unitComponent?.Get(message.UnitId)?.GetComponent<NumericComponent>()?.Set(message.NumericType,message.NewValue);
            await ETTask.CompletedTask;
        }
    }
}