namespace ET.Server
{
    [EntitySystemOf(typeof(UnitCache))]
    [FriendOfAttribute(typeof(ET.Server.UnitCache))]
    public static partial class UnitCacheSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.UnitCache self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Server.UnitCache self)
        {
            self.key = null;
            self.CacheComponentsDictionary.Clear();
        }

        public static async ETTask<Entity> Get(this UnitCache self, long unitId)
        {
            Entity entity = null;
            if (!self.CacheComponentsDictionary.TryGetValue(unitId, out EntityRef<Entity> entityRef))
            {
                DBComponent dbComponent = self.Root().GetComponent<DBManagerComponent>().GetZoneDB(self.Zone());
                entity = await dbComponent.Query<Entity>(unitId,self.key);
                if (entity != null)
                {
                    self.AddOrUpdate(entity);
                }
            }
            else
            {
                entity = entityRef;
            }
            return entity;
        }

        public static void AddOrUpdate(this UnitCache self, Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            if (self.CacheComponentsDictionary.TryGetValue(entity.Id, out EntityRef<Entity> oldEntityRef))
            {
                var oldEntity = (Entity)oldEntityRef;
                if (entity != oldEntity)
                {
                    oldEntity.Dispose();
                }

                self.CacheComponentsDictionary.Remove(entity.Id);
            }
            self.CacheComponentsDictionary.Add(entity.Id,entity);
        }

        public static void Delete(this UnitCache self, long id)
        {
            if (self.CacheComponentsDictionary.TryGetValue(id, out var entity))
            {
                ((Entity)entity).Dispose();
                self.CacheComponentsDictionary.Remove(id);
            }
        }
    }
}