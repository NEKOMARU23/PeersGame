using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

public class BuildApp
{
    public static void Build()
    {
        // 1. ビルド対象のシーンを自動取得（空なら失敗させる）
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0) {
            throw new System.Exception("ビルド対象のシーンがEditor Build Settingsに登録されていません！");
        }

        // 2. 保存先フォルダを強制的に作成
        string outputPath = "Builds/App/CiTest.exe";
        System.IO.Directory.CreateDirectory("Builds/App");

        // 3. ビルド実行
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Debug.Log("--- Unity Build Started ---");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == BuildResult.Succeeded) {
            Debug.Log("--- Unity Build Succeeded! ---");
        } else {
            Debug.LogError("--- Unity Build Failed! ---");
            // 終了コード 1 で落として GitHub Actions に失敗を知らせる
            EditorApplication.Exit(1);
        }
    }
}