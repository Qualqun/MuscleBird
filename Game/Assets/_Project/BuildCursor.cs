using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildCursor : LazySingleton<BuildCursor>
{
    public interface Behavior
    {
        public void OnMouseButtonDown();
        public void OnMouseButtonUp();
        public void OnBeginDrag();
        public void OnDrag(Vector3 dragPos);
        public void OnEndDrag();
    }

    enum State
    {
        Null,
        Down, WaitDragDetection, Up,
        BeginDrag, OnDrag, EndDrag
    }

    [SerializeField] Vector2 dragOffset;
    [SerializeField] float dragActivTime = 0.3f;
    [SerializeField] Image dragWheel;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] NewBuildSelecterPanel buildSelecter;

    State state;
    Behavior buildSelected;
    Coroutine dragDetectCoroutine;
    bool stopDragDetect;
    float startTime;
    //NewBuildSlot buildSlotSelected;

    private void Update()
    {
        UpdateState();
        UpdateDragWheel();

        if (GameManager.Instance.constructionDone)
        {
            gameObject.SetActive(false);
        }
    }

    void UpdateState()
    {
        bool loopState = false;
        do
        {
            loopState = false;
            switch (state)
            {
                case State.Null:
                    if (Input.GetMouseButtonDown(0))
                    {
                        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        buildSelected = buildSelecter.GetBuildSlotPressed();

                        if (buildSelected != null)
                        {
                            state = State.Down;
                            loopState = true;
                        }
                        else if (PickBuild())
                        {
                            state = State.Down;
                            loopState = true;
                        }
                    }
                    break;

                case State.Down:

                    buildSelected?.OnMouseButtonDown();
                    dragDetectCoroutine = StartCoroutine(DragDetect());
                    state = State.WaitDragDetection;
                    loopState = true;
                    break;

                case State.WaitDragDetection:

                    if (Input.GetMouseButtonUp(0))
                    {
                        state = State.Up;
                        loopState = true;
                    }
                    break;

                case State.Up:

                    buildSelected?.OnMouseButtonUp();
                    StopDragDetect();
                    state = State.Null;

                    break;

                case State.BeginDrag:
                    buildSelected?.OnBeginDrag();
                    state = State.OnDrag;
                    loopState = true;
                    break;

                case State.OnDrag:

                    transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    buildSelected?.OnDrag(GetDragPos());
                    if (Input.GetMouseButtonUp(0))
                    {
                        state = State.EndDrag;
                        loopState = true;
                    }

                    break;

                case State.EndDrag:

                    buildSelected?.OnEndDrag();
                    buildSelected = null;
                    StopDragDetect();
                    state = State.Null;

                    break;

                default:
                    break;
            }
        }
        while (loopState);
    }

    float MapValue(float val, float oMin, float oMax, float nMin = 0f, float nMax = 1f)
    {
        float normalised = Mathf.InverseLerp(oMin, oMax, val);
        return Mathf.Lerp(nMin, nMax, normalised);
    }

    void UpdateDragWheel()
    {
        if (!dragWheel.gameObject.activeSelf) return;

        float timeElapsed = Time.realtimeSinceStartup - startTime;
        dragWheel.fillAmount = MapValue(timeElapsed, 0f, dragActivTime);
        dragWheel.transform.parent.position = Camera.main.WorldToScreenPoint(GetDragPos());
    }

    bool PickBuild()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 10f, targetLayer);

        if (hit.collider != null)
        {
            Build buildHited = hit.collider.GetComponent<Build>();
            buildSelected = buildHited;
            return true;
        }
        return false;
    }

    public void PickBuildOnUI(Build buildOnUI)
    {
        buildSelected = buildOnUI;
        state = State.Down;
    }

    Vector3 GetDragPos()
    {
        Vector3 pos = transform.position;
        pos.x += dragOffset.x;
        pos.y += dragOffset.y;
        return pos;
    }

    IEnumerator DragDetect()
    {
        stopDragDetect = false;
        dragWheel.transform.parent.gameObject.SetActive(true);
        dragWheel.fillAmount = 0f;
        startTime = Time.realtimeSinceStartup;

        yield return new WaitForSeconds(dragActivTime);

        dragWheel.transform.parent.gameObject.SetActive(false);

        if (stopDragDetect) yield break;

        state = State.BeginDrag;

        yield break;
    }

    void StopDragDetect()
    {
        if (dragDetectCoroutine != null)
        {
            stopDragDetect = true;
            StopCoroutine(dragDetectCoroutine);
            dragWheel.transform.parent.gameObject.SetActive(false);
        }
    }

    public void SetDragBuild(Build build)
    {
        buildSelected = build;
        state = State.OnDrag;
        StopDragDetect();
    }

    public void LeaveBuildMode()
    {
        if (buildSelected is Build)
        {
            Build build = buildSelected as Build;
            Destroy(build.gameObject);
        }
        state = State.Null;
        gameObject.SetActive(false);
    }

    public Build BuildSelected { get => buildSelected as Build; /*set => buildSelected = value;*/ }
}
