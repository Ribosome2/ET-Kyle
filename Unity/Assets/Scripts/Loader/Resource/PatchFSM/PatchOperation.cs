using ET.PatchFSM;
using YooAsset;

namespace ET
{
    public class PatchOperation:GameAsyncOperation
    {
        private enum ESteps
        {
            None,
            Update,
            Done,
        }
        
        private readonly StateMachine _machine;
        private ESteps _steps = ESteps.None;

        public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
        {
            // 创建状态机
            _machine = new StateMachine(this);
            _machine.AddNode<FsmInitializePackage>();
            _machine.AddNode<FsmUpdatePackageVersion>();
            _machine.AddNode<FsmUpdatePackageManifest>();
            _machine.AddNode<FsmCreatePackageDownloader>();
            _machine.AddNode<FsmDownloadPackageFiles>();
            _machine.AddNode<FsmDownloadPackageOver>();
            _machine.AddNode<FsmClearPackageCache>();
            _machine.AddNode<FsmUpdaterDone>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnAbort()
        {
        }
    }
}