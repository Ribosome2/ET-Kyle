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
    }

    private CodeGenContext m_codeGenContext = new CodeGenContext();
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
        
        if(GUILayout.Button("生成",GUILayout.Height(40)))
        {
            if (selectGo == null)
            {
                ShowNotification(new GUIContent("请先选中UI资源"));
                return;
            }
            m_codeGenContext = new CodeGenContext();
            this.m_codeGenContext.UIName = selectGo.name;
            this.m_codeGenContext.AssetPath = AssetDatabase.GetAssetPath(selectGo);
            this.m_codeGenContext.FolderName = new FileInfo(this.m_codeGenContext.AssetPath).Directory.Name;
            GenerateEvent();
        }
    }

    void GenerateComponent()
    {
        
    }

    void GenerateComponentSystem()
    {
        
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
    
    
}
