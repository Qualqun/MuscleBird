using System.Collections;
using UnityEngine;

public class BuildScore : MonoBehaviour
{
    float appearTime = 1f;
    private void Start()
    {
        StartCoroutine(AppearTime());
    }

    IEnumerator AppearTime()
    {
        yield return new WaitForSeconds(appearTime);
        Destroy(gameObject);
    }
}
