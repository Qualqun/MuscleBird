using System.Linq;
using UnityEngine;
using UnityEngine.Audio;



public class PoolManager : MonoBehaviour
{
    public AudioSourceContainer[] audioSourceContainers = new AudioSourceContainer[32];

    static public float gameVolume = 1;
    float pastVolume = 1;
    void Awake()
    {
        for (int i = 0; i < audioSourceContainers.Length; i++)
        {
            audioSourceContainers[i] = gameObject.AddComponent<AudioSourceContainer>();
            audioSourceContainers[i].creationTime = Time.time;
        }
    }

    public AudioSource GetFirstAvailableSource()
    {
        for (int i = 0; i < audioSourceContainers.Length; i++)
        {
            if (!audioSourceContainers[i].audioSource.isPlaying)
            {
                audioSourceContainers[i].creationTime = Time.time;
                return audioSourceContainers[i].audioSource;
            }
        }
        return null;
    }

    public AudioSource GetOldestAvailableSource(bool forcePlay = true)
    {
        //nous utilision linQ pour interragir avec une collection comme sur une base de donn�es SQL avec l'interaction Order By
        //celle ci nous permet d'ordonner une collection sur un param�tre donn�
        //ici dans l'ordre croissant sur la propri�t� CreationTime

        //Ici o est le nom de la variable similaire � un auto ou un foreach, qui est cr��e pour l'it�ration
        AudioSourceContainer[] sortedSourceContainers = audioSourceContainers.OrderBy(o => o.creationTime).ToArray();

        //On parcourt le tableau tri�
        for (int i = 0; i < sortedSourceContainers.Length; i++)
        {
            //si l'audio n'est pas utilis�e ou que l'on souhaite forcer l'usage du plus ancien
            if (!sortedSourceContainers[i].audioSource.isPlaying || forcePlay)
            {
                //Si on force l'usage du plus ancien et qu'il est occup� alors on le stoppe
                if (sortedSourceContainers[i].audioSource.isPlaying && forcePlay)
                {
                    sortedSourceContainers[i].audioSource.Stop();
                }
                //on renvoie l'audio source trouv�e
                return sortedSourceContainers[i].audioSource;
            }
        }

        return null;
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            gameVolume = gameVolume <= 1 ? gameVolume + 0.1f : 1;
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            gameVolume = gameVolume >= 0 ? gameVolume - 0.1f : 0;
        }

        if (gameVolume != pastVolume)
        {
            for (int i = 0; i < audioSourceContainers.Length; i++)
            {
                audioSourceContainers[i].audioSource.volume = gameVolume;
            }
        }
        pastVolume = gameVolume;
    }
}

