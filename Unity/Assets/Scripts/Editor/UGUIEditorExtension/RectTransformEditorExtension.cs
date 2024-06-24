using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UGUIEditorExtension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UGUIEditorExtension
{


    [CustomEditor(typeof(UnityEngine.RectTransform))]
    public class RectTransformEditorExtension : UnityEditorExtensionBase
    {
        public RectTransformEditorExtension() : base("RectTransformEditor")
        {
        }

        string postFix = "(Clone)";
        const int button_height = 25;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rectTrans = target as RectTransform;
            if (rectTrans.name.EndsWith(postFix))
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Ping Prefab", GUILayout.ExpandWidth(false), GUILayout.Height(button_height)))
                {
                    PingPrefab(rectTrans);
                }

                if (GUILayout.Button("打开Prefab", GUILayout.ExpandWidth(false), GUILayout.Height(button_height)))
                {
                    OpenPrefab(rectTrans);
                }



                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
            }


            if (rectTrans.name.StartsWith("m_"))
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("挂CellView", GUILayout.ExpandWidth(false), GUILayout.Height(button_height)))
                {
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            CommonUIPoolEditor.DrawUIPool();
        }



    

      

        private void PingPrefab(RectTransform rectTrans)
        {
            var cleanName = rectTrans.name.Replace(postFix, "");
            var assetsIds = AssetDatabase.FindAssets($"t:Prefab {cleanName}");
            if (assetsIds.Length > 0)
            {
                foreach (var assetsId in assetsIds)
                {
                    var path = AssetDatabase.GUIDToAssetPath(assetsId);
                    if (Path.GetFileNameWithoutExtension(path) == cleanName)
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        EditorGUIUtility.PingObject(prefab);
                        break;
                    }
                }
            }
        }

        private void OpenPrefab(RectTransform rectTrans)
        {
            var cleanName = rectTrans.name.Replace(postFix, "");
            var assetsIds = AssetDatabase.FindAssets($"t:Prefab {cleanName}");
            // PrefabUtility.
            foreach (var assetsId in assetsIds)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetsId);
                if (Path.GetFileNameWithoutExtension(path) == cleanName)
                {
                    PrefabStageUtility.OpenPrefab(path);
                    break;
                }
            }
        }
    }
}