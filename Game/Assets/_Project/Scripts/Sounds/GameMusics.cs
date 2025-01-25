using UnityEngine;

public class GameMusics : Singleton<GameMusics>
{

    [SerializeField]
    AudioClip GameMusic;
    AudioSource audioSource;
    AudioSource audioSourceForDestroyableObjects;
    AudioSource audioSourceCollide;

    [SerializeField]
    AudioClip[] woodCollision;

    [SerializeField]
    AudioClip[] woodDestroy;

    [SerializeField]
    AudioClip[] rockCollision;

    [SerializeField]
    AudioClip[] rockDestroy;

    [SerializeField]
    AudioClip[] glassDestroy;

    
   

    void Start()
    {
        audioSource = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();

        if (audioSource != null)
        {
            audioSource.clip = GameMusic;
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


    public void CollideObjectSounds(BuildMaterialData.Material _typeMaterial)
    {
        switch (_typeMaterial)
        {
            case BuildMaterialData.Material.WOOD:
                audioSourceCollide = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();
                audioSourceCollide.clip = woodCollision[Random.Range(0, woodCollision.Length)];
                audioSourceCollide.Play();
                audioSourceCollide.loop = false;
                break;

            case BuildMaterialData.Material.STONE:
                audioSourceCollide = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();
                audioSourceCollide.clip = rockCollision[Random.Range(0, rockCollision.Length)];
                audioSourceCollide.Play();
                audioSourceCollide.loop = false;
                break;

            default:

                break;
        }
    }
    public void DestroyObjectSounds(BuildMaterialData.Material _typeMaterial)
    {
        audioSourceForDestroyableObjects = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();

        switch (_typeMaterial)
        {
            case BuildMaterialData.Material.WOOD:

                audioSourceForDestroyableObjects.clip = woodDestroy[Random.Range(0, woodDestroy.Length)];
                audioSourceForDestroyableObjects.Play();
                audioSourceForDestroyableObjects.loop = false;

                break;
            case BuildMaterialData.Material.STONE:
                audioSourceForDestroyableObjects.clip = rockDestroy[Random.Range(0, rockDestroy.Length)];
                audioSourceForDestroyableObjects.Play();
                audioSourceForDestroyableObjects.loop = false;
                break;
            case BuildMaterialData.Material.GLASS:
                audioSourceForDestroyableObjects.clip = glassDestroy[Random.Range(0, glassDestroy.Length)];
                audioSourceForDestroyableObjects.Play();
                audioSourceForDestroyableObjects.loop = false;
                break;
            default:
                break;
        }

    }

}
