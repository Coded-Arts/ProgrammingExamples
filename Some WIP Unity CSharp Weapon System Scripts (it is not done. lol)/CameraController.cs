using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float sensitivity = 100.0f;
    [SerializeField] private Vector2 cameraVerticalRotationRange = new Vector2(-70, 70);
    [SerializeField] private Vector2 cameraHorizontalalRotationRange = new Vector2(-180, 180);
    [SerializeField] private CursorLockMode cursorLockMode = CursorLockMode.Locked;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool useXAxis = true, useYAxis = true;
    [SerializeField] private float lookSmoothing = 5f;

    private float refX = 0, refY = 0;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    private Vector2 initialTouchRotation;

    public bool PauseCameraController { get; set; } = false;

    public float Sensitivity {
        set {
            sensitivity = value;
        }
    }

    private void Awake() {
        if (!cameraTransform) {
            cameraTransform = GetComponent<Transform>();
        }
    }

    private void Start() {
        Vector3 rot = cameraTransform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = cursorLockMode;
    }

    private void Update() {
        if (!PauseCameraController) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

#if UNITY_EDITOR || UNITY_STANDALONE
            if (useXAxis)
                rotY += mouseX * sensitivity * Time.smoothDeltaTime;
            if (useYAxis)
                rotX += mouseY * sensitivity * Time.smoothDeltaTime;

            //rotX = Mathf.Clamp(rotX, cameraVerticalRotationRange.x, cameraVerticalRotationRange.y);
            rotX = Mathf.SmoothDamp(rotX, Mathf.Clamp(rotX, cameraVerticalRotationRange.x, cameraVerticalRotationRange.y), ref refX, lookSmoothing * Time.deltaTime);
            //rotY = Mathf.Clamp(rotY, cameraHorizontalalRotationRange.x, cameraHorizontalalRotationRange.y);
            rotY = Mathf.SmoothDamp(rotY, Mathf.Clamp(rotY, cameraHorizontalalRotationRange.x, cameraHorizontalalRotationRange.y), ref refY, lookSmoothing * Time.deltaTime);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            //Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            cameraTransform.localRotation = localRotation;
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0) {
                Touch touch;
                touch = Input.GetTouch(0);

                switch (touch.phase) {
                    case TouchPhase.Began:
                        initialTouchRotation = touch.position;
                        break;
                    case TouchPhase.Moved:
                        rotY += mouseX * sensitivity * Time.smoothDeltaTime;
                        rotX += mouseY * sensitivity * Time.smoothDeltaTime;

                        rotX = Mathf.Clamp(rotX, cameraVerticalRotationRange.x, cameraVerticalRotationRange.y);

                        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

                        transform.rotation = localRotation;
                        break;
                }
            }
#endif
        }
    }

    public void LockCursor () {
        cursorLockMode = CursorLockMode.Locked;
        Cursor.lockState = cursorLockMode;
    }

    public void UnlockCursor () {
        cursorLockMode = CursorLockMode.None;
        Cursor.lockState = cursorLockMode;
    }

    public void ToggleCursorLocked () {
        if (cursorLockMode == CursorLockMode.Confined || cursorLockMode == CursorLockMode.Locked) {
            cursorLockMode = CursorLockMode.None;
        }
        else {
            cursorLockMode = CursorLockMode.Locked;
        }
        Cursor.lockState = cursorLockMode;
    }

}
