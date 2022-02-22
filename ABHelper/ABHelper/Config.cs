using System.IO;
using UnityEngine;

namespace ABHelper
{
    public static class Config
    {
        public const string VersionFileName         = "Version.txt";                // 记录各个bundle的hash，方便对比差异，确定下载
        public const string AssetFileInfo           = "AssetFileInfo.txt";          // version以及 Asset名字:HashCode
        public const string AssetRelevanceBundle    = "AssetRelevanceBundle.txt";   // 记录asset与bundle的包含关系，哪个bundle里面有哪个asset
        public const string BuildedFolderFileName   = "BuildedFolder.txt";          // 记录所有的打包的文件夹路径
        public const string BundleNameFileName      = "BundleName.txt";             // 记录所有的打包的bundle的名称

        private const string ABFolderName           = "/AssetBundle/";              // bundle文件夹名称
        /// <summary>
        /// rootPath确定到root
        /// </summary>
        public static string GetServerPath(string rootPath, string fileName)
        {
            return rootPath + "/" + CurrentPlatformName + "/" + fileName;
        }
        /// <summary>
        /// 本地保存路径
        /// </summary>
        private static string _NativePathRoot;
        /// <summary>
        /// 保存路径确定为：persistentDataPath + "AssetBundle" + 平台 + path
        /// </summary>
        public static string GetNativePath(string path)
        {
            if(string.IsNullOrEmpty(_NativePathRoot))
            {
                _NativePathRoot = Application.persistentDataPath + ABFolderName + CurrentPlatformName;
            }
            return _NativePathRoot + "/" + path;
            //return Path.Combine(Application.persistentDataPath, GetPlatform(Application.platform));//这里使用Path.Combine结合符为\而不是/
        }
        /// <summary>
        /// 包里面的路径为：streamingAssetsPath + path
        /// </summary>
        public static string GetPackagePath(string path)
        {
            return Application.streamingAssetsPath + "/" + path;
        }
        //取文件的时候,如果streamingAssetsPath没有的话去persistentDataPath查找
        public static string GetFilePath(string path)
        {
            var nativePath = GetNativePath(path);
            if(File.Exists(nativePath))//如果persistentDataPath存在
            {
                return nativePath;
            }
            else
            {
                return GetPackagePath(path);
            }
        }

        private static string _CurrentPlatformName;
        public static string CurrentPlatformName
        {
            get
            {
                if(string.IsNullOrEmpty(_CurrentPlatformName))
                {
                    _CurrentPlatformName = GetPlatform(Application.platform);
                }
                return _CurrentPlatformName;
            }
        }
        public static string GetPlatform(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }
    }
}
