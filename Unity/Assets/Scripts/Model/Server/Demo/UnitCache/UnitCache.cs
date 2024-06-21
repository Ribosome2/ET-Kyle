using System.Collections.Generic;

namespace ET.Server
{
    public interface IUnitCache
    {
        
    }
    [ChildOf(typeof(UnitCacheComponent))]
    public class UnitCache:Entity,IAwake,IDestroy
    {
        public string key;
        public Dictionary<long, EntityRef<Entity>> CacheComponentsDictionary = new Dictionary<long, EntityRef<Entity>>();
    }
}