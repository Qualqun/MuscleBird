using UnityEngine;

public class DestroyOnVoid : MonoBehaviour
{

    private void Start()
    {
        CannonManager.Instance.destroyAllLaunchedBullets += DestroyBullet;
    }

    private void OnDestroy()
    {
        CannonManager.Instance.destroyAllLaunchedBullets -= DestroyBullet;

    }



    private void Update()
    {
        if (transform.position.y < -10)
        {
            DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
