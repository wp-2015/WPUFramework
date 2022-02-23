using ABHelper;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WPUFramework
{
    public static class HotFixDllEntry
    {
        /// <summary>
        /// 主程序集
        /// </summary>
        public static Assembly Assembly { get; private set; }

        /// <summary>
        /// ILRuntime入口对象
        /// </summary>
        public static AppDomain AppDomain { get; private set; }


        private static ILRuntimeMethod start;
        private static ILRuntimeMethod update;
        private static ILRuntimeMethod fixedUpdate;
        private static ILRuntimeMethod lateUpdate;
        private static ILRuntimeMethod onFocus;
        private static ILRuntimeMethod onPause;
        private static ILRuntimeMethod onDestroy;

        public static void Init(System.Action<AppDomain> loadHotfixDelegate)
        {
            var fs = AssetManager.Load<TextAsset>("Assets/ABRes/HotfixDLL/Hotfix.dll");
            var p = AssetManager.Load<TextAsset>("Assets/ABRes/HotfixDLL/Hotfix.pdb");

            AppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            PreAddDelegate(AppDomain);
            loadHotfixDelegate?.Invoke(AppDomain);
            AppDomain.LoadAssembly(new MemoryStream(fs.bytes), new MemoryStream(p.bytes), new PdbReaderProvider());

#if !ILRuntime
            Debug.Log("运行的ILRuntime模式");
            AppDomain.LoadAssembly(new MemoryStream(fs.bytes), new MemoryStream(p.bytes), new PdbReaderProvider());
            start = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "Start", 0);
            update = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "Update", 2);
            fixedUpdate = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "FixedUpdate", 0);
            lateUpdate = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "LateUpdate", 0);
            onFocus = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "OnFocus", 1);
            onPause = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "OnPause", 1);
            onDestroy = new ILStaticMethod(AppDomain, "Hotfix.HotfixEntry", "OnDestroy", 0);
#else
            Debug.Log("没有运行的ILRuntime模式");
            Assembly = Assembly.Load(fs.bytes, p.bytes);
            System.Type hotfixInit = Assembly.GetType("Hotfix.HotfixEntry");

            start = new MonoStaticMethod(hotfixInit, "Start");
            update = new MonoStaticMethod(hotfixInit, "Update");
            fixedUpdate = new MonoStaticMethod(hotfixInit, "FixedUpdate");
            lateUpdate = new MonoStaticMethod(hotfixInit, "LateUpdate");
            onFocus = new MonoStaticMethod(hotfixInit, "OnFocus");
            onPause = new MonoStaticMethod(hotfixInit, "OnPause");
            onDestroy = new MonoStaticMethod(hotfixInit, "OnDestroy");
#endif
        }

        public static void Start()
        {
            start?.Invoke();
        }
        public static void Update()
        {
            update?.Invoke();
        }
        public static void FixedUpdate()
        {
            fixedUpdate?.Invoke();
        }
        public static void LateUpdate()
        {
            lateUpdate?.Invoke();
        }
        public static void OnFocus()
        {
            onFocus?.Invoke();
        }
        public static void OnPause()
        {
            onPause?.Invoke();
        }
        public static void OnDestroy()
        {
            onDestroy?.Invoke();
        }

        private static void PreAddDelegate(AppDomain appDomain)
        {
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterMethodDelegate<bool, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector2>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector3>();
        }
    }

}
