using ABHelper;
using ILRuntime.Runtime.Enviorment;
using Netwrok;
using UnityEngine;

namespace WPUFramework
{
    public class WPUFrameworkEntry : MonoBehaviour
    {
        private System.Action<AppDomain> loadHotfixDelegate;
        public void Entry(System.Action<AppDomain> loadHotfixDelegate)
        {
            this.loadHotfixDelegate = loadHotfixDelegate;


        }

        private void Awake()
        {
            UpdateAsset("");
            ConnectSocket();
            HotFixDllEntry.Init(loadHotfixDelegate);
        }

        private void Start()
        {
            HotFixDllEntry.Start();
        }

        public void Update()
        {
            NetworkManager.Update();
            HotFixDllEntry.Update();
        }

        private void FixedUpdate()
        {
            HotFixDllEntry.FixedUpdate();
        }

        private void LateUpdate()
        {
            HotFixDllEntry.LateUpdate();
        }

        //������Դ
        public static void UpdateAsset(string ip, System.Action<float> processChangeCB = null)
        {
            ABUpdate.Start("10.21.249.136", processChangeCB);
        }

        //����Socket
        public static void ConnectSocket()
        {
            NetworkManager.Connect("Main", "127.0.0.1", 8999, false, ProxyInit);
        }

        public static void ProxyInit()
        {

        }
    }
}