//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Runtime.CompilerServices;

[CustomEditor(typeof(CustomWebGL))]
public class CustomWebGLBuildSettings : Editor
{
    public override void OnInspectorGUI()
    {
        /*
        base.OnInspectorGUI();
        
        // PlayerSettings.SetPropertyString("emscriptenArgs", "-s ALLOW_MEMORY_GROWTH=1", BuildTargetGroup.WebGL);
        PlayerSettings.WebGL.emscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1";

        // BuildPipeline.BuildPlayer(levels, buildPath, target, options);        
        */

        CustomWebGL myScript = (CustomWebGL)target;
        
        myScript.Version = EditorGUILayout.TextField("Version", myScript.Version);        

        string path = "D:/business/Meshugina/Jobs/Teach SEG (Alan Koenig)/EQual-I/Builds/2020/May";        
        myScript.Path = EditorGUILayout.TextField("Build Path", path);
        string buildPath = myScript.Path + "/" + myScript.Version;

        GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MaxMem", GUILayout.Width(75));
            myScript.MaxMem = EditorGUILayout.IntField(myScript.MaxMem, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create Build Path"))
        {
            System.IO.Directory.CreateDirectory(buildPath);
        }

        if (GUILayout.Button("Build..."))
        {
            if(!System.IO.Directory.Exists(buildPath))
            {
                System.IO.Directory.CreateDirectory(buildPath);
                while (!System.IO.Directory.Exists(buildPath)) 
                    Debug.LogFormat("waiting for {0} to be created...");
            }
            MyBuild(buildPath, myScript.MaxMem);
        }
    }
    

    // [MenuItem("Build/Build WebGL")]
    public static void MyBuild(string buildPath, int maxMem)
    {
        var maxHeapSize = maxMem * 1024 * 1024;

        Debug.LogFormat("Building: {0}  maxHeapSize: {1}", buildPath, maxHeapSize);

        // PlayerSettings.WebGL.emscriptenArgs = string.Format("-s WASM_MEM_MAX={0} -s ALLOW_MEMORY_GROWTH=1", maxHeapSize);        
        PlayerSettings.WebGL.emscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        // buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.LogFormat("Build succeeded: {0} Gb ( {1} bytes )", summary.totalSize / 1024 / 1024, summary.totalSize);
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }        
    }
}
