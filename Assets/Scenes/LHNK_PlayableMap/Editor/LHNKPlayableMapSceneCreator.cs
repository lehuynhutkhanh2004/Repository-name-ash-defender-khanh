#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LHNKPlayableMapSceneCreator
{
    [MenuItem("Tools/LHNK/Create Playable LHNK Map Scene")]
    public static void CreatePlayableScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject manager = new GameObject("LHNK_Map_GameManager");
        manager.AddComponent<LHNKPlayableMapGame>();

        string path = "Assets/Scenes/LHNK-Map-Playable.unity";
        EditorSceneManager.SaveScene(scene, path);
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        Debug.Log("Created playable LHNK scene at: " + path);
    }
}
#endif
