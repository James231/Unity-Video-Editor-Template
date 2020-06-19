using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

public class VideoEditorWelcomeWindow : EditorWindow
{
    private Vector2 scrollPos;
    private int width = 1920;
    private int height = 1080;
    private static VideoEditorWelcomeWindow window;

    [MenuItem("Video Editor/Welcome Window")]
    public static void Init()
    {
        if (window != null)
        {
            window = (VideoEditorWelcomeWindow)GetWindow(typeof(VideoEditorWelcomeWindow));
            window.Focus();
            return;
        }

        window = (VideoEditorWelcomeWindow)GetWindow(typeof(VideoEditorWelcomeWindow));
        try
        {
            var main = GetEditorMainWindowPos();
            window.position = new Rect(main.x + (main.width / 2) - 300, main.y + (main.height / 2) - 300, 600, 500);
        }
        catch (Exception) {
            window.position = new Rect(100, 100, 600, 500);
        }
        window.titleContent = new GUIContent("Welcome");
        window.Show();
    }

    public void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        GUIStyle hStyle = GetHeadingStyle();
        GUIStyle pStyle = GetParagraphStyle();
        GUIStyle bStyle = GetButtonStyle();

        GUILayout.Label("Unity Video Editor Template", hStyle);
        GUILayout.Label("Thank you for downloading the Video Editor Template.", pStyle);

        GUILayout.Label("The template is centered around Timeline. So we recommend you use a specific Window Layout to reflect this. Press the button below to set the best Window Layout.", pStyle);
        if (GUILayout.Button("Set VE Window Layout", bStyle))
        {
            LayoutUtility.LoadLayout(VideoEditorConstants.WindowLayoutPath);
        }

        GUILayout.Label("To set the resolution of the output video, change the resolution in the 'Game' window. Or use the button below:", pStyle);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(60));
        GUILayout.Label("Width:");
        GUILayout.Label("Height:");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        width = EditorGUILayout.IntField(width);
        height = EditorGUILayout.IntField(height);
        GUILayout.EndVertical();
        if (GUILayout.Button("Set Resolution", SetResButtonStyle(), GUILayout.Height(42)))
        {
            GameViewUtils.AddSetSize(width, height);
        }
        GUILayout.EndHorizontal();


        GUILayout.Label("See the sample scene for example use. Specifically look at the 'GlobalTimeline' object. To export the video simply enter playmode and wait. You do not need to build anything! To see the video output settings look at the RecorderClip on in the Timeline of the 'GlobalTimeline' object.", pStyle);

        GUILayout.Label("Note: The template uses the 'Unity Recorder' package which is in alpha at the time of writing (June 2020). You may wish to update the package through PackageManager.", pStyle);

        GUILayout.Label("Also Note: The template includes the 'Default Playables' package from the Asset Store. But this has been heavily modified, so don't update/remove it.", pStyle);

        GUILayout.Label("Resources", hStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("YouTube Tutorials", bStyle))
        {
            Application.OpenURL(VideoEditorConstants.WindowLayoutPath);
        }
        if (GUILayout.Button("Blog Post (written documentation)", bStyle))
        {
            Application.OpenURL(VideoEditorConstants.BlogLink);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Need More Support?", hStyle);
        GUILayout.Label("If you need more support consider opening an issue on the GitHub repository. You can also contact us by completing a contact form on our website.", pStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GitHub Repo", bStyle))
        {
            Application.OpenURL(VideoEditorConstants.GitHubLink);
        }
        if (GUILayout.Button("Support Website", bStyle))
        {
            Application.OpenURL(VideoEditorConstants.SupportWebsiteLink);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Support Us", hStyle);
        GUILayout.Label("If you found this useful and would like to support us, consider purchasing our other Unity Assets from the Asset Store:", pStyle);
        if (GUILayout.Button("See our Unity Assets", bStyle))
        {
            Application.OpenURL(VideoEditorConstants.SolutionStudiosLink);
        }

        GUILayout.Label("Contributions", hStyle);
        GUILayout.Label("All contributions are welcome. Please open a Pull Request on GitHub. Thank you.", pStyle);
        GUILayout.EndScrollView();
    }

    private GUIStyle GetButtonStyle()
    {
        GUIStyle bStyle = GUI.skin.button;
        bStyle.margin = new RectOffset(10, 10, 10, 10);
        bStyle.padding = new RectOffset(7, 7, 7, 7);
        bStyle.fontSize = 14;
        return bStyle;
    }

    private GUIStyle SetResButtonStyle()
    {
        GUIStyle bStyle = GUI.skin.button;
        bStyle.margin = new RectOffset(8, 0, 0, 0);
        bStyle.fontSize = 14;
        return bStyle;
    }

    private GUIStyle GetHeadingStyle()
    {
        GUIStyle headingStyle = new GUIStyle();
        headingStyle.fontSize = 20;
        headingStyle.padding = new RectOffset(10, 10, 10, 10);
        return headingStyle;
    }

    private GUIStyle GetParagraphStyle()
    {
        GUIStyle paraStyle = new GUIStyle();
        paraStyle.fontSize = 14;
        paraStyle.padding = new RectOffset(10, 10, 10, 10);
        paraStyle.wordWrap = true;
        return paraStyle;
    }

    private static Rect GetEditorMainWindowPos()
    {
        var containerWinType = GetAllDerivedTypes(System.AppDomain.CurrentDomain, typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
        if (containerWinType == null)
            throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
        var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (showModeField == null || positionProperty == null)
            throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
        var windows = Resources.FindObjectsOfTypeAll(containerWinType);
        foreach (var win in windows)
        {
            var showmode = (int)showModeField.GetValue(win);
            if (showmode == 4) // main window
            {
                var pos = (Rect)positionProperty.GetValue(win, null);
                return pos;
            }
        }
        throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
    }

    private static System.Type[] GetAllDerivedTypes(System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        var assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);
            }
        }
        return result.ToArray();
    }
}
