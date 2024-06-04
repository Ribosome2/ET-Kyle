namespace ET.Server
{
    [ChildOf(typeof(Player))]
    public class PlayerOfflineOutTimeComponent:Entity,IAwake,IDestroy
    {
        public long Timer;

    }
}
