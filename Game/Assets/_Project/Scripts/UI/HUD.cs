using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pauseLayout;
    [SerializeField]
    private GameObject m_pauseButton;

    #region Messages
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_pauseButton.SetActive(true);
        m_pauseLayout.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    public void PauseClick()
    {
        m_pauseButton.SetActive(!m_pauseButton.activeSelf);
        m_pauseLayout.SetActive(!m_pauseLayout.activeSelf);
    }

    public void Leave()
    {
        LoadingScreen.Instance.SwapToScene("MainMenu", 5.0f);
    }
}
