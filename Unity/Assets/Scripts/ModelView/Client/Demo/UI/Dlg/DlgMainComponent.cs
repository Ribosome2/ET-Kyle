using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(UI))]
	public class DlgMainComponent: Entity, IAwake
	{
        public ReferenceCollector RefBind;
		#region 节点定义

		public GameObject E_Role {  get	{   return RefBind.Get<GameObject>("E_Role");	}}
		public GameObject E_Bag {  get	{   return RefBind.Get<GameObject>("E_Bag");	}}
		public GameObject E_Battle {  get	{   return RefBind.Get<GameObject>("E_Battle");	}}
		public GameObject E_Make {  get	{   return RefBind.Get<GameObject>("E_Make");	}}
		public GameObject E_Task {  get	{   return RefBind.Get<GameObject>("E_Task");	}}
		public GameObject E_RoleLevel {  get	{   return RefBind.Get<GameObject>("E_RoleLevel");	}}
		public GameObject E_Gold {  get	{   return RefBind.Get<GameObject>("E_Gold");	}}
		public GameObject E_Exp {  get	{   return RefBind.Get<GameObject>("E_Exp");	}}
		public GameObject E_Rank {  get	{   return RefBind.Get<GameObject>("E_Rank");	}}
		public GameObject E_Chat {  get	{   return RefBind.Get<GameObject>("E_Chat");	}}
        #endregion 节点定义
	}
}
