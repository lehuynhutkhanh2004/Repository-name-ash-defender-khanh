#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NKPlayableMapSceneCreator
{
    [MenuItem("Tools/Ash Defender/Create Playable NK Map Scene")]
    public static void CreatePlayableScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject manager = new GameObject("Map03_GameManager");
        manager.AddComponent<NKPlayableMapGame>();

        string path = "Assets/Scenes/NK-Map-Playable.unity";
        EditorSceneManager.SaveScene(scene, path);
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        Debug.Log("Created playable scene at: " + path);
    }
}
#endif
