using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ParagliderController : MonoBehaviour, IFlightEntity
{

    public ParaplanSettings paraplanSettings;

    public Rigidbody2D EntityRB2D { get; set; }
    public float CurrentHorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }
    public float CurrentVerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }

    private float _horizontalSpeed;
    private float _verticalSpeed;
    private float _modifierVerticalSpeed = 10f;
    private float _smoothDeltaSpeed;
    private bool _currentDirection; // false - вправо, true - влево

    private void Awake()
    {
        EntityRB2D = GetComponent<Rigidbody2D>();
        ApplySettings();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ApplySettings()
    {
        if (paraplanSettings == null) return;

        _verticalSpeed = paraplanSettings.VerticalSpeed;
        _horizontalSpeed = paraplanSettings.HorizontalSpeed;
        _smoothDeltaSpeed = paraplanSettings.SmoothDeltaSpeed;
        _currentDirection = paraplanSettings.CurrentDirection;
        _modifierVerticalSpeed = paraplanSettings.ModifierVerticalSpeed;

        CurrentVerticalSpeed = _verticalSpeed;
        CurrentHorizontalSpeed = _horizontalSpeed;
    }

    public void Move()
    {
        Vector2 velocity = Vector2.zero;
        if (!_currentDirection)
        {
            velocity.x = _horizontalSpeed + CurrentHorizontalSpeed;
        }
        else
        {
            velocity.x = -_horizontalSpeed + CurrentHorizontalSpeed;
        }

        if (CurrentVerticalSpeed > -1 && CurrentVerticalSpeed <= 0)
        {
            CurrentVerticalSpeed = _verticalSpeed;
        }
        if (CurrentVerticalSpeed <= -1)
        {
            velocity.y = Mathf.Lerp(velocity.y, CurrentVerticalSpeed * _verticalSpeed * -_modifierVerticalSpeed, Time.deltaTime * _smoothDeltaSpeed);
            EntityRB2D.linearVelocity = velocity;
        }
        else
        {
            velocity.y = Mathf.Lerp(velocity.y, CurrentVerticalSpeed * _modifierVerticalSpeed, Time.deltaTime * _smoothDeltaSpeed);
            EntityRB2D.linearVelocity = velocity;
        }
    }

    public void SetDirection(bool isLeft)
    {
        if (_currentDirection == isLeft) return;
        _currentDirection = isLeft;
        FlipSprite();
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

