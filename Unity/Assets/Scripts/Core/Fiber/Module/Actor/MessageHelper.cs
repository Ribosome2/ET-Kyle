using System;

namespace ET
{
    public static class MessageHelper
    {
        public static IResponse CreateResponse(Type requestType, int rpcId, int error)
        {
            Type responseType = OpcodeType.Instance.GetResponseType(requestType);
            IResponse response = (IResponse)ObjectPool.Instance.Fetch(responseType);
            response.Error = error;
            response.RpcId = rpcId;
            return response;
        }
        
        /// <summary>
        /// 发送RPC协议给Actor
        /// </summary>
        /// <param name="actorId">注册Actor的InstanceId</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async ETTask<IResponse> CallActor(ActorId actorId, IActorRequest message)
        {
            // return await ActorMessageSenderComponent.Instance.Call(actorId, message);
            Log.Error("Todo");
            IResponse response = null;
            return  response;
        }
    }
}