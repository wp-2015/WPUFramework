using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace ABHelper
{
    public partial class BuildScript
    {
        /// <summary>
        /// 保存这次打包中所有bundle名称，所有文件夹名称，所有文件所在的bundle映射
        /// 这里为了避免需要缓存的数据过大、字符串过多。将重复的文件夹路径、bundle名称做了映射，所以有了保存所有bundle名称的文件、所有文件夹路径的文件
        /// </summary>
        /// <param name="bundleRelevanceAssetPath"></param>
        /// <param name="buildedfolderFilePath"></param>
        /// <param name="bundleNameFilePath"></param>
        public static void SaveAssetRelevanceBundle(string bundleRelevanceAssetPath, string buildedfolderFilePath, string bundleNameFilePath)
        {
            string[] folders = new string[0];
            string[] bundleNames = new string[0];
            List<string> assetRelevanceBundleList = new List<string>();
            foreach (var assetWithBundle in AssetRelevanceBundle)
            {
                var assetName = assetWithBundle.Key;
                var bundleName = assetWithBundle.Value;
                var folderPath = Path.GetDirectoryName(assetName).Replace("\\\\", "/").Replace('\\', '/');
                var folderIndex = ArrayUtility.FindIndex(folders, (iterator) => { return iterator == folderPath; });
                if (folderIndex == -1)
                {
                    ArrayUtility.Add(ref folders, folderPath);
                    folderIndex = folders.Length - 1;
                }
                var bundleNameIndex = ArrayUtility.FindIndex(bundleNames, (iterator) => { return iterator == bundleName; });
                if (bundleNameIndex == -1)
                {
                    ArrayUtility.Add(ref bundleNames, bundleName);
                    bundleNameIndex = bundleNames.Length - 1;
                }
                assetRelevanceBundleList.Add(bundleNameIndex + ":" + folderIndex + "," + Path.GetFileName(assetName));
            }
            UtilsEditor.ListToTxt(bundleNameFilePath, new List<string>(bundleNames));
            UtilsEditor.ListToTxt(buildedfolderFilePath, new List<string>(folders));
            UtilsEditor.ListToTxt(bundleRelevanceAssetPath, assetRelevanceBundleList);
        }
    }
}
