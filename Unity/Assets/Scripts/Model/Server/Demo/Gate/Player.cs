namespace ET.Server
{
    public enum PlayerState
    {
        Disconnect,
        Gate,  //连接了Gate但是还没传送到Game服务器
        Game,
    }
    
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity, IAwake<string>
    {
        public string Account { get; set; }
        public PlayerState PlayerState { get; set; }
        public long UnitId { get; set; }
    }
}