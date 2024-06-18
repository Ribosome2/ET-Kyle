using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class YooAssetPatchWnd : EditorWindow
{
    [MenuItem("YooAsset/热更工具窗口")]
    public static void OpenWindow()
    {
        GetWindow<YooAssetPatchWnd>();
    }

    private const string fileServerPathPref = "LocalPatchFileServer";
    private void OnGUI()
    {
        string patchServerFolder = EditorPrefs.GetString(fileServerPathPref);
        GUILayout.BeginHorizontal();
        EditorGUILayout.TextField("local FileServer: ",  patchServerFolder);
        if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
        {
            var path = EditorUtility.OpenFolderPanel("select file Folder", patchServerFolder, "");
            if (string.IsNullOrEmpty(path) == false)
            {
                EditorPrefs.SetString(fileServerPathPref, path);
            }
        }

        string bundleFolder = "Bundles\\Android\\DefaultPackage";
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearFolder",GUILayout.Height(50)))
        {
            var allFiles = Directory.GetFiles(patchServerFolder);
            foreach (string file in allFiles)
            {
                File.Delete(file);
            }
        }
        if (GUILayout.Button("CopyBundlesTo",GUILayout.Height(50)))
        {
            var dirs = Directory.GetDirectories(bundleFolder);
            foreach (string dir in dirs)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                //bundle生成目录格式：“2024-06-18-1003”
                if (Regex.Match(directoryInfo.Name, "[\\d_]+").Success)
                {
                    Debug.Log("找到生成目录"+directoryInfo.FullName);
                    var allFiles = Directory.GetFiles(dir);
                    int count=0;
                    foreach (string filePath in allFiles)
                    {
                        var fileInfo = new FileInfo(filePath);
                        var newPath = Path.Combine(patchServerFolder, fileInfo.Name);
                        File.Copy(filePath,newPath);
                        count++;
                    }
                    Debug.Log($"复制了 {count} 个文件到 {patchServerFolder}");
                    break;
                }
            }
        }
        GUILayout.EndHorizontal();
    }
}
