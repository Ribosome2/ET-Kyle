using System.Collections;
using System.Collections.Generic;
using UniFramework.Machine;
using UnityEngine;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmUpdaterDone : IStateNode
{
    void IStateNode.OnCreate(StateMachine machine)
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("Patch Finish!");
        
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchFinish.SendEventMessage();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}