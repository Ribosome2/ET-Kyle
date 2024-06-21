using System;
using System.Collections.Generic;

namespace ET.Server
{
    public static class UnitCacheHelper
    {
        public static async ETTask AddOrUUpdateUnitCache<T>(this T self) where T : Entity, IUnitCache
        {
            Other2UnitCache_AddOrUpdateUnit message = Other2UnitCache_AddOrUpdateUnit.Create();
            message.UnitId = self.Id;
            message.EntityTypes.Add(typeof(T).FullName);
            message.EntityBytes.Add(MongoHelper.ToJson(self));
            // await MessageHelper.CallActor()
        }
    }
}