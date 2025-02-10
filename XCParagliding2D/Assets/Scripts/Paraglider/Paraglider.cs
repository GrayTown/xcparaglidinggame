using UnityEngine;

public class Paraglider : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float sinkRate = -2f;
    [SerializeField] private float smoothDeltaSpeed = 2f;

    public float upLiftNow = 0;

    private float _currentLift = 0;
    private bool _changeDirection = false;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        MoveForward();
        ApplyLift();
    }

    void HandleInput()
    {

        if (Input.GetKey(KeyCode.A))
        {
            _changeDirection = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _changeDirection = false;
        }
        EventManager.Instance.Publish<bool>("ParagliderDirection", _changeDirection);
    }

    void MoveForward()
    {
        Vector2 velocity = _rb.linearVelocity;
        if (!_changeDirection)
        {
            velocity.x = forwardSpeed;
        }
        else
        {
            velocity.x = -forwardSpeed;
        }
        velocity.y = Mathf.Lerp(velocity.y, _currentLift, Time.deltaTime * smoothDeltaSpeed);
        _rb.linearVelocity = velocity;
        EventManager.Instance.Publish<float>("ParagliderPosition", transform.position.x);
    }

    void ApplyLift()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Lerp(_rb.linearVelocity.y, _currentLift, Time.deltaTime * smoothDeltaSpeed));
        upLiftNow = _rb.linearVelocityY;
    }

    private void ThermalLift(float thermalLift)
    {
        _currentLift = thermalLift + sinkRate;
    }
}