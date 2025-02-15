using UnityEngine;
using UnityEngine.InputSystem;

public class Paraglider : MonoBehaviour , IFlying , INewInputSystemHandler
{
    [SerializeField] private float _verticalSpeed = -1;
    [SerializeField] private float _currentVerticalSpeed = 0;
    [SerializeField] private float _horizontalSpeed = 10;
    [SerializeField] private float _smoothDeltaSpeed = 2;

    public float VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }
    public float CurrentVerticalSpeed { get => _currentVerticalSpeed; set => _currentVerticalSpeed = value; }
    public float HorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }
    public float SmoothDeltaSpeed { get => _smoothDeltaSpeed; set => _smoothDeltaSpeed = value; }

    private PlayerInput _playerInput;

<<<<<<< HEAD
=======
    public float upLiftNow = 0;

    private float _currentLift = 0;
    private bool _changeDirection = false;
>>>>>>> origin/main
    private Rigidbody2D _rb;
    private bool _currentDirection = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
<<<<<<< HEAD
        _currentVerticalSpeed = _verticalSpeed;
=======
        _rb.gravityScale = 0;
        _currentLift = sinkRate;
        upLiftNow = sinkRate;
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<float>("ThermalLift", ThermalLift);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<float>("ThermalLift", ThermalLift);
>>>>>>> origin/main
    }

    // Update is called once per frame
    void Update()
    {
        if (this is INewInputSystemHandler && _playerInput != null)
        {
            ProcessInput(_playerInput);
        }
    }

    void FixedUpdate()
    {
        FlyForward();
    }

    public void FlyForward()
    {
        Vector2 velocity = _rb.velocity;
        if (!_currentDirection)
        {
            velocity.x = _horizontalSpeed;
        }
        else
        {
            velocity.x = -_horizontalSpeed;
        }
        velocity.y = Mathf.Lerp(velocity.y, _currentVerticalSpeed, Time.deltaTime * _smoothDeltaSpeed);
        _rb.velocity = velocity;
    }

    public void ProcessInput(PlayerInput input)
    {
<<<<<<< HEAD
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
=======
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Lerp(_rb.linearVelocity.y, _currentLift, Time.deltaTime * smoothDeltaSpeed));
        upLiftNow = _rb.linearVelocityY;
>>>>>>> origin/main
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Инвертируем масштаб по X
        transform.localScale = scale;
    }
}
