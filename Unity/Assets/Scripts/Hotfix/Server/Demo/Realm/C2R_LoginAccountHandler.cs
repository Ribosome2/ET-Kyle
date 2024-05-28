using System.Text.RegularExpressions;

namespace ET.Server;

[MessageHandler(SceneType.Realm)]
public class C2R_LoginAccountHandler:MessageSessionHandler<C2R_LoginAccount,R2C_LoginAccount>
{
    protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
    {
        session.RemoveComponent<SessionAcceptTimeoutComponent>();

        if (session.GetComponent<SessionLockingComponent>()!=null)
        {
            response.Error = ErrorCode.ERR_RequestRepeatedly;
            session.Disconnect().Coroutine();
            return;
        }

        if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
        {
            response.Error = ErrorCode.ERR_LoginInfoIsNull;
            session.Disconnect().Coroutine();
            return;
        }

        if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6.15}$"))
        {
            response.Error = ErrorCode.ERR_AccountNameFormError;
            session.Disconnect().Coroutine();
            return;
        }
        
        if (!Regex.IsMatch(request.Password.Trim(), @"^[A-za-z0-9]+$"))
        {
            response.Error = ErrorCode.ERR_PasswordFormError;
            session.Disconnect().Coroutine();
            return;
        }

        CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
        using (session.AddComponent<SessionLockingComponent>())
        {
            using (await coroutineLockComponent.Wait(CoroutineLockType.LoginAccount,request.AccountName.GetLongHashCode()))
            {
                
            }
        }
    }
}