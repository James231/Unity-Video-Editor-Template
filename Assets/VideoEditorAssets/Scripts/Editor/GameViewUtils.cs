using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GameViewUtils
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;

    static GameViewUtils()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    public static void AddSetSize(int width, int height)
    {
        int idx = FindSize(GameViewSizeGroupType.Standalone, width, height);
        if (idx == -1)
        {
            int keyIndex = 2;
            string key = "Video Output";
            if (SizeExists(GameViewSizeGroupType.Standalone, key))
            {
                while (SizeExists(GameViewSizeGroupType.Standalone, key + " " + keyIndex))
                {
                    keyIndex++;
                }
                key += " " + keyIndex;
            }
            AddCustomSize(GameViewSizeType.AspectRatio, GameViewSizeGroupType.Standalone, width, height, key);
            idx = FindSize(GameViewSizeGroupType.Standalone, key);
        }

        SetSize(idx);
    }

    public enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
#if NET_4_6
        var ctor = gvsType.GetConstructor(new Type[] { typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType"), typeof(int), typeof(int), typeof(string) });
        var newGvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        if (viewSizeType == GameViewSizeType.AspectRatio)
        {
            newGvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType.AspectRatio");
        }
        else
        {
            newGvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType.FixedResolution");
        }

        var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
#else
        var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
#endif
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
    {
        return FindSize(sizeGroupType, text) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
    {
        var group = GetGroup(sizeGroupType);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        for (int i = 0; i < displayTexts.Length; i++)
        {
            string display = displayTexts[i];
            int pren = display.IndexOf('(');
            if (pren != -1)
                display = display.Substring(0, pren - 1);
            if (display == text)
                return i;
        }
        return -1;
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }
}