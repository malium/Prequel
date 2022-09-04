using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
    #region UI

    [Space]

    [SerializeField]
    public bool _active = true;

    [Space]

    [SerializeField]
    public bool _enableRotation = true;

    [SerializeField]
    public float _mouseSense = 1.8f;

    [Space]

    [SerializeField]
    public bool _enableTranslation = true;

    [SerializeField]
    public float _translationSpeed = 55f;

    [Space]

    [SerializeField]
    public bool _enableMovement = true;

    [SerializeField]
    public float _movementSpeed = 10f;

    [SerializeField]
    public float _boostedSpeed = 50f;

    [Space]

    [SerializeField]
    public bool _enableSpeedAcceleration = true;

    [SerializeField]
    public float _speedAccelerationFactor = 1.5f;

    [Space]

    [SerializeField]
    public KeyCode _initPositonButton = KeyCode.R;

    #endregion UI

    public CursorLockMode _wantedMode;

    public float _currentIncrease = 1;
    public float _currentIncreaseMem = 0;

    public Vector3 _initPosition;
    public Vector3 _initRotation;
    //Vector3 CursorPosition;
    //bool WasLocked;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }
#endif


    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
        //WasLocked = false;
    }

    private void OnEnable()
    {
        if (_active)
            _wantedMode = CursorLockMode.Locked;
    }

    // Apply requested cursor state
    private void SetCursorState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = _wantedMode = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _wantedMode = CursorLockMode.Locked;
        }

        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update()
    {
        if (!_active)
            return;

        //bool isLocked = Cursor.lockState == CursorLockMode.Locked;
        //if (!WasLocked && isLocked)
        //    CursorPosition = Input.mousePosition;
        //if (WasLocked && !isLocked)
        //    System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)CursorPosition.x, (int)CursorPosition.y);
        //if(_enableRotation)
        //{

        //}
        //SetCursorState();

        //if (Cursor.visible)
        //    return;

        // Translation
        if (_enableTranslation)
        {
            transform.Translate(_translationSpeed * Input.mouseScrollDelta.y * Time.deltaTime * Vector3.forward);
        }

        // Movement
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed = _boostedSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += transform.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= transform.forward;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= transform.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += transform.right;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
        }

        // Rotation
        if (_enableRotation)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * _mouseSense,
                Vector3.right
            );
            
            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                transform.eulerAngles.z
            );
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Return to init position
        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
        //WasLocked = _wantedMode == CursorLockMode.Locked;
    }
}
