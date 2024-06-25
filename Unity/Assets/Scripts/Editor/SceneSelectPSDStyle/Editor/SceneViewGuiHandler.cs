// Copyright (c) 2019 Nementic Games GmbH.
// This file is subject to the MIT License. 
// See the LICENSE file in the package root folder for more information.
// Author: Chris Yarbrough

using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

namespace Nementic.SelectionUtility
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Stopwatch = System.Diagnostics.Stopwatch;

    /// <summary>
    ///     The main entry point of the tool which handles the SceneView callback.
    /// </summary>
    [InitializeOnLoad]
    public static class SceneViewGuiHandler
    {
        static SceneViewGuiHandler()
        {
            SetEnabled(UserPreferences.Enabled);
        }

        public static void SetEnabled(bool enabled)
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.beforeSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif

            if (enabled)
            {
#if UNITY_2019_1_OR_NEWER
                SceneView.beforeSceneGui += OnSceneGUI;
#else
                SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif

                // Lazy-initialize members to avoid allocating memory
                // if the tool has been disabled in user preferences.
                if (initialized == false)
                {
                    clickTimer = new Stopwatch();
                    controlIDHint = "NementicSelectionUtility".GetHashCode();
                    gameObjectBuffer = new List<GameObject>(8);
                    initialized = true;
                }
            }
        }

        private static bool initialized;
        private static Stopwatch clickTimer;
        private static int controlIDHint;
        private static List<GameObject> gameObjectBuffer;

        private static void OnSceneGUI(SceneView sceneView)
        {
            try
            {
                Event current = Event.current;
                int id = GUIUtility.GetControlID(controlIDHint, FocusType.Passive);

                // Right mouse button (context-blick).
                if (current.button == 1 && current.shift)
                {
                    HandleMouseButton(current, id,sceneView);
                }
            }
            catch (Exception ex)
            {
                // When something goes wrong, we need to reset hotControl or else
                // the SceneView mouse cursor will stay stuck as a drag hand.
                GUIUtility.hotControl = 0;

                // When opening a UnityEditor.PopupWindow EditGUI throws an exception
                // to break out of the GUI loop. We want to ignore this but still log
                // all other unintended exceptions potentially caused by this tool.
                if (ex.GetType() != typeof(ExitGUIException))
                    Debug.LogException(ex);
            }
        }

        private static void HandleMouseButton(Event current, int id, SceneView sceneView)
        {
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    OnMouseDown();
                    break;

                case EventType.MouseUp:
                    OnMouseUp(current,sceneView);
                    break;
            }
        }

        private static void OnMouseDown()
        {
            clickTimer.Start();
        }

        private static void OnMouseUp(Event current,SceneView sceneView)
        {
            long elapsedMilliseconds = ResetTimer();

            // Only show the selection menu if the click was short,
            // not if the user is holding to drag the SceneView camera.
            if (elapsedMilliseconds < UserPreferences.ClickTimeout)
            {
                GUIUtility.hotControl = 0;
                current.Use();

                var gameObjects = GameObjectsUnderMouse(current.mousePosition,sceneView);

                if (gameObjects.Count > 0)
                {
                    Rect activatorRect = new Rect(current.mousePosition, Vector2.zero);
                    ShowSelectableGameObjectsPopup(activatorRect, gameObjects);
                    current.Use();
                    var sceneViewMotion = Assembly.GetAssembly(typeof(SceneView)).GetType("UnityEditor.SceneViewMotion");
                    var mouseUpMethod = sceneViewMotion.GetMethod("HandleMouseUp", BindingFlags.Static | BindingFlags.NonPublic);
                    mouseUpMethod.Invoke(null,BindingFlags.Static | BindingFlags.NonPublic, null, new object[] { sceneView,GUIUtility.hotControl,2,1}, null);
                    sceneViewMotion.GetMethod("ResetMotion")?.Invoke(null, null);

                }
            }
        }

        private static void ShowSelectableGameObjectsPopup(Rect rect, List<GameObject> options)
        {
#if UNITY_2019_3_OR_NEWER
            var popup = EditorWindow.CreateInstance<UIE_PopupWindow>();
            var content = new UIE_PopupWindowContent(options);
            popup.Show(rect, content);
#else
            var content = new IMGUI_SelectionPopup(options);
            UnityEditor.PopupWindow.Show(rect, content);
#endif
        }

        /// <summary>
        ///     Resets the timer and returns the elapsed time of the last run.
        /// </summary>
        private static long ResetTimer()
        {
            clickTimer.Stop();
            long elapsedTime = clickTimer.ElapsedMilliseconds;
            clickTimer.Reset();
            return elapsedTime;
        }
        static void SelectComponent(Component comp)
        {
            UnityEditor.EditorGUIUtility.PingObject(comp.gameObject);
            Selection.activeGameObject = comp.gameObject;
        }
      
        private static bool CheckHitRectTransform(RectTransform trans, Vector3 worldPos)
        {
            Vector3[] corners = new Vector3[4];
            if (trans != null)
            {
                //获取节点的四个角的世界坐标，分别按顺序为左下左上，右上右下
                trans.GetWorldCorners(corners);
                if (worldPos.x >= corners[0].x && worldPos.y <= corners[1].y && worldPos.x <= corners[2].x &&
                    worldPos.y >= corners[3].y)
                {
                    return true;
                }
            }

            return false;
        }


        static RectTransform[]  GetAllRectTransforms()
        {
            if(PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                return PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponentsInChildren<RectTransform>();
            }
            return GameObject.FindObjectsOfType<RectTransform>();
        }
        
        /// <summary>
        ///     Returns all GameObjects under the provided mouse position.
        /// </summary>
        private static List<GameObject> GameObjectsUnderMouse(Vector2 mousePosition,SceneView sceneView)
        {
            gameObjectBuffer.Clear();

            if (UserPreferences.OnlySelectUIElement)
            {
                Vector3 worldPos = Event.current.mousePosition;
                worldPos.y = sceneView.camera.pixelHeight - worldPos.y;
                worldPos = sceneView.camera.ScreenToWorldPoint(worldPos);
                var targets = GetAllRectTransforms();
                List<GameObject> results =new  List<GameObject>();
                
                foreach (var rectTransform in targets)
                {
                    if (rectTransform.gameObject && CheckHitRectTransform(rectTransform, worldPos))
                    {
                        if (rectTransform.GetComponent<Image>() ||
                            rectTransform.GetComponent<Button>() ||
                            rectTransform.GetComponent<Text>() ||
                            rectTransform.GetComponent<RawImage>()
                        )
                        {

                            results.Add(rectTransform.gameObject);
                        }
                    }
                   
                }
                return results;
            }
            else
            {

                // Unity does not provide an API to retrieve all GameObjects under a SceneView position.
                // So, we pick objects one by one since Unity cycles through them.
                while (true)
                {
                    var go = HandleUtility.PickGameObject(
                        mousePosition,
                        selectPrefabRoot: false,
                        ignore: gameObjectBuffer.ToArray());

                    if (go == null)
                        break;

                    int count = gameObjectBuffer.Count;
                    if (count > 0 && go == gameObjectBuffer[count - 1])
                    {
                        Debug.LogError("Could not ignore game object " + go.name + " when picking.");
                        break;
                    }

                    gameObjectBuffer.Add(go);
                }
            }
            return gameObjectBuffer;
        }
    }
}
