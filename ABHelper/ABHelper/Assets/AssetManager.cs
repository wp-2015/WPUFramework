using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ABHelper
{
    public static class AssetManager
    {
        private static bool bIsUseAssetBundle;
        static AssetManager()
        {
            bIsUseAssetBundle = IsUseAssetBundle;
        }
         
        public static T Load<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!bIsUseAssetBundle)
            {
                return (T)AssetDatabase.LoadAssetAtPath<T>(path);
            }
#endif
            return ABManager.Load<T>(path);
        }

        public static void UnLoad(string path)
        {
#if UNITY_EDITOR
            if (bIsUseAssetBundle)
#endif
            {
                ABManager.UnLoad(path);
            }
        }

        public static void UnLoadAllUnuseAsset()
        {
#if UNITY_EDITOR
            if (bIsUseAssetBundle)
#endif
            {
                ABManager.UnLoadUnusefulAssetBundle();
            }
        }

        private static bool IsUseAssetBundle
        {
            get
            {
                return PlayerPrefs.GetString("UseAssetBundle") == "ON";
            }
            set
            {
                string res = value ? "ON" : "OFF";
                if (res == "OFF")
                    Debug.Log("开始直接加载资源");
                else
                    Debug.Log("使用AB模式加载资源");
                PlayerPrefs.SetString("UseAssetBundle", res);
            }
        }
#if UNITY_EDITOR
        [MenuItem("ABHelper/资源加载方式/使用AssetBundle", true)]
        static bool UseBundle()
        {
            return !IsUseAssetBundle && !Application.isPlaying;
        }
        [MenuItem("ABHelper/资源加载方式/使用AssetBundle", false)]
        static void UseBundleShow()
        {
            IsUseAssetBundle = true;
        }
        [MenuItem("ABHelper/资源加载方式/不使用AssetBundle", true)]
        static bool NotUseBundle()
        {
            return IsUseAssetBundle && !Application.isPlaying;
        }
        [MenuItem("ABHelper/资源加载方式/不使用AssetBundle", false)]
        static void NotUseBundleShow()
        {
            IsUseAssetBundle = false;
        }
#endif
    }
}
