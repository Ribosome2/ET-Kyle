using UnityEngine;
using YooAsset;

namespace ET.Client
{
    public class BundleFixForEditor
    {
        public static void HandleSceneLoadFinish()
        {
#if UNITY_EDITOR
            if (ResourcesComponent.Instance.PlayMode != EPlayMode.HostPlayMode)
            {
                return;
            }
            var renderers = GameObject.FindObjectsOfType<Renderer>();
            FixRenderers(renderers);
#endif
        }

        private static void FixRenderers(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer.materials != null)
                {
                    foreach (var material in renderer.materials)
                    {
                        material.shader = Shader.Find(material.shader.name);
                    }
                }
            }
        }

        public static void HandleLoadAssetDone<T>(UnityEngine.Object AssetObject)
        {
#if UNITY_EDITOR
            if (ResourcesComponent.Instance.PlayMode != EPlayMode.HostPlayMode)
            {
                return;
            }

            if (typeof (T) == typeof (GameObject))
            {
                if (AssetObject != null)
                {
                    var renderers = ((GameObject)AssetObject).GetComponentsInChildren<Renderer>();
                    FixRenderers(renderers);
                }
            }
#endif
        }
    }
}