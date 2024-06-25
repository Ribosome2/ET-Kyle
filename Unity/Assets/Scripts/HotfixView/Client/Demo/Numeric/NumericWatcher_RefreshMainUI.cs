namespace ET.Client
{
    [NumericWatcher(SceneType.Demo,NumericType.Level)]
    // [NumericWatcher(SceneType.Demo,NumericType.Gold)]
    // [NumericWatcher(SceneType.Demo,NumericType.Exp)]
    public class NumericWatcher_RefreshMainUI:INumericWatcher
    {
        public void Run(Unit unit, NumbericChange args)
        {
            UI mainUI =unit.Root().GetComponent<UIComponent>().Get(UIType.UIServerSelect);
            if (mainUI != null)
            {
                mainUI.GetComponent<DlgMainComponent>()?.Refresh();
            }
        }
    }
}