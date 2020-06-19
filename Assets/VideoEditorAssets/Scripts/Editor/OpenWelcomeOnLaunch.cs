using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Diagnostics;

[InitializeOnLoad]
[System.Serializable]
public static class OpenWelcomeOnLaunch
{
    [SerializeField]
    private static int count = 0;

    [SerializeField]
    private static bool launched = false;

    static OpenWelcomeOnLaunch()
    {
        VideoEditorWelcomeWindowData dataObj = AssetDatabase.LoadAssetAtPath<VideoEditorWelcomeWindowData>(VideoEditorConstants.WindowDataPath);
        if (dataObj == null)
        {
            dataObj = CreateDataObj(false);
        }

        if (!dataObj.launched)
        {
            launched = false;
            EditorApplication.update += Update;
        }
    }

    public static void Update()
    {
        if (!launched)
        {
            count++;
            if (count > 100)
            {
                VideoEditorWelcomeWindow.Init();
                CreateDataObj(true);
                launched = true;
            }
        }
    }

    public static VideoEditorWelcomeWindowData CreateDataObj(bool isLaunched)
    {
        VideoEditorWelcomeWindowData dataObj = ScriptableObject.CreateInstance<VideoEditorWelcomeWindowData>();
        dataObj.launched = isLaunched;
        AssetDatabase.CreateAsset(dataObj, VideoEditorConstants.WindowDataPath);
        AssetDatabase.SaveAssets();
        return dataObj;
    }
}
