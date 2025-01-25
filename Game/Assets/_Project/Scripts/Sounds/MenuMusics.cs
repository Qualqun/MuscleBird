using UnityEngine;
using UnityEngine.Audio;

public class MenuMusics : MonoBehaviour
{
    [SerializeField]
    AudioClip MenuMusic;
    AudioSource audioSource;

    void Start()
    {
        audioSource = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();

        if (audioSource != null)
        {
            audioSource.clip = MenuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

}
