 using System.Collections.Generic;

 namespace ET.Server
{

    /// <summary>
    /// 记录账户名hash和所在服务器的Id的映射
    /// </summary>
    [ComponentOf(typeof (Scene))]
    public class LoginInfoRecordComponent: Entity, IAwake,IDestroy
    {

        public Dictionary<long, int> AccountLoginInfoDict = new Dictionary<long, int>(); 
    }
}