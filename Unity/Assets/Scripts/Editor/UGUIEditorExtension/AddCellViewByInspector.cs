using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UGUIEditorExtension
{
    public class AddChildUnitInfo
    {
        public string cellType;
        public string rootName;
    }
    public class AddCellViewByInspector
    {
       
      
        public static void AddPrefabToPopupRoot(string path,RectTransform root)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj)
            {
                var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                if (root)
                {
                    go.transform.SetParent(root.transform, false);
                    go.name = obj.name;
                }
                EditorGUIUtility.PingObject(go);
            }
        }


        static List<AddChildUnitInfo> GetCreateViewAsyncToTargetNode(string fileContent, RectTransform targetNode)
        {
            List<AddChildUnitInfo> result = new List<AddChildUnitInfo>();
            var addChildUnitInfoList = GetAddChildUnitInfo(fileContent);
            FindMatchInfo(targetNode, addChildUnitInfoList, result);
            
            
            var createViewList = GetCreateViewAsync(fileContent);
            FindMatchInfo(targetNode, createViewList, result);
            
            var createDynamicScrollList = GetCreateDynamicScroll(fileContent,targetNode);
            FindMatchInfo(targetNode, createDynamicScrollList, result);
            return result;
        }

        private static void FindMatchInfo(RectTransform targetNode, List<AddChildUnitInfo> createViewList, List<AddChildUnitInfo> result)
        {
            foreach (var addChildUnitInfo in createViewList)
            {
                if (addChildUnitInfo.rootName == targetNode.name)
                {
                    result.Add(addChildUnitInfo);
                }
            }
        }


        static List<AddChildUnitInfo> GetAddChildUnitInfo(string fileContent)
        {
            Regex pattern = new Regex(@"AddChild<\s*(?<cellType>\w+)\s*>\(\s*(?<root>[\w-]+)\.Transform\s*\)");
            var matches = pattern.Matches(fileContent);
            var result = new List<AddChildUnitInfo>();
            foreach (Match match in matches)
            {
                var unitInfo = new AddChildUnitInfo();
                unitInfo.cellType = match.Groups["cellType"].Value;
                unitInfo.rootName = match.Groups["root"].Value;
                result.Add(unitInfo);
            }
            return result;
        }
        
        static List<AddChildUnitInfo> GetCreateViewAsync(string fileContent)
        {
            Regex pattern = new Regex(@"CreateViewAsync<\s*(?<cellType>\w+)\s*>\(\s*(?<root>[\w-]+)\.Transform\s*\)");
            var matches = pattern.Matches(fileContent);
            var result = new List<AddChildUnitInfo>();
            foreach (Match match in matches)
            {
                var unitInfo = new AddChildUnitInfo();
                unitInfo.cellType = match.Groups["cellType"].Value;
                unitInfo.rootName = match.Groups["root"].Value;
                result.Add(unitInfo);
            }
            return result;
        }
        
        static List<string> FindCellTypesByLogicType(string fileContent)
        {
            Regex pattern = new Regex(@"LogicType\s*=\s*typeof\((?<cellType>\w+)\)");
            var matches = pattern.Matches(fileContent);
            var result = new List<string>();
            foreach (Match match in matches)
            {
                var cellType = match.Groups["cellType"].Value;
                result.Add(cellType);
            }
            return result;
        }
        
        static List<AddChildUnitInfo> GetCreateDynamicScroll(string fileContent,RectTransform targetNode)
        {
            // UIUtil.CreateDynamicScroll(m_NegoList.GameObject, m_NegoListContent.GameObject, null);
            Regex pattern = new Regex(@"CreateDynamicScroll\(\s*(?<scrollView>[\w-.]+)\s*,\s*(?<root>[\w-]+)\.GameObject\s*,");
            var matches = pattern.Matches(fileContent);
            var result = new List<AddChildUnitInfo>();

            foreach (Match match in matches)
            {
                var rootName = match.Groups["root"].Value;
                if (rootName == targetNode.name)
                {
                    var logicTypeList = FindCellTypesByLogicType(fileContent);
                    // 这里找CellType有点麻烦，简便的做法先假设一个界面里面出现 LogicType = typeof(XXXX) 的就是目标CellType
                    foreach (var logicType in logicTypeList)
                    {
                        var unitInfo = new AddChildUnitInfo();
                        unitInfo.cellType = logicType;
                        unitInfo.rootName = rootName;
                        result.Add(unitInfo);
                    }
                }
            }
            return result;
        }
    }
    
    
}