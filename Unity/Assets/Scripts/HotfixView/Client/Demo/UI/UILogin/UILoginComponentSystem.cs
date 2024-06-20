using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[EntitySystemOf(typeof(UILoginComponent))]
	[FriendOf(typeof(UILoginComponent))]
	public static partial class UILoginComponentSystem
	{
		private const string PrefKeyAccount="ETLastLoginAccount";
		private const string PrefKeyPsw="ETLastLoginPassword";
		[EntitySystem]
		private static void Awake(this UILoginComponent self)
		{
			ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			self.loginBtn = rc.Get<GameObject>("LoginBtn");
			
			self.loginBtn.GetComponent<Button>().onClick.AddListener(()=> { self.OnLogin(); });
			self.account = rc.Get<GameObject>("Account");
			self.password = rc.Get<GameObject>("Password");
			self.version = rc.Get<GameObject>("Version");
			self.version.GetComponent<Text>().text="版本号:"+YooAsset.YooAssets.GetPackage("DefaultPackage").GetPackageVersion();

			self.account.GetComponent<InputField>().text = PlayerPrefs.GetString(PrefKeyAccount);
			self.password.GetComponent<InputField>().text = PlayerPrefs.GetString(PrefKeyPsw);
			PlayerPrefs.SetString(PrefKeyAccount,self.account.GetComponent<InputField>().text);
			PlayerPrefs.SetString(PrefKeyPsw,self.password.GetComponent<InputField>().text);
		}

		
		public static void OnLogin(this UILoginComponent self)
		{
			LoginHelper.Login(
				self.Root(), 
				self.account.GetComponent<InputField>().text, 
				self.password.GetComponent<InputField>().text).Coroutine();
			
			PlayerPrefs.SetString(PrefKeyAccount,self.account.GetComponent<InputField>().text);
			PlayerPrefs.SetString(PrefKeyPsw,self.password.GetComponent<InputField>().text);
		}
	}
}
