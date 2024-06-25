using System;
using System.Collections.Generic;
using MongoDB.Driver.Core.Events;

namespace ET.Server
{
    public static class UnitCacheHelper
    {
        /// <summary>
        /// 增加或者更新玩家组件缓存
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        public static async ETTask AddOrUUpdateUnitCache<T>(this T self) where T : Entity, IUnitCache
        {
            Other2UnitCache_AddOrUpdateUnit message = Other2UnitCache_AddOrUpdateUnit.Create();
            message.UnitId = self.Id;
            message.EntityTypes.Add(typeof(T).FullName);
            message.EntityBytes.Add(self.ToBson());
            var scene = self.Root();
            var actorId = StartSceneConfigCategory.Instance.GetUnitCacheConfig(self.Id).ActorId;
            await scene.GetComponent<MessageSender>().Call(actorId, message);
            // await MessageHelper.CallActor(StartSceneConfigCategory.Instance.GetUnitCacheConfig(self.Id).ActorId, message);
        }

        public static async ETTask<(bool,Unit)> LoadUnit(Player player)
        {
            GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
            gateMapComponent.Scene = await GateMapFactory.Create(gateMapComponent, player.Id, IdGenerater.Instance.GenerateInstanceId(), "GateMap");

            Unit unit = await UnitCacheHelper.GetUnitCache(gateMapComponent.Scene, player.UnitId);
            bool isNewUnit = unit == null;
            if (isNewUnit)
            {
                unit = UnitFactory.Create(gateMapComponent.Scene, player.Id, UnitType.Player);
                UnitCacheHelper.AddOrUpdateUnitAllCache(unit);
            }

            return (isNewUnit, unit);

        }
        public static async ETTask<Unit> GetUnitCache(Scene scene, long unitId)
        {
            var actorId = StartSceneConfigCategory.Instance.GetUnitCacheConfig(unitId).ActorId;
            Other2UnitCache_GetUnit message = Other2UnitCache_GetUnit.Create();
            UnitCache2Other_GetUnit queryUnit = (UnitCache2Other_GetUnit)await UnitHelper.CallActor(actorId,scene,message);
            if (queryUnit.Error != ErrorCode.ERR_Success || queryUnit.EntityList.Count <= 0)
            {
                return null;
            }

            int indexOf = queryUnit.ComponentNameList.IndexOf(nameof (Unit));
            Unit unit = queryUnit.EntityList[indexOf] as Unit;
            if (unit == null)
            {
                return null;
            }

            // scene.AddChild(unit);//6.0 add 到Scene 8不行
            scene.GetComponent<UnitComponent>().AddChild(unit);
            foreach (Entity entity in queryUnit.EntityList)
            {
                if (entity == null || entity is Unit)
                {
                    continue;
                }

                unit.AddComponent(entity);
            }

            return unit;
        }


        /// <summary>
        /// 获取玩家缓存组件
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="scene"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ETTask<T> GetUnitComponentCahce<T>(long unitId,Scene scene) where T : Entity, IUnitCache
        {
            
            //todo: 怎么根据UnitId获取scene，而不用传进来？
            Other2UnitCache_GetUnit message = Other2UnitCache_GetUnit.Create();
            message.UnitId = unitId;
            message.ComponentNameList.Add(typeof(T).Name);
            
            var actorId = StartSceneConfigCategory.Instance.GetUnitCacheConfig(unitId).ActorId;
            UnitCache2Other_GetUnit queryUnit =(UnitCache2Other_GetUnit) await scene.GetComponent<MessageSender>().Call(actorId, message);
            if (queryUnit.Error == ErrorCode.ERR_Success && queryUnit.EntityList.Count > 0)
            {
                return queryUnit.EntityList[0] as T;
            }

            return null;

        }

        public static async ETTask DeleteUnitCache(long unitId,Scene scene)
        {
            //todo: 怎么根据UnitId获取scene，而不用传进来？
            Other2UnitCache_DeleteUnit msg = Other2UnitCache_DeleteUnit.Create();
            msg.UnitId = unitId;
            var actorId = StartSceneConfigCategory.Instance.GetUnitCacheConfig(unitId).ActorId;
            await scene.GetComponent<MessageSender>().Call(actorId, msg);
        }


        public static void AddOrUpdateUnitAllCache(Unit unit)
        {
            Other2UnitCache_AddOrUpdateUnit message = Other2UnitCache_AddOrUpdateUnit.Create();
            message.EntityTypes.Add(unit.GetType().FullName);
            message.EntityBytes.Add(MongoHelper.ToBson(unit));

            foreach(var kv in unit.Components)
            {
                var entityType = kv.Value.GetType();
                if (!typeof (IUnitCache).IsAssignableFrom(entityType))
                {
                    continue;
                }
                message.EntityTypes.Add(entityType.FullName);
                message.EntityBytes.Add(MongoHelper.ToBson(kv.Value));
            }
            
            UnitHelper.CallActor(StartSceneConfigCategory.Instance.GetUnitCacheConfig(unit.Id).ActorId,unit.Root(),message).Coroutine();
            
        }
    }
}