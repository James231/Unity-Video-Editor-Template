using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class VideoEditorMenuItems
{
    [MenuItem("Video Editor/Links/Support Website")]
    public static void OpenSupportWebsite()
    {
        Application.OpenURL(VideoEditorConstants.SupportWebsiteLink);
    }

    [MenuItem("Video Editor/Links/GitHub Repo")]
    public static void OpenGitHubRepo()
    {
        Application.OpenURL(VideoEditorConstants.GitHubLink);
    }

    [MenuItem("Video Editor/Links/Documentation (Blog Post)")]
    public static void OpenBlog()
    {
        Application.OpenURL(VideoEditorConstants.BlogLink);
    }

    [MenuItem("Video Editor/Links/Video Tutorials (YouTube)")]
    public static void OpenYouTube()
    {
        Application.OpenURL(VideoEditorConstants.YouTubeLink);
    }

    [MenuItem("Video Editor/Utils/Set VE Window Layout")]
    public static void SetVEWindowLayout()
    {
        LayoutUtility.LoadLayout(VideoEditorConstants.WindowLayoutPath);
    }
}
