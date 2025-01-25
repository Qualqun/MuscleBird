using UnityEngine;

public class LlamaBulletScript : MonoBehaviour
{
    [SerializeField]
    AudioClip spitSound;
    AudioSource audioSource;


    [SerializeField]
    private GameObject bullet;
    public bool canShoot = true;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && canShoot)
        {

            Transform head = transform.GetChild(0);
            Rigidbody2D llamaRigidBody = GetComponent<Rigidbody2D>();
            Vector2 unitForceShoot;

            int directionDegre = -45;
           // int powerForce = 10;

            for (int i = 0; i < 3; i++)
            {
                GameObject newBullet = Instantiate(bullet, head.position, Quaternion.identity, GameObject.Find("AllBullets").transform);
                Rigidbody2D bulletBody = newBullet.GetComponent<Rigidbody2D>();
                float dirRad = Mathf.Deg2Rad * (directionDegre + transform.rotation.eulerAngles.z);
                int speed = 10;

                bulletBody.linearVelocity = new Vector2(speed * Mathf.Cos(dirRad), speed * Mathf.Sin(dirRad)) * transform.localScale.x;
                directionDegre += 45;
            }

            llamaRigidBody.angularVelocity = 450 * transform.localScale.x;
            unitForceShoot.x = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.z);
            unitForceShoot.y = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z);

            //llamaRigidBody.AddForce(-unitForceShoot * powerForce, ForceMode2D.Impulse);
            canShoot = false;

            if (spitSound != null)
            {
                audioSource = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();
                audioSource.clip = spitSound;
                audioSource.Play();
            }

        }
    }
}
