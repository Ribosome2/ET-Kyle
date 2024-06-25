namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常


        public const int ERR_RequestRepeatedly = 200001;
        public const int ERR_LoginInfoIsNull = 200002;
        public const int ERR_AccountNameFormError = 200003;
        public const int ERR_PasswordFormError = 200004;
        public const int ERR_AccountInBlackListError = 200005;
        public const int ERR_AccountPasswordError = 200006;
        public const int ERR_TokenError= 200007;
        
        //创角色 和删除角色
        public const int ERR_RoleNameIsNull = 200008;
        public const int ERR_RoleNameSame = 200009;
        public const int ERR_RoleNotExist = 200010;
        
        //连接Gate
        public const int ERR_ConnectGateKeyError = 200011;
        public const int ERR_LoginGameGateError01 = 200012;
        public const int ERR_OtherAccountLogin = 200013;
        
        //Game
        public const int ERR_SessionPlayerError = 200100;
        public const int ERR_NonePlayerError = 200101;
        public const int ERR_EnterGameError = 200102;
        public const int ERR_ReEnterGameError = 200103;
        public const int ERR_ReEnterGameError2 = 200104;
        public const int ERR_NetWorkError = 200105;
        
    }
}