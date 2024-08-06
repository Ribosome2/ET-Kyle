using System;
using ET.Client;
using TrueSync;

namespace ET
{
    [EntitySystemOf(typeof(LSInputComponent))]
    [LSEntitySystemOf(typeof(LSInputComponent))]
    [FriendOfAttribute(typeof(ET.LSInputComponent))]
    public static partial class LSInputComponentSystem
    {
        [EntitySystem]
        private static void Awake(this LSInputComponent self)
        {

        }

        [LSEntitySystem]
        private static void LSUpdate(this LSInputComponent self)
        {
            LSUnit unit = self.GetParent<LSUnit>();
           
            if (self.LSInput.Button == (int)LSInput.FireKeyCode && self.FireInteval<=0)
            {
                Log.Debug("Create bullet ");
                self.FireInteval = 10;
                self.GetParentRecursively<LSWorld>().GetComponent<LSBulletComponent>().CreateBullet(unit.Position);
            }
            self.FireInteval--;


            TSVector2 v2 = self.LSInput.V * 6 * 50 / 1000;
            if (v2.LengthSquared() < 0.0001f)
            {
                return;
            }
            TSVector oldPos = unit.Position;
            unit.Position += new TSVector(v2.x, 0, v2.y);
            unit.Forward = unit.Position - oldPos;
        }
    }
}