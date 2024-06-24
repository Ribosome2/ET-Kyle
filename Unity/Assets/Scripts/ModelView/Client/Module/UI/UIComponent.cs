using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
	public interface IUILogic
	{
        
	}
	
	public interface IUIScrollItem<T> where T : Entity,IAwake
	{
		public T BindTrans(Transform trans);
	}


	/// <summary>
	/// 管理Scene上的UI
	/// </summary>
	[ComponentOf]
	public class UIComponent: Entity, IAwake
	{
		public Dictionary<string, EntityRef<UI>> UIs = new();

		private EntityRef<UIGlobalComponent> uiGlobalComponent;

		public UIGlobalComponent UIGlobalComponent
		{
			get
			{
				return this.uiGlobalComponent;
			}
			set
			{
				this.uiGlobalComponent = value;
			}
		}
		
		public Dictionary<int, UIBaseWindow> AllWindowsDic     = new Dictionary<int, UIBaseWindow>();
		public List<WindowID> UIBaseWindowlistCached           = new List<WindowID>();
		public Dictionary<int, UIBaseWindow> VisibleWindowsDic = new Dictionary<int, UIBaseWindow>();
		public Queue<WindowID> StackWindowsQueue               = new Queue<WindowID>();
		public bool IsPopStackWndStatus                        = false;
	}
}