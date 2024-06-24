using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UGUIEditorExtension
{
    [System.Serializable]
    public class UIPoolUnit
    {
        public string PreviewImagePath;
        public string PrefabPath;
    }

    [System.Serializable]
    public class UIPoolGroup
    {
        public string GroupName;
        public List<UIPoolUnit> PoolUnits = new List<UIPoolUnit>();
    }

    [System.Serializable]
    public class UIPoolConfigList
    {
        public List<UIPoolGroup> Groups = new List<UIPoolGroup>();
    }

    public class CommonUIPoolEditor
    {
        static bool s_showUIPool = false;
        private const string ConfigPath = "Assets/Editor/UGUIEditorExtension/CommonUIPoolConfig/Editor/CommonUIPoolConfig.json";

        private static UIPoolConfigList s_poolConfigList;


        static void CheckInit()
        {
            if (s_poolConfigList == null)
            {
                ReloadConfig();
            }
        }

        private static void ReloadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                var fileContent = File.ReadAllText(ConfigPath);
                s_poolConfigList = JsonUtility.FromJson<UIPoolConfigList>(fileContent);
            }
            else
            {
                s_poolConfigList = new UIPoolConfigList();
            }
        }

        public static void DrawUIPool()
        {
            // if (GUILayout.Button(" test"))
            // {
                // UIPoolConfigList test = new UIPoolConfigList();
                // var group = new UIPoolGroup();
                // group.GroupName = "按钮";
                // group.PoolUnits.Add(new UIPoolUnit()
                // {
                //     PrefabPath = "Assets/PatchResources/UI/BackGround/Btn/ExampleBtn1.prefab",
                //     PreviewImagePath = "teee",
                // });
                // test.Groups.Add(group);
                //
                // File.WriteAllText(ConfigPath, JsonUtility.ToJson(test, true));
            // }
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
            DrawHeader();
            if (s_showUIPool)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("reload",GUILayout.ExpandWidth(false)))
                {
                    ReloadConfig();
                }
                GUILayout.Label("点击图片创建到当前节点下",GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                CheckInit();
                EditorGUILayout.Separator();
                if (s_poolConfigList != null)
                {
                    
                    foreach (var group in s_poolConfigList.Groups)
                    {
                        foreach (UIPoolUnit uiPoolUnit in group.PoolUnits)
                        {
                            DrawSingleUnit(uiPoolUnit);
                        }
                    }
                }
            }
        }

        private static void DrawSingleUnit(UIPoolUnit uiPoolUnit)
        {
            if (File.Exists(uiPoolUnit.PreviewImagePath))
            {
                var preview = AssetDatabase.LoadAssetAtPath<Texture>(uiPoolUnit.PreviewImagePath);
                if (GUILayout.Button(preview, GUILayout.ExpandWidth(false)))
                {
                    CreatePrefab(uiPoolUnit.PrefabPath);
                }
            }
            else
            {
                GUILayout.Label($"文件{uiPoolUnit.PreviewImagePath}不存在");
            }
        }

        private static void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string switchStr = s_showUIPool ? "隐藏通用UI库" : "显示通用UI库";
            if (GUILayout.Button(switchStr, GUILayout.ExpandWidth(false)))
            {
                s_showUIPool = !s_showUIPool;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void CreatePrefab(string prefabPath)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var go = new GameObject("go", typeof(RectTransform));
            go.transform.parent = Selection.activeTransform;
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.GetComponent<RectTransform>().sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;
            var ctx = new ConvertToPrefabInstanceSettings();
            PrefabUtility.ConvertToPrefabInstance(go, prefab, ctx, InteractionMode.AutomatedAction);
            Undo.RegisterCreatedObjectUndo(go, "创建通用资源");
            EditorGUIUtility.PingObject(go);

            // Selection.activeGameObject = go;
        }
    }
}