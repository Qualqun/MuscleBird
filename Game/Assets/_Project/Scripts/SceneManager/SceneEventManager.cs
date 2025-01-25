using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEditor;
using Eflatun.SceneReference;
using SceneManagement;


public class SceneEventManager
{
    public event Action<string> OnSceneLoaded = delegate { };
    public event Action<string> OnSceneUnloaded = delegate { };
    public event Action OnSceneGroupLoaded = delegate { };

    public float loadDelay = 5.0f;
    public float LoadDelay
    {
        get => loadDelay;
        set
        {
            if ((value > 0) && (value < 10))
            {
                LoadDelay = value;
            }
        }
    }


    SceneGroup activeSceneGroup;

    public async Task LoadScenes(SceneGroup sceneGroup, IProgress<float> progress, bool reloadDupScenes = false)
    {
        activeSceneGroup = sceneGroup;
        List<string> loadedScene = new List<string>();

        await UnloadScenes();

        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            loadedScene.Add(SceneManager.GetSceneAt(i).name);
        }
        int totalScenesToLoad = activeSceneGroup.scenes.Count;

        AsyncOperationGroup operationGroup = new AsyncOperationGroup(totalScenesToLoad);
        for (int i = 0; i < totalScenesToLoad; i++)
        {
            var sceneData = sceneGroup.scenes[i];
            if (reloadDupScenes == false && loadedScene.Contains(sceneData.Name))
            {
                continue;
            }

            AsyncOperation newOperation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);


            await Task.Delay(TimeSpan.FromSeconds(loadDelay)); //Artifical delay for loading
            operationGroup.operations.Add(newOperation);
            OnSceneLoaded.Invoke(sceneData.Name);
        }

        while (!operationGroup.IsDone)
        {

            progress?.Report(operationGroup.Progress);
            await Task.Delay(100);
        }

        //Publish progress
        Scene activeScene = SceneManager.GetSceneByName(activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
        if (activeScene.IsValid() && activeScene.isLoaded)
        {
            SceneManager.SetActiveScene(activeScene);
        }
        OnSceneGroupLoaded.Invoke();
    }

    public async Task UnloadScenes()
    {
        List<string> scenesToUnload = new List<string>();
        string activeScene = SceneManager.GetActiveScene().name;

        //How many scenes are actually open
        int sceneCount = SceneManager.sceneCount;

        for (int i = sceneCount - 1; i > 0; i--)
        {
            var SceneAt = SceneManager.GetSceneAt(i);
            if (!SceneAt.isLoaded)
            {
                continue;
            }

            string sceneName = SceneAt.name;
            if (sceneName.Equals(activeScene) || sceneName == "PersistentSceneManager")
            {
                continue;
            }
            scenesToUnload.Add(sceneName);
        }

        AsyncOperationGroup operationGroup = new AsyncOperationGroup(scenesToUnload.Count);

        foreach (var scene in scenesToUnload)
        {
            var newOperation = SceneManager.UnloadSceneAsync(scene);
            if (newOperation == null)
            {
                continue;
            }
            operationGroup.operations.Add(newOperation);
            OnSceneUnloaded.Invoke(scene);
        }

        while (!operationGroup.IsDone)
        {
            await Task.Delay(100);
        }
    }

    //Most methods from unity SceneManager return async Operations(def : working on multiple threads)

    /// <summary>
    /// Collect all of the return types into a group to do actions and logic on
    /// </summary>
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> operations;


        /// <summary>
        /// CHeck the scene progression 
        /// </summary>
        public float Progress => operations.Count == 0 ? 0 : operations.Average(currentOperation => currentOperation.progress);

        /// <summary>
        /// Check the scene is loaded or not
        /// </summary>
        public bool IsDone => operations.All(currentOperation => currentOperation.isDone);


        /// <summary>
        /// Overload the constructor to the new capacity fron the readonly List
        /// </summary>
        /// <param name="startCapacity"></param>
        public AsyncOperationGroup(int startCapacity)
        {
            operations = new List<AsyncOperation>(startCapacity);
        }
    }

}