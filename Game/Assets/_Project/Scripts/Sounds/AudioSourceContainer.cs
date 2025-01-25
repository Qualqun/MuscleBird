using UnityEngine;

public class AudioSourceContainer : MonoBehaviour
{
    public AudioSource audioSource;
    public float creationTime;
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
}
