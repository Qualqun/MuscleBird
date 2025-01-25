using UnityEngine;

public class OrthographicSpriteScaler : MonoBehaviour
{
    [Header("default : main Camera if empty")]
    [SerializeField] Camera targetCamera;

    [Header("desired object size")]
    [SerializeField] float targetSize = 100f;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        if (targetCamera == null) return;

        // Screen size in pixels
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Screen size in Unity unit (orthographique)
        float screenHeightInUnits = 2f * targetCamera.orthographicSize;
        float screenWidthInUnits = screenHeightInUnits * targetCamera.aspect;

        // Match size of Unity unit to targetSize
        float desiredWidthInUnits = (targetSize / screenWidth) * screenWidthInUnits;
        float desiredHeightInUnits = (targetSize / screenHeight) * screenHeightInUnits;

        // Rescale
        transform.localScale = new Vector3(desiredWidthInUnits, desiredHeightInUnits, transform.localScale.z);

        Vector3 size = GetComponent<BuildDrawer>().GetCutedSize();
        float maxSize = Mathf.Max(size.x, size.y);

        if (maxSize != 0f && targetSize != 0f)
        {
            ////Adjusts size based on build size
            float rescale = transform.localScale.x / maxSize * targetSize;
            transform.localScale = new Vector3(rescale, rescale, transform.localScale.z);
        }
        else
        {
            Debug.LogWarning("Division by zero", this);
        }
    }
}
