namespace ET.Client
{
    [NumericWatcher(SceneType.Current,NumericType.Level)]
    [NumericWatcher(SceneType.Current,NumericType.Gold)]
    [NumericWatcher(SceneType.Current,NumericType.Exp)]
    public class NumericWatcher_RefreshMainUI:INumericWatcher
    {
        public void Run(Unit unit, NumbericChange args)
        {
            UI mainUI =unit.Scene().GetComponent<UIComponent>().Get(UIType.DlgMain);
            if (mainUI != null)
            {
                mainUI.GetComponent<DlgMainComponent>()?.Refresh();
            }
        }
    }
}