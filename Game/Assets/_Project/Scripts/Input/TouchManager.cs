using UnityEngine.InputSystem;
using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    public float speed = 0.01f;
    public float swipeSpeed = 2.0f;
    private float prevMagnitude = 0;
    private int touchCount = 0;
    public float zoomThreshold = 0.1f;
    public float swipeThreshold = 50f;
    private Vector2 swipeStartPos;
    private Vector2 swipeCurrentPos;


    public bool canSlide;
    private Camera cam;

    private enum TouchState
    {
        None,
        Zooming,
        Swiping
    }
    private TouchState currentState = TouchState.None;

    private void Start()
    {
        cam = Camera.main;
        canSlide = false;
        // Mouse scroll for zoom
        var scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>().y * speed);

        // Touch gesture setup
        var touch0contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        var touch1contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        touch0contact.Enable();
        touch1contact.Enable();

        touch0contact.performed += _ => touchCount++;
        touch1contact.performed += _ => touchCount++;
        touch0contact.canceled += _ => { touchCount--; prevMagnitude = 0; ResetState(); };
        touch1contact.canceled += _ => { touchCount--; prevMagnitude = 0; ResetState(); };

        var touch0pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        var touch1pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        touch0pos.Enable();
        touch1pos.Enable();

        touch1pos.performed += _ =>
        {
            if (touchCount < 2 || currentState == TouchState.Swiping) { return; }

            var touch0Position = touch0pos.ReadValue<Vector2>();
            var touch1Position = touch1pos.ReadValue<Vector2>();

            var magnitude = (touch0Position - touch1Position).magnitude;

            if (prevMagnitude == 0)
                prevMagnitude = magnitude;

            var difference = prevMagnitude - magnitude;
            prevMagnitude = magnitude;

            if (Mathf.Abs(difference) < zoomThreshold)
                return;

            currentState = TouchState.Zooming;

            Vector2 midpoint = (touch0Position + touch1Position) / 2;
            ZoomAt(midpoint, difference * speed);
        };

        touch0pos.performed += ctx =>
        {
            if ((touchCount > 0 && currentState != TouchState.Zooming) && canSlide)
            {
                swipeCurrentPos = ctx.ReadValue<Vector2>();
                if (swipeStartPos == Vector2.zero)
                {
                    swipeStartPos = swipeCurrentPos;
                }

                Vector2 swipeDelta = swipeStartPos - swipeCurrentPos;

                if (swipeDelta.magnitude > swipeThreshold)
                {
                    currentState = TouchState.Swiping;
                    MoveCamera(swipeDelta);
                }
            }
        };

        touch0contact.canceled += _ =>
        {
            swipeStartPos = Vector2.zero;
        };
    }

    private void CameraZoom(float increment)
    {
        float newSize = Mathf.Clamp(cam.orthographicSize + increment, 3.5f, 5);
        cam.orthographicSize = newSize;
    }

    private void ZoomAt(Vector2 screenPoint, float increment)
    {
        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, cam.nearClipPlane));
        float oldSize = cam.orthographicSize;

        CameraZoom(increment);

        float sizeDelta = cam.orthographicSize - oldSize;
        Vector3 focusOffset = worldPoint - cam.transform.position;
        focusOffset.y = 0;
        cam.transform.position -= focusOffset * (sizeDelta / oldSize);
    }

    private void MoveCamera(Vector2 swipeDelta)
    {
        if (swipeDelta.magnitude > swipeThreshold)
        {
            float moveAmount = swipeDelta.x * swipeSpeed * Time.deltaTime;
            float newXPosition = cam.transform.position.x + moveAmount;
            newXPosition = Mathf.Clamp(newXPosition, -200, 200); //TODO : Get map limit
            cam.transform.position = new Vector3(newXPosition, cam.transform.position.y, cam.transform.position.z);
        }
    }

    private void ResetState()
    {
        currentState = TouchState.None;
        swipeStartPos = Vector2.zero;
    }
}
