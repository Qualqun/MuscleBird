using UnityEditor;
using UnityEngine;

namespace SceneManagement.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneLoader sceneLoader = (SceneLoader)target;

            if (EditorApplication.isPlaying && GUILayout.Button("Load First Scene Group"))
            {
                LoadSceneGroup(sceneLoader, 0);
            }

            if (EditorApplication.isPlaying && GUILayout.Button("Load Second Scene Group"))
            {
                LoadSceneGroup(sceneLoader, 2);
            }
            
            if (EditorApplication.isPlaying && GUILayout.Button("Load Third Scene Group"))
            {
                LoadSceneGroup(sceneLoader, 3);
            }
        }

        static async void LoadSceneGroup(SceneLoader sceneLoader, int index)
        {
            await sceneLoader.LoadSceneGroup(index);
        }
    }

#endif
}