using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;

public class LoadingScreen : MonoBehaviour
{
    #region Singleton
    #endregion

    #region Public Methods

    private static LoadingScreen _instance;

    public static LoadingScreen Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loadingScreenObject = new GameObject("LoadingScreen");
                _instance = loadingScreenObject.AddComponent<LoadingScreen>();

                DontDestroyOnLoad(loadingScreenObject);
            }
            return _instance;
        }
    }

    private GameObject loadingScreenInstance;

    private IEnumerator Watcher(AsyncOperation asyncLoad, float minDuration)
    {
        float elapsedTime = .0f;
        while (asyncLoad.progress < 0.9f || elapsedTime < minDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (loadingScreenInstance != null)
        {
            Destroy(loadingScreenInstance);
            loadingScreenInstance = null;
        }
        asyncLoad.allowSceneActivation = true;
    }

    public void SwapToScene(string sceneName, float minDuration = .0f)
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootGameObjects)
        {
            Canvas canvas = rootObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                rootObject.SetActive(false);
            }
        }

        GameObject prefab = Resources.Load<GameObject>("LoadingScreen");
        if (prefab == null)
        {
            Debug.Log("LoadingScreen prefab isn't found");
            return;
        }
        loadingScreenInstance = Instantiate(prefab);
        DontDestroyOnLoad(loadingScreenInstance);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        StartCoroutine(Watcher(asyncLoad, minDuration));
    }
    #endregion
}
