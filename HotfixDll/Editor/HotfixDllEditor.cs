using System.IO;
using UnityEditor;
using UnityEngine;
using WPUFramework;

public class HotfixDllEditor
{
    private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
    private const string CodeDir = "Assets/ABRes/HotfixDLL/";
    private const string HotfixDll = "Hotfix.dll";
    private const string HotfixPdb = "Hotfix.pdb";

    public static void BuildHotfixDLL()
    {
        CheckDir(CodeDir);
        AssetDatabase.Refresh(); //构建新的DLL
        File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
        File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
        Debug.Log("复制Hotfix.dll, Hotfix.pdb到Asset/Res/HotfixDLL完成");
        AssetDatabase.Refresh(); //构建新的DLL
    }

    private static string CheckDir(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    static void AllScriptsReloaded()
    {
        BuildHotfixDLL();
    }

    [MenuItem("Tools/ILRuntime/Generate ILRuntime CLR Binding Code by Analysis")]
    public static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream("Assets/ABRes/HotfixDLL/Hotfix.dll.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
            //Crossbind Adapter is needed to generate the correct binding code
            HotfixRegister.RegisterAdaptor(domain);
            //ILRuntimeHelper.RegisterAdaptor(domain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Scripts/GameMain/ILRuntime/Generated");
            AssetDatabase.Refresh();
            Debug.Log("CLRBinding生成完成");
        }
    }
}
