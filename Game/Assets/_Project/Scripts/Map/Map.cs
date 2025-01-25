using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FX
{
    public GameObject prefab;
    public Vector2 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
    public float minEmissionRange = 2f;
    public float maxEmissionRange = 2f;
}
[System.Serializable]
public class ParallaxElement
{
    public Sprite m_sprite;
    public Vector2 m_position;
    public bool m_loop;
    public bool m_flip;
    public bool m_disabled; // !Debug flag
    public bool m_defineLimits;
    [HideInInspector] public GameObject m_gameObject;
    [HideInInspector] public float m_spriteWidth;
    [HideInInspector] public GameObject m_gameObjectClone01;
    [HideInInspector] public GameObject m_gameObjectClone02;
    public bool cloudAnimation;
    [HideInInspector] public GameObject m_cloudClone01;
    [HideInInspector] public GameObject m_cloudClone02;
    public List<FX> fxList;
}

[System.Serializable]
public class Layer
{
    public string m_name;
    public float m_parallaxAmount;
    public List<ParallaxElement> m_elements;
}

[System.Serializable]
public class Parallax
{
    public List<Layer> m_layers;
}


public class Map : MonoBehaviour
{
    #region Fields
    private Vector3 m_initialCamPosition;
    private Vector3 m_previousCamPosition;

    private List<Player> m_players = new();

    [SerializeField]
    public Parallax m_parallax;

    public float cloudSpeed = 1.0f;
    #endregion

    #region Messages
    void Start()
    {
        m_initialCamPosition = Camera.main.transform.position;
        m_previousCamPosition = m_initialCamPosition;

        for (int layerIndex = 0; layerIndex < m_parallax.m_layers.Count; layerIndex++)
        {
            Layer layer = m_parallax.m_layers[layerIndex];

            for (int i = 0; i < layer.m_elements.Count; i++)
            {
                ParallaxElement element = layer.m_elements[i];
                // !Debug flag
                if (element.m_disabled) continue;

                element.m_gameObject = new GameObject(element.m_sprite.name);
                SceneManager.MoveGameObjectToScene(element.m_gameObject, this.gameObject.scene);
                element.m_gameObject.layer = 6;
                SpriteRenderer renderer = element.m_gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = element.m_sprite;
                if (element.m_defineLimits)
                {
                    //Bounds bounds = renderer.sprite.bounds;

                    //Vector2 spriteMin = renderer.transform.position + (Vector3)bounds.min;
                    //Vector2 spriteMax = renderer.transform.position + (Vector3)bounds.max;
                    //GameManager.Instance.mapLimits[0] = spriteMin;
                    //GameManager.Instance.mapLimits[1] = spriteMax;
                }
                element.m_gameObject.transform.position = element.m_position;
                if (element.m_flip)
                {
                    element.m_gameObject.transform.localScale = new Vector3(element.m_gameObject.transform.localScale.x * -1, element.m_gameObject.transform.localScale.y, element.m_gameObject.transform.localScale.z);
                }
                renderer.sortingOrder = (layerIndex * 100 + i + 10) * -1;

                if (element.m_loop)
                {
                    element.m_spriteWidth = renderer.bounds.size.x;

                    CreateLoopingClone(element, -element.m_spriteWidth, ref element.m_gameObjectClone01);
                    CreateLoopingClone(element, element.m_spriteWidth, ref element.m_gameObjectClone02);
                }
                if (element.cloudAnimation)
                {
                    element.m_spriteWidth = renderer.bounds.size.x;
                    CreateLoopingClone(element, -element.m_spriteWidth, ref element.m_cloudClone01);
                    CreateLoopingClone(element, element.m_spriteWidth, ref element.m_cloudClone02);
                }
                foreach (var fx in element.fxList)
                {
                    if (fx.prefab != null)
                    {
                        GameObject fxInstance = Instantiate(fx.prefab, element.m_gameObject.transform);
                        fxInstance.transform.localPosition = fx.position;
                        fxInstance.transform.localRotation = Quaternion.Euler(fx.rotation);
                        fxInstance.name = fx.prefab.name + "_FX";
                        var particleSystem = fxInstance.GetComponent<ParticleSystem>();
                        if (particleSystem != null)
                        {
                            var emission = particleSystem.emission;
                            emission.rateOverTime = Random.Range(fx.minEmissionRange, fx.maxEmissionRange);
                            var shape = particleSystem.shape;
                            shape.scale = new Vector3(shape.scale.x * fx.scale.x, shape.scale.y * fx.scale.y, shape.scale.z * fx.scale.z); // Apply the specified shape scale
                        }
                    }
                }
            }
        }

       // Debug.Log(GameManager.Instance.mapLimits[0]);
    }

    void CreateLoopingClone(ParallaxElement originalElement, float xOffset, ref GameObject instance)
    {
        instance = Instantiate(originalElement.m_gameObject);
        SceneManager.MoveGameObjectToScene(instance, this.gameObject.scene);
        instance.name = originalElement.m_sprite.name + "_Clone";
        instance.transform.position = originalElement.m_position + new Vector2(xOffset, 0);
        instance.layer = 6;
    }

    void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0f)
        {
            Camera.main.transform.position += new Vector3(horizontalAxis * Time.deltaTime * 6.0f, 0, 0);

            ApplyParallax();
        }
        UpdateClouds();
    }
    #endregion

    private void UpdateClouds()
    {
        foreach (var layer in m_parallax.m_layers)
        {
            foreach (var element in layer.m_elements)
            {
                if (element.cloudAnimation && element.m_gameObject != null && element.m_cloudClone01 != null && element.m_cloudClone02 != null)
                {
                    MoveCloud(element.m_gameObject, element);
                    MoveCloud(element.m_cloudClone01, element);
                    MoveCloud(element.m_cloudClone02, element);
                }
            }
        }
    }

    private void MoveCloud(GameObject cloud, ParallaxElement element)
    {
        cloud.transform.position += Vector3.left * cloudSpeed * Time.deltaTime;

        float cameraLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        float cameraRightEdge = Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);

        if (cloud.transform.position.x + element.m_spriteWidth / 2 < cameraLeftEdge)
        {
            cloud.transform.position = new Vector3(
                cameraRightEdge + element.m_spriteWidth / 2,
                cloud.transform.position.y,
                cloud.transform.position.z);
        }
    }


    #region Methods
    private void ApplyParallax()
    {
        Vector3 camOffset = Camera.main.transform.position - m_initialCamPosition;

        foreach (var layer in m_parallax.m_layers)
        {
            float parallaxEffect = layer.m_parallaxAmount;

            foreach (var element in layer.m_elements)
            {
                // !Debug flag
                if (element.m_disabled || element.cloudAnimation) continue;
                if (element.m_gameObject != null)
                {
                    Vector3 newPosition = new Vector3(
                        element.m_position.x + camOffset.x * parallaxEffect,
                        element.m_gameObject.transform.position.y,
                        element.m_gameObject.transform.position.z);

                    element.m_gameObject.transform.position = newPosition;
                }
                if (element.m_loop && element.m_gameObjectClone01 && element.m_gameObjectClone02)
                {
                    Vector3 newPosition1 = new Vector3(
                        element.m_position.x + element.m_spriteWidth + camOffset.x * parallaxEffect,
                        element.m_gameObjectClone01.transform.position.y,
                        element.m_gameObjectClone01.transform.position.z);

                    element.m_gameObjectClone01.transform.position = newPosition1;

                    Vector3 newPosition2 = new Vector3(
                        element.m_position.x - element.m_spriteWidth + camOffset.x * parallaxEffect,
                        element.m_gameObjectClone02.transform.position.y,
                        element.m_gameObjectClone02.transform.position.z);

                    element.m_gameObjectClone02.transform.position = newPosition2;
                }
            }
        }
    }

    public void ResetCameraPosition()
    {
        Camera.main.transform.position = m_initialCamPosition;
        m_previousCamPosition = m_initialCamPosition;

        foreach (var layer in m_parallax.m_layers)
        {
            foreach (var element in layer.m_elements)
            {
                // !Debug flag
                if (element.m_disabled) continue;
                if (element.m_gameObject != null)
                {
                    element.m_gameObject.transform.position = element.m_position;
                }
            }
        }
    }
    #endregion
}
