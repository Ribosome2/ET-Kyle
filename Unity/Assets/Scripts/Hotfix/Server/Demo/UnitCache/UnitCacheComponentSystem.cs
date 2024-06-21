using System;

namespace ET.Server
{
    [EntitySystemOf(typeof(UnitCacheComponent))]
    [FriendOfAttribute(typeof(ET.Server.UnitCacheComponent))]
    [FriendOfAttribute(typeof(ET.Server.UnitCache))]
    public static partial class UnitCacheComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.UnitCacheComponent self)
        {

            self.UnitCacheKeyList.Clear();
            foreach (Type type in CodeTypes.Instance.GetTypes().Values)
            {
                if (type != typeof (IUnitCache) && typeof (IUnitCache).IsAssignableFrom(type))
                {
                    self.UnitCacheKeyList.Add(type.Name);
                }
            }

            foreach (string key in self.UnitCacheKeyList)
            {
                UnitCache unitCache = self.AddChild<UnitCache>();
                unitCache.key = key;
                self.UnitCaches.Add(key,unitCache);
            }
        }
        [EntitySystem]
        private static void Destroy(this ET.Server.UnitCacheComponent self)
        {
            foreach (EntityRef<UnitCache> entityRef in self.UnitCaches.Values)
            {
                ((Entity)entityRef)?.Dispose();
            }
            self.UnitCaches.Clear();
        }
        
        public static async ETTask<Entity> Get(this UnitCacheComponent self,long unitId,string key)
        {
            if (!self.UnitCaches.TryGetValue(key, out EntityRef<UnitCache> unitCacheRef))
            {
                 UnitCache unitCache = self.AddChild<UnitCache>();
                 unitCacheRef = unitCache;
                 unitCache.key = key;
                 self.UnitCaches.Add(key,unitCacheRef);
            }

            return await ((UnitCache)unitCacheRef).Get(unitId);
        }


        public static async ETTask AddOrUpdate(this UnitCacheComponent self, long unitId, ListComponent<Entity> entityList)
        {
            using (ListComponent<Entity> list = ListComponent<Entity>.Create())
            {
                foreach (Entity entity in entityList)
                {
                    string key = entity.GetType().Name;
                    if (!self.UnitCaches.TryGetValue(key, out EntityRef<UnitCache> unitCacheRef))
                    {
                        UnitCache unitCache = self.AddChild<UnitCache>();
                        unitCache.key = key; 
                        self.UnitCaches.Add(key, unitCache);
                    }

                    UnitCache unitCache2 = unitCacheRef;
                    unitCache2.AddOrUpdate(entity);
                    list.Add(entity);
                }


                if (list.Count > 0)
                {
                    DBComponent dbComponent =self.Root().GetComponent<DBManagerComponent>().GetZoneDB(self.DomainZone());
                    await dbComponent.Save(unitId, list);
                }
            }
        }

        public static void Delete(this UnitCacheComponent self, long unitId)
        {
            foreach (var cache in self.UnitCaches.Values)
            {
                ((UnitCache)cache).Delete(unitId);
            }
        }

        public static int DomainZone(this UnitCacheComponent self)
        {
            Log.Error("Todo");
            return self.Zone();
        }
    }
}