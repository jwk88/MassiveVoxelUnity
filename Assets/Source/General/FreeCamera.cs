namespace RideTools
{
    using UnityEngine;

    public class FreeCamera : MonoBehaviour
    {
        [SerializeField] Camera _cameraChild;
        [SerializeField] float _mouseSensitivity = 5;
        [SerializeField] float _mouseSmoothing = 4;
        [SerializeField] float _movementSpeed = 10f;
        [SerializeField] bool _oldInputSystem = true;
        [SerializeField] bool _startDisabled = false;
        [SerializeField] KeyCode _toggleKey = KeyCode.Tab;

        Vector2 _sensitivity;
        Vector2 _smoothing;
        Vector2 _mousePosition;
        Quaternion _viewRotation;
        Quaternion _bodyRotation;
        bool _enabled = true;

        public Camera Camera { get => _cameraChild; private set => _cameraChild = value; }

        void OnEnable()
        {
            if (_startDisabled)
            {
                Disable();
                return;
            }

            Initialize();
        }

        void Initialize()
        {
            _sensitivity = Vector2.one;
            _smoothing = Vector2.zero;
            _mousePosition = Vector2.zero;
            _sensitivity *= _mouseSensitivity * _mouseSmoothing;

            _viewRotation = Quaternion.Euler(Camera.transform.localRotation.eulerAngles);
            _bodyRotation = Quaternion.Euler(transform.rotation.eulerAngles);

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                Toggle();
            }
            if (!_enabled) return;

            if (_oldInputSystem)
            {
                UpdateMouse(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                UpdateMovement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));   
            }
            else
            {
                Debug.LogWarning("New input system for free camera has not been implemented yet");
            }
        }

        public void UpdateMovement(float x, float z)
        {
            var dir = new Vector3(x, 0, z);
            var sprint = Input.GetButton("Fire3");
            var movement = Camera.transform.rotation * dir * (sprint ? _movementSpeed * 3 : _movementSpeed) * Time.deltaTime;
            if (Physics.Raycast(Camera.transform.position, movement.normalized, out var hit, 1f))
            {
                movement = ReflectMovement(movement, hit);
            }
            transform.position += movement;
        }

        Vector3 ReflectMovement(Vector3 movement, RaycastHit hit)
        {
            return Vector3.Reflect(movement.normalized, hit.normal) * movement.magnitude;
        }

        public void UpdateMouse(float x, float y)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            x *= 0.1f;
            y *= 0.1f;
#endif

            var m_delta = new Vector2(x, y);
            m_delta = Vector2.Scale(m_delta, _sensitivity);

            _smoothing.x = Mathf.Lerp(_smoothing.x, m_delta.x, 1f / _mouseSmoothing);
            _smoothing.y = Mathf.Lerp(_smoothing.y, m_delta.y, 1f / _mouseSmoothing);

            _mousePosition += _smoothing;
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, -90, 90);
            Camera.transform.localRotation = Quaternion.AngleAxis(-_mousePosition.y, _viewRotation * Vector3.right) * _viewRotation;

            var yRot = Quaternion.AngleAxis(_mousePosition.x, Vector3.up);
            transform.rotation = yRot * _bodyRotation;
        }

        public void Enable()
        {
            _enabled = true;
            Initialize();
        }

        public void Toggle()
        {
            if (_enabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public void Disable()
        {
            _enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
