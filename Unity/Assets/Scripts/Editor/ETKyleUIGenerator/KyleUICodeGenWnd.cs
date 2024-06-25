using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class KyleUICodeGenWnd : EditorWindow
{
    [MenuItem("ET/KyleUIGen")]
    public static void KyleUICodeGenrator()
    {
        GetWindow<KyleUICodeGenWnd>();
    }

    public class CodeGenContext
    {
        public string UIName;
        public string AssetPath;
        public string FolderName;
        public GameObject uiRoot;
    }

    private CodeGenContext m_codeGenContext = new CodeGenContext();
    private bool overrideComponentSystemFile = false;
    private void OnGUI()
    {
        var selectGo = Selection.activeGameObject;
        if (selectGo == null)
        {
            GUILayout.Label("先选择UI资源根节点");
        }
        else
        {
            GUILayout.Label("当前选中"+selectGo.name);
        }

        this.overrideComponentSystemFile = EditorGUILayout.Toggle("覆盖ComponentSystem文件", this.overrideComponentSystemFile);
        if(GUILayout.Button("生成",GUILayout.Height(40)))
        {
            if (selectGo == null)
            {
                ShowNotification(new GUIContent("请先选中UI资源"));
                return;
            }
            m_codeGenContext = new CodeGenContext();
            this.m_codeGenContext.UIName = selectGo.name;
            this.m_codeGenContext.uiRoot = selectGo;
            this.m_codeGenContext.AssetPath = AssetDatabase.GetAssetPath(selectGo);
            this.m_codeGenContext.FolderName = new FileInfo(this.m_codeGenContext.AssetPath).Directory.Name;
            GenerateEvent();
            GenerateComponent();
            GenerateComponentSystem();
            Debug.Log("生成成功");
        }
    }

    void GenerateComponent()
    {

        string componentTemplate =
@"using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(UI))]
	public class XXXXXXComponent: Entity, IAwake
	{
        public ReferenceCollector RefBind;
		#region 节点定义
		
		
		#endregion 节点定义
	}
}
";
        
        var folderPath =$"Assets/Scripts/ModelView/Client/Demo/UI/{this.m_codeGenContext.FolderName}";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string eventFilePath =Path.Combine(folderPath,$"{this.m_codeGenContext.UIName}Component.cs");
        StringBuilder sb = new StringBuilder();
        string fileContent;
        if (!File.Exists(eventFilePath))
        {
            fileContent = componentTemplate.Replace("XXXXXX", this.m_codeGenContext.UIName);
        }
        else
        {
            fileContent = File.ReadAllText(eventFilePath);
        }
        InsertFieldDeclare(ref fileContent, this.m_codeGenContext.uiRoot.GetComponent<ReferenceCollector>());
        File.WriteAllText(eventFilePath,fileContent);
    }

    void GenerateComponentSystem()
    {
        var folderPath =$"Assets/Scripts/HotfixView/Client/Demo/UI/{this.m_codeGenContext.FolderName}";
        string systemFilePath =Path.Combine(folderPath,$"{this.m_codeGenContext.UIName}ComponentSystem.cs");
        if (File.Exists(systemFilePath) && this.overrideComponentSystemFile==false)
        {
            return;
        }
        
        string systemTemplate =
                @"using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(XXXUIName_Component))]
    [FriendOfAttribute(typeof(ET.Client.XXXUIName_Component))]
    public static partial class XXXUIName_ComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.XXXUIName_Component self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.RefBind = rc;
        }
    }
}
";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(systemTemplate.Replace("XXXUIName_", this.m_codeGenContext.UIName));
        File.WriteAllText(systemFilePath,sb.ToString());
    }

    void GenerateEvent()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("");
        sb.AppendLine("namespace ET.Client\n{");
        sb.AppendLine($"\t[UIEvent(UIType.{this.m_codeGenContext.UIName})]");
        sb.AppendLine($"\tpublic class {this.m_codeGenContext.UIName}Event: AUIEvent");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tpublic override async ETTask<UI> OnCreate(UIComponent uiComponent, UILayer uiLayer)");
        sb.AppendLine("\t\t{");
        sb.AppendLine($"\t\t\tstring assetsName =\"{this.m_codeGenContext.AssetPath}\";");
        sb.AppendLine("\t\t\tGameObject bundleGameObject = await uiComponent.Scene().GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);");
        sb.AppendLine("\t\t\tGameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, uiComponent.UIGlobalComponent.GetLayer((int)uiLayer));");
        sb.AppendLine("\t\t\tUI ui = uiComponent.AddChild<UI, string, GameObject>(UIType.UIServerSelect, gameObject);");
        sb.AppendLine($"\t\t\tui.AddComponent<{this.m_codeGenContext.UIName}Component>();");
        sb.AppendLine("\t\t\treturn ui;");
        sb.AppendLine("\t\t}");
        sb.AppendLine("");
        sb.AppendLine("\t\tpublic override void OnRemove(UIComponent uiComponent)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        var folderPath =$"Assets/Scripts/HotfixView/Client/Demo/UI/{this.m_codeGenContext.FolderName}";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string eventFilePath =Path.Combine(folderPath,$"{this.m_codeGenContext.UIName}Event.cs");
        File.WriteAllText(eventFilePath,sb.ToString());

    }
    
    private static void InsertFieldDeclare(ref string code, ReferenceCollector referenceCollector)
    {
        if (referenceCollector == null)
            return;

        int beginIndex = code.IndexOf("#region 节点定义\n");
        int endIndex = code.IndexOf("#endregion 节点定义\n");
        if (beginIndex == -1 || endIndex == -1)
            return;

        var allGameObject = referenceCollector.data;
        string temp = code.Substring(0, beginIndex);
        temp += "#region 节点定义\n";
        temp += GenerateField(ref allGameObject);
        temp += "\n";
        temp += "        "+code.Substring(endIndex);

        code = temp;
    }

    private const string FileGetTemplate = @"
		public GameObject RefKey {  get	{   return RefBind.Get<GameObject>(""RefKey"");	}}";
    private static string GenerateField(ref List<ReferenceCollectorData> allGameObject)
    {
        string add = "";
        foreach (var collectorData in allGameObject)
        {
            if (collectorData.gameObject != null)
            {
                add += FileGetTemplate.Replace("RefKey", collectorData.key);
            }
        }
        return add;
    }
}
