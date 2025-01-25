using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class PersistentSceneManager : PersistentSingleton<PersistentSceneManager>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static async void Init()
    {
        //Debug.Log("PersistentSceneManager loaded");
        await SceneManager.LoadSceneAsync("PersistentSceneManager", LoadSceneMode.Single);

#if UNITY_EDITOR
        // Set the bootstrapper scene to be the play mode start scene when running in the editor
        // This will cause the bootstrapper scene to be loaded first (and only once) when entering
        // play mode from the Unity Editor, regardless of which scene is currently active.
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
#endif
    }
}
