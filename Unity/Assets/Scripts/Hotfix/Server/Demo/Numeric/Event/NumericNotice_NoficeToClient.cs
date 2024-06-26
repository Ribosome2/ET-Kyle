namespace ET.Server
{
    [Event(SceneType.All)]
    public class NumericChangeEvent_NoticeToClient:AEvent<Scene, NumbericChange>
    {
        protected override async ETTask Run(Scene scene, NumbericChange args)
        {
            if (args.Unit==null)
            {
                return;
            }

            var noticeComponent =args.Unit.GetComponent<NumericNoticeComponent>();
            if (noticeComponent == null)
            {
                Log.Console("no--- "+args.Unit.Id);
            }
            else
            {
                Log.Console("Do Notify--- "+args.Unit.Id);
            }
            noticeComponent?.NoticeImdediately(args);
            
            await ETTask.CompletedTask;
        }
    }
}