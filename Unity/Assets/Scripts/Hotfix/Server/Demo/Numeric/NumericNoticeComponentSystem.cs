namespace ET.Server
{
    public static partial class NumericNoticeComponentSystem
    {
        public static void NoticeImdediately(this NumericNoticeComponent self, NumbericChange args)
        {
            Unit unit = self.GetParent<Unit>();

            var msg =M2C_NoticeUnitNumeric.Create(true);
            msg.UnitId = unit.Id;
            msg.NewValue = args.New;
            msg.NumericType = args.NumericType;
            MapMessageHelper.SendToClient(unit, msg);
        }
    }
}