using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace SceneManagement
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroups;
        [SerializeField, Range(0,10)] float loadDelay;


        float targetProgress;
        bool isLoading;

        public readonly SceneEventManager manager = new SceneEventManager();

        protected override void Awake()
        {
            base.Awake();
            // TODO can remove
            //manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            //manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            //manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");

            manager.loadDelay = loadDelay;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        async void Start()
        {
            await LoadSceneGroup(0, true, false);
        }

        void Update()
        {
            if (!isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }


        public async Task LoadSceneGroup(int index, bool bypassDelay = false, bool showLoadingScreen = true)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            if (showLoadingScreen)
            {
                EnableLoadingCanvas();
            }

            if (bypassDelay)
            {
                float originalDelay = manager.loadDelay;
                manager.loadDelay = 0;
                await manager.LoadScenes(sceneGroups[index], progress);
                manager.loadDelay = originalDelay;
                EnableLoadingCanvas(false);
            }
            else
            {
                await manager.LoadScenes(sceneGroups[index], progress);
            }

            if (showLoadingScreen)
            {
                EnableLoadingCanvas(false);
            }

            if (index == 1)
            {
                GameManager.Instance.readyToPlay = true;
            }
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}
