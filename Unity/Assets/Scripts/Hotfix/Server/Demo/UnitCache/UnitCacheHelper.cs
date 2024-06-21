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
    }
}