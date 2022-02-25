using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace WPUFramework
{
    public class HotfixRegister
    {
        public static void RegisterAdaptor(AppDomain appDomain)
        {
            appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        }

        public static void InitILRuntime(AppDomain appDomain)
        {
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterMethodDelegate<bool, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector2>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector3>();
        }
    }
}
