using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class NametagBanner : MonoBehaviour
{

    #region Fields
    private Vector3 m_beginposition;
    private const float m_verticalMovement = 400.0f;
    private Coroutine m_animationCoroutine;

    #endregion


    #region Methods
    public void TurnOn()
    {
        m_beginposition = transform.position;
        m_animationCoroutine = StartCoroutine(StartAnimation());
    }
    public void TurnOff()
    {
        if (m_animationCoroutine != null)
        {
            StopCoroutine(m_animationCoroutine);
            m_animationCoroutine = null;
            transform.position = m_beginposition;
        }
    }

    private IEnumerator StartAnimation()
    {
        yield return Animate(1.0f, m_beginposition, (Vector2)m_beginposition + new Vector2(.0f, -m_verticalMovement));
        yield return new WaitForSeconds(2.0f);
        yield return Animate(1.0f, transform.position, (Vector2)transform.position + new Vector2(.0f, +m_verticalMovement));
    }
    private IEnumerator Animate(float duration, Vector2 startPosition, Vector2 endPosition)
    {

        float elapsedTime = .0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            t = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector2.Lerp(startPosition, endPosition, t); 
            yield return null;
        }
    }
    #endregion

}
