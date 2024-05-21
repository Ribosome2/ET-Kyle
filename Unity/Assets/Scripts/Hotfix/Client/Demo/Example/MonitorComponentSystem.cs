namespace ET.Client;

[EntitySystemOf(typeof(MonitorComponent))]
[FriendOf(typeof(MonitorComponent))]
public static partial class MonitorComponentSystem
{
    [EntitySystem]
    private static void Awake(this ET.Client.MonitorComponent self, int args2)
    {
        Log.Debug("MonitorComponent Awake");
        self.Brighterness = args2;
    }
    [EntitySystem]
    private static void Destroy(this ET.Client.MonitorComponent self)
    {
        Log.Debug("MonitorComponent Destroy");
    }

    public static void ChangeBritghtness(this MonitorComponent self, int value)
    {
        self.Brighterness = value;
    }
}