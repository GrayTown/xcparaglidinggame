using UnityEngine;
using UnityEngine.InputSystem;

public class Paraglider : MonoBehaviour
{
    [Header("Настройки параплана")]
    public ParaplanSettings paraplanSettings;

    [Header("Текущая скорость по X")]
    public float _currentHorizontalSpeed = 0;

    [Header("Текущая скорость по Y")]
    public float _currentVerticalSpeed = 0;

    [Header("Текущая AGL")]
    public float _currentAGL = 0;

    [Header("Коллайдер рельефа")]
    public PolygonCollider2D terrainCollider;

    private PlayerInput _playerInput;
    private Rigidbody2D _rb;

    private float _verticalSpeed = -1;
    private float _horizontalSpeed = 10;
    private float _smoothDeltaSpeed = 2;
    private bool _currentDirection = false;

    public float resultHorizontalSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        if (paraplanSettings != null)
        {
            _verticalSpeed = paraplanSettings._verticalSpeed;
            _horizontalSpeed = paraplanSettings._horizontalSpeed;
            _smoothDeltaSpeed = paraplanSettings._smoothDeltaSpeed;
            _currentDirection = paraplanSettings._currentDirection;
        }

        _currentVerticalSpeed = _verticalSpeed;
        _currentHorizontalSpeed = _horizontalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput != null)
        {
            ProcessInput(_playerInput);
        }
    }

    void FixedUpdate()
    {
        FlyForward();
        _currentAGL = GetAGL();
    }

    public void FlyForward()
    {
        Vector2 velocity = Vector2.zero;
        if (!_currentDirection)
        {
            // летим вправо ----->
            if (_currentHorizontalSpeed >= 0)
            {
                // ветер вправо ----->
                velocity.x = _horizontalSpeed + _currentHorizontalSpeed;
                resultHorizontalSpeed = velocity.x;
            }
            else 
            {
                // ветер влево <-----
                velocity.x = _horizontalSpeed + _currentHorizontalSpeed;
                resultHorizontalSpeed = velocity.x;
            }
        }
        else
        {
            // летим влево <-----
            if (_currentHorizontalSpeed >= 0)
            {
                // ветер вправо ----->
                velocity.x = - _horizontalSpeed + _currentHorizontalSpeed;
                resultHorizontalSpeed = velocity.x;
            }
            else
            {
                // ветер влево <-----
                velocity.x = - _horizontalSpeed + _currentHorizontalSpeed;
                resultHorizontalSpeed = velocity.x;
            }
        }

        if (_currentVerticalSpeed > -1 && _currentVerticalSpeed <= 0)
        {
            _currentVerticalSpeed = _verticalSpeed;
        }
        if (_currentVerticalSpeed <= -1)
        {
            velocity.y = Mathf.Lerp(velocity.y, _currentVerticalSpeed * _verticalSpeed * -10, Time.deltaTime * _smoothDeltaSpeed);
            _rb.linearVelocity = velocity;
        }
        else
        {
            velocity.y = Mathf.Lerp(velocity.y, _currentVerticalSpeed * 10, Time.deltaTime * _smoothDeltaSpeed);
            _rb.linearVelocity = velocity;
        }
    }

    public void ProcessInput(PlayerInput input)
    {
        Vector2 moveInput = input.actions["Move"].ReadValue<Vector2>(); // Читаем оси ввода

        if (moveInput.x < 0 && !_currentDirection)
        {
            _currentDirection = true; // Влево
            FlipSprite();
        }
        else if (moveInput.x > 0 && _currentDirection)
        {
            _currentDirection = false; // Вправо
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Инвертируем масштаб по X
        transform.localScale = scale;
    }

    public float GetAGL()
    {
        if (terrainCollider == null)
        {
            Debug.LogWarning("Terrain Collider не назначен!");
            return float.MaxValue;
        }

        float paragliderX = transform.position.x;
        float paragliderY = transform.position.y;

        float minDistance = float.MaxValue;
        float terrainY = float.MinValue;

        // Проходим по всем точкам коллайдера
        for (int i = 0; i < terrainCollider.points.Length; i++)
        {
            Vector2 worldPoint = terrainCollider.transform.TransformPoint(terrainCollider.points[i]);

            if (Mathf.Abs(worldPoint.x - paragliderX) < minDistance)
            {
                minDistance = Mathf.Abs(worldPoint.x - paragliderX);
                terrainY = worldPoint.y;
            }
        }

        return (terrainY == float.MinValue) ? float.MaxValue : paragliderY - terrainY;
    }
}
