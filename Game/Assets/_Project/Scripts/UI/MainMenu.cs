using SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_buttonsLayout;
    [SerializeField]
    private GameObject m_pauseLayout;
    [SerializeField]
    private GameObject m_splash;
    [SerializeField] private GameObject m_soundOn;
    [SerializeField] private GameObject m_soundOff;
    [SerializeField] private GameObject m_musicOn;
    [SerializeField] private GameObject m_musicOff;
    [SerializeField] private GameObject m_credits;

    [SerializeField] private Image mainTitle;
    [SerializeField] private float titleAnimationSpeed = 2f;
    [SerializeField] private float titleMovementAmount = 0.5f;
    private Vector3 titleInitialPosition;
    [SerializeField]
    private GameObject m_playText;
    public float m_scaleSpeed = 0.4f;
    public float m_scaleAmount = 1.2f;
    private Vector3 m_originalScale;
    private float m_scaleDirection = 1f;

    void Start()
    {
        m_originalScale = m_playText.transform.localScale;
        titleInitialPosition = mainTitle.rectTransform.localPosition;
        m_buttonsLayout.SetActive(false);
        mainTitle.gameObject.SetActive(false);
        m_pauseLayout.SetActive(false);
        m_credits.SetActive(false);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void Update()
    {
        BounceTitle();
        if (!m_splash.activeSelf) { return; }
        TextAnimation();
    }

    public async void Play()
    {
        await SceneLoader.Instance.LoadSceneGroup(1);
    }
    public void Options()
    {
        m_pauseLayout.SetActive(!m_pauseLayout.activeSelf);
        m_buttonsLayout.SetActive(!m_buttonsLayout.activeSelf);
    }
    public void SoundToggle()
    {
        if (PoolManager.gameVolume == 1.0f)
        {
            m_soundOn.SetActive(false);
            m_soundOff.SetActive(true);
            PoolManager.gameVolume = 0.0f;
        }
        else
        {
            m_soundOn.SetActive(true);
            m_soundOff.SetActive(false);
            PoolManager.gameVolume = 1.0f;
        }
    }
    public void MusicToggle()
    {
        if (PoolManager.gameVolume == 1.0f)
        {
            m_musicOn.SetActive(false);
            m_musicOff.SetActive(true);
            PoolManager.gameVolume = 0.0f;
        }
        else
        {
            m_musicOn.SetActive(true);
            m_musicOff.SetActive(false);
            PoolManager.gameVolume = 1.0f;
        }
    }
    public void Credits()
    {
        m_credits.SetActive(!m_credits.activeSelf);
        bool state = m_credits.activeSelf;
        m_buttonsLayout.SetActive(!state);
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void Splash()
    {
        m_buttonsLayout.SetActive(true);
        m_splash.SetActive(false);
        mainTitle.gameObject.SetActive(true);
    }

    private void TextAnimation()
    {
        float scaleChange = m_scaleSpeed * Time.deltaTime * m_scaleDirection;
        m_playText.transform.localScale += new Vector3(scaleChange, scaleChange, scaleChange);

        if (m_playText.transform.localScale.x >= m_originalScale.x * m_scaleAmount)
        {
            m_playText.transform.localScale = m_originalScale * m_scaleAmount;
            m_scaleDirection = -1f;
        }
        else if (m_playText.transform.localScale.x <= m_originalScale.x)
        {
            m_playText.transform.localScale = m_originalScale;
            m_scaleDirection = 1f;
        }
    }

    private void BounceTitle()
    {
        float newY = Mathf.Sin(Time.time * titleAnimationSpeed) * titleMovementAmount;
        Vector3 position = titleInitialPosition;
        position.y += newY;
        mainTitle.rectTransform.localPosition = position;
    }
}
