using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ABHelper
{
    public class ABUpdate : MonoBehaviour
    {
        public int _UpdateABCount;  //需要更新ab总数
        public int _UpdateABIndex;  //已经更新ab总数
        private static Action<float> _ProcessChangeCB;
        private static string _RootURL;
        public static float CurrentUpdateABProcess //当前更新的AB的进度
        {   get
            {
                var currentDownload = DownloadManager.CurrentDownload;
                if (null != currentDownload)
                {
                    return currentDownload.Progress;
                }
                return 0;
            }
        }
        /// <summary>
        /// 这里回调中的process为已经更新的bundle数量/总的需要更新的bundle数量
        /// </summary>
        public static void Start(string rootURL, bool needUpdate = true, Action<float> processChangeCB = null)
        {
            if(!needUpdate)
            {
                ABManager.InitWithUpdate();
                return;
            }
            _RootURL = rootURL;
            var instance = FindObjectOfType<ABUpdate>();
            if(instance == null)
            {
                instance = new GameObject("AB").AddComponent<ABUpdate>();
            }
            _ProcessChangeCB = processChangeCB;
        }

        private void Awake()
        {
            BundleUpdate();
        }

        public void BundleUpdate()
        {
            WebAsset webAsset = null;
            var path = ABConfig.GetServerPath(_RootURL, ABConfig.VersionFileName);
            Debug.Log(string.Format("Server AB Path: {0}", path));
            webAsset = WebAssetManager.Load(path, (msg)=>
            {
                //远端的version文件
                var text = (string)msg;
                Dictionary<string, string> newABVersions = new Dictionary<string, string>();
                text = text.Replace("\r", "");
                var items = text.Split('\n');
                for (int i = 0; i < items.Length; i++)
                {
                    var version = items[i].Split(':');
                    if(version.Length > 1 && version[0] != ABConfig.VersionFileName) //对比文件差异时排除version文件
                        newABVersions[version[0]] = version[1];
                }
                //已经更新过的version文件
                Dictionary<string, string> nativeABVersions = new Dictionary<string, string>();
                var nativePath = ABConfig.GetNativePath(ABConfig.VersionFileName);
                UtilsRuntime.TxtToDic(nativePath, nativeABVersions);
                //打在包里的version文件
                Dictionary<string, string> packageABVersions = new Dictionary<string, string>();
                var packagePath = ABConfig.GetPackagePath(ABConfig.VersionFileName);
                UtilsRuntime.TxtToDic(packagePath, packageABVersions);
                //两边的version文件综合起来
                foreach(var version in packageABVersions)
                {
                    if(!nativeABVersions.TryGetValue(version.Key, out string res))
                    {
                        nativeABVersions.Add(version.Key, version.Value);
                    }
                }
                //筛查需要更新的文件
                Dictionary<string, string> waitForUpdate = new Dictionary<string, string>();
                UtilsRuntime.MakeFileSafe(nativePath);
                StreamWriter swVersion = new StreamWriter(nativePath);
                foreach (var versions in newABVersions)
                {
                    var bundleName = versions.Key;
                    var version = versions.Value;
                    if (!nativeABVersions.TryGetValue(bundleName, out string ver) || ver != version)
                    {
                        waitForUpdate.Add(bundleName, version);
                    }
                    else
                    {
                        swVersion.WriteLine(bundleName + ":" + version);
                    }
                }
                if(waitForUpdate.Count <= 0)
                {
                    Debug.Log("没有Asset可更新");
                    UpdateFinish();
                }

                foreach(var versions in waitForUpdate)
                {
                    var bundleName = versions.Key;
                    var version = versions.Value;
                    _UpdateABCount++;
                    DownloadManager.Download(ABConfig.GetServerPath(_RootURL, bundleName), ABConfig.GetNativePath(bundleName), delegate
                    {
                        _UpdateABIndex++;
                        _ProcessChangeCB?.Invoke((float)_UpdateABIndex / _UpdateABCount);
                        swVersion.WriteLine(bundleName + ":" + version);
                        if (_UpdateABIndex == _UpdateABCount)
                        {
                            if (null != webAsset)
                                webAsset.Unload();
                            swVersion.Close();
                            swVersion.Dispose();
                            UpdateFinish();
                        }
                    });
                }
            });
        }

        private void UpdateFinish()
        {
            ABManager.InitWithUpdate();
            Destroy(this.gameObject);
        }

        private void Update()
        {
            WebAssetManager.Update();
            DownloadManager.Update();
        }
    }
}
