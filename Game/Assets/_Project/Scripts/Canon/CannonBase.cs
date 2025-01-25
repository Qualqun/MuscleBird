using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CannonBase : MonoBehaviour
{
    [System.Serializable]
    protected struct Settings
    {
        public float minLaunchLimit;
        public float maxLaunchLimit;
        public float maxSpeed;
        public Vector3 defaultScale;
        public Quaternion defaultRotation;
    }

    [System.Serializable]
    protected struct BulletInfo
    {
        public bool lauchBullet;
        public Vector2 vecUnit;
        public float magnitude;
        public float actuelSpeed;
        public int rotationBulletSpeed;

    }

    [SerializeField]
    AudioClip lauchedClip;
    AudioSource audioSource;

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    protected Transform strechPowerTransform;

    [SerializeField]
    SpriteRenderer indicatorCurve;

    SpriteRenderer[] curvePoint = new SpriteRenderer[8];
    float timerCurve = 0;

    [SerializeField]
    protected Settings settings;

    protected BulletInfo bulletInfo;
    CircleCollider2D selectZone;
    bool isSelected = false;

    public bool Selected => isSelected;

    public abstract void VisualComportement();

    public virtual void Start()
    {

        //Init base settings
        settings.minLaunchLimit = 1f;
        settings.maxLaunchLimit = 4f;
        settings.maxSpeed = 15f;
        settings.defaultScale = strechPowerTransform.localScale;
        settings.defaultRotation = Quaternion.identity;


        //Init bullet info
        bulletInfo.lauchBullet = false;
        bulletInfo.vecUnit = Vector3.zero;
        bulletInfo.magnitude = 0;
        bulletInfo.actuelSpeed = 0;
        bulletInfo.rotationBulletSpeed = 450;

        //Init curve point 
        for (int i = 0; i < curvePoint.Length; i++)
        {
            curvePoint[i] = Instantiate(indicatorCurve, transform);
        }

        ResetCurve();
        ResetCannon();

        selectZone = gameObject.GetComponent<CircleCollider2D>();

    }

    bool yolo;
    private void FixedUpdate()
    {
        if (!Input.GetMouseButton(0)) yolo = true;
        if (!yolo) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        LaunchBullet();

        if (Input.GetMouseButton(0) && Physics2D.Raycast(mousePosition, Vector2.zero, 0f, LayerMask.GetMask("Canon")) && !CannonManager.Instance.cannonPause)
        {
            Vector2 vecMouseCircle = (Vector2)transform.position - mousePosition;
            //float cannonRotation = Mathf.Rad2Deg * Mathf.Atan2(vecMouseCircle.y, vecMouseCircle.x);
            isSelected = true;
            TouchManager.Instance.canSlide = false;

            if (vecMouseCircle.magnitude < settings.minLaunchLimit)
            {
                bulletInfo.lauchBullet = false;
                ResetCurve();
                ResetCannon();
                isSelected = false;
            }
            else
            {

                float maxLenght = settings.maxLaunchLimit - settings.minLaunchLimit;

                bulletInfo.vecUnit = vecMouseCircle.normalized;
                bulletInfo.magnitude = vecMouseCircle.magnitude > settings.maxLaunchLimit ? settings.maxLaunchLimit : vecMouseCircle.magnitude;

                VisualComportement();

                // We do a cross product to know the actual speed 
                bulletInfo.actuelSpeed = settings.maxSpeed * (bulletInfo.magnitude - settings.minLaunchLimit) / maxLenght;
                bulletInfo.lauchBullet = true;

                //Need to be on the last
                ActiveCurve();
            }

        }
        else
        {
            bulletInfo.lauchBullet = false;
            ResetCurve();
            ResetCannon();
            //TouchManager.Instance.canSlide = true;
        }
    }


    private void ResetCurve()
    {
        for (int i = 0; i < curvePoint.Length; i++)
        {
            curvePoint[i].transform.localScale = Vector3.zero;
        }
    }

    private void ActiveCurve()
    {
        float timeMaxDistance = 0.2f * curvePoint.Length;

        timerCurve = timerCurve > timeMaxDistance ? 0 : timerCurve + Time.deltaTime / 4;

        for (int i = 0; i < curvePoint.Length; i++)
        {
            float time = timerCurve + 0.2f * i;
            float timeCurve = time > timeMaxDistance ? time - timeMaxDistance : time;

            curvePoint[i].transform.position = SimulateProjectileCurve(timeCurve, 10, bulletInfo.vecUnit * bulletInfo.actuelSpeed);
            curvePoint[i].transform.localScale = Vector3.one - new Vector3(timeCurve, timeCurve, timeCurve) / 2.5f;
        }
    }

    private Vector2 SimulateProjectileCurve(float _timeSimulate, int _nbSteps, Vector2 _baseVelocity)
    {
        //Mathematical formula that simulates the curve of the projectile, more you have  better the precision is / performance required 
        Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();

        Vector2 acceleration = Physics.gravity * bulletBody.gravityScale;
        Vector2 oldVelocity = _baseVelocity;
        Vector2 newPos = transform.position;

        float dtSimulate = _timeSimulate / _nbSteps;

        for (int i = 0; i < _nbSteps; ++i)
        {
            newPos += oldVelocity * dtSimulate + acceleration * dtSimulate * dtSimulate / 2;
            oldVelocity += acceleration * dtSimulate;
        }

        return newPos;
    }

    private void ResetCannon()
    {
        strechPowerTransform.localScale = settings.defaultScale;
        transform.rotation = settings.defaultRotation;
    }


    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, settings.minLaunchLimit);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, settings.maxLaunchLimit - settings.minLaunchLimit);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.25f);
        }
    }


    private void OnDestroy()
    {
        for (int i = 0; i < curvePoint.Length; i++)
        {
            if (curvePoint[i] != null)
            {
                Destroy(curvePoint[i].gameObject);
            }
        }
    }

    public void LaunchBullet()
    {

        if (!Input.GetMouseButton(0) && bulletInfo.lauchBullet && !CannonManager.Instance.cannonPause)
        {
            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation, GameObject.Find("AllBullets").transform);
            Rigidbody2D bulletBody = newBullet.GetComponent<Rigidbody2D>();
            CannonManager.Instance.HadShot();


            newBullet.transform.localScale = transform.localScale;
            bulletBody.angularVelocity = -bulletInfo.rotationBulletSpeed * transform.localScale.x;
            bulletBody.linearVelocity = bulletInfo.vecUnit * bulletInfo.actuelSpeed;
            

            SoundLaunch();
            Destroy(gameObject);

        }
    }

    private void SoundLaunch()
    {
        if (lauchedClip != null)
        {
            audioSource = FindAnyObjectByType<PoolManager>().GetFirstAvailableSource();
            audioSource.clip = lauchedClip;
            audioSource.Play();
        }
    }

}