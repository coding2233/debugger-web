using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenFolderEditor
{

    //打开读写文件夹
    [MenuItem("Tools/OpenFolder/PersistentDataPath")]
    private static void OpenPersistentDataPath()
    {
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }

    //打开只读文件夹
    [MenuItem("Tools/OpenFolder/StreamingAssetsPath")]
    private static void OpenStreamingAssetsPath()
    {
        EditorUtility.OpenWithDefaultApp(Application.streamingAssetsPath);
    }

    //打开工程文件夹
    [MenuItem("Tools/OpenFolder/DataPath")]
    private static void OpenDataPath()
    {
        EditorUtility.OpenWithDefaultApp(Application.dataPath);
    }

    //打开缓存文件夹
    [MenuItem("Tools/OpenFolder/TemporaryCachePath")]
    private static void OpenTemporaryCachePath()
    {
        EditorUtility.OpenWithDefaultApp(Application.temporaryCachePath);
    }
}
