using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ABHelper
{
    public enum TargetPlatform { Window = BuildTarget.StandaloneWindows64, Android = BuildTarget.Android, IOS = BuildTarget.iOS }
    public partial class BuildScript
    {
        private static Dictionary<string, string> AssetRelevanceBundle = new Dictionary<string, string>();
        public static void CheckPlatformAndBuild()
        {
            var abManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            if ((int)abManifest.BuildTargetPlatform == (int)EditorUserBuildSettings.activeBuildTarget)
            {
                Build();
            }
            else
            {
                EditorUserBuildSettings.activeBuildTargetChanged = delegate ()
                {
                    Build();
                };
                EditorUserBuildSettings.SwitchActiveBuildTarget(abManifest.TargetGroup, (BuildTarget)abManifest.BuildTargetPlatform);
            }
        }

        public static void Build()
        {
            EditorUtility.DisplayProgressBar("ABHelper", "重新生成配置文件", 0);
            //打包之前需要确定manifest文件
            MakeABManifest();
            EditorUtility.DisplayProgressBar("ABHelper", "重新生成配置文件", 1);
            var abManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            //检查配置文件的有效行
            abManifest.CheckAssetDatasValid();

            EditorUtility.DisplayProgressBar("ABHelper", "读取AssetFileInfo文件", 1);

            //统计所有需要打包的资源文件
            var assets = abManifest.AssetDatas;
            var dirs = abManifest.Dirs;
            var abName = abManifest.BundleNames;
            var map = new Dictionary<string, List<string>>();
            var len = assets.Length;
            for (int i = 0; i < len; i++)
            {
                var asset = assets[i];
                var bundleName = abName[asset.BundleNameInde];
                var dir = dirs[asset.DirIndex];
                var path = dir + "/" + asset.Name;
                List<string> assetToBuild;
                if (!map.TryGetValue(bundleName, out assetToBuild))
                {
                    assetToBuild = new List<string>();
                    map[bundleName] = assetToBuild;
                }
                assetToBuild.Add(path);
            }
            var waitforBuild = new Dictionary<string, List<string>>();
            foreach (var item in map)
            {
                var bundleName = item.Key;
                var assetToBuild = item.Value;
                foreach (var asset in assetToBuild)
                {
                    AssetRelevanceBundle[asset] = bundleName;
                }
                waitforBuild[bundleName] = assetToBuild;
            }

            //需要打包的资源文件放入buildMap中
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var ab in waitforBuild)
            {
                builds.Add(new AssetBundleBuild()
                {
                    assetBundleName = ab.Key,
                    assetNames = ab.Value.ToArray()
                });
            }

            EditorUtility.DisplayProgressBar("ABHelper", "开始打包", 1);
            //调用打包接口
            var outputFolder = abManifest.OutputPlatformFolder;//PathUtils.MakeAbsolutePath(abManifest.OutputFolder, Application.dataPath);

            //TODO:暂时先删除以前的所有AB
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            PathUtils.ConfirmDirectoryExist(outputFolder);

            var manifest = BuildPipeline.BuildAssetBundles(outputFolder, builds.ToArray(), 
                                abManifest.BuildOptions, EditorUserBuildSettings.activeBuildTarget);

            //Asset与bundle的包含关系文件
            var bundleRelevanceAssetPath = abManifest.OutputPlatformFolder + "/" + Config.AssetRelevanceBundle;
            var buildedfolderFilePath = abManifest.OutputPlatformFolder + "/" + Config.BuildedFolderFileName;
            var bundleNameFilePath = abManifest.OutputPlatformFolder + "/" + Config.BundleNameFileName;
            SaveAssetRelevanceBundle(bundleRelevanceAssetPath, buildedfolderFilePath, bundleNameFilePath);
            MakeAllFileMD5(abManifest.OutputPlatformFolder);

            EditorUtility.DisplayProgressBar("ABHelper", "生成本次打包记录文件", 1);

            if (EditorUtility.DisplayDialog("ABHelper", "AssetBundle打包完成, 是否需要打开输出文件", "打开", "取消"))
            {
                EditorUtility.OpenWithDefaultApp(outputFolder);
            }
            EditorUtility.ClearProgressBar();
        }

        public static void MakeAllFileMD5(string path)
        {
            var normalPath = path.Replace(@"\", "/") + "/";
            List<string> versions = new List<string>();
            PathUtils.TravelDirectory(path, (dir, name) =>
            {
                if (name.Contains(Config.AssetFileInfo)) return;
                var fullName = (dir + "/" + name).Replace(@"\", "/");
                versions.Add(fullName.Replace(normalPath, "") + ":" + UtilsEditor.GetFileCertificate(fullName));
            });
            UtilsEditor.ListToTxt(path + "/" + Config.VersionFileName, versions);
        }

        /// <summary>
        /// 需要打包的文件是否合法(是否需要过滤的)
        /// </summary>
        public static bool IsFileValid(string fileName, string[] ignoreList)
        {
            for(int i = 0; i < ignoreList.Length; i++)
            {
                if(fileName.EndsWith(ignoreList[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}