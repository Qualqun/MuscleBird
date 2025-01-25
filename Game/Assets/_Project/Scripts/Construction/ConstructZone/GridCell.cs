using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GridCell : MonoBehaviour
{
    #region Fields
    [SerializeField] float scaleMultiply;
    [SerializeField] float scalingDelay;

    BoxCollider2D boxCollider;
    Vector3 baseScale;
    #endregion

    #region Messages
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        baseScale = transform.localScale;
    }

    private void Update()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;

        if (bounds.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            //transform.DOScale(baseScale * scaleMultiply, scalingDelay);
        }
        else
        {
            //transform.DOScale(baseScale, scalingDelay);
        }
    }

    #endregion

    #region PrivateMethods

    #endregion

    #region PublicMethods

    #endregion

    #region Properties/Accessors

    #endregion
}
