using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ParagliderController : MonoBehaviour, IFlightEntity
{

    public ParaplanSettings paraplanSettings;
    public Rigidbody2D EntityRB2D { get; set; }
    
    private PolygonCollider2D terrainCollider;

    private float _horizontalSpeed = 10f;
    private float _verticalSpeed = -1f;
    private bool _currentDirection = false; // false - вправо, true - влево
    private float _liftForce = 0f;
    private float _windForce = 0f;
    private float _forcesModifier = 2f;

    private void Awake()
    {
        EntityRB2D = GetComponent<Rigidbody2D>();
        terrainCollider = GameObject.FindGameObjectWithTag("GroundLanding").GetComponent<PolygonCollider2D>();
        EntityRB2D.gravityScale = 1f;
        ApplySettings();
    }

    private void FixedUpdate()
    {
        // Применяем подъем в термическом потоке (если есть)
        ApplyLift();

        // Применяем ускорение/замедление по оси X (если есть)
        ApplyWind();

        Move();
    }

    private void Update()
    {
        EventManager.Instance.Publish("AGL", Mathf.RoundToInt(GetAGL()).ToString());
        EventManager.Instance.Publish("Speed", Mathf.RoundToInt(_horizontalSpeed/1000 * 3600).ToString());
        EventManager.Instance.Publish("WindSpeed", Mathf.RoundToInt(_windForce).ToString());
        EventManager.Instance.Publish("ThermalLift", Mathf.RoundToInt(_verticalSpeed).ToString());
    }

    private void ApplySettings()
    {
        if (paraplanSettings == null) return;

        _verticalSpeed = paraplanSettings.VerticalSpeed;
        _horizontalSpeed = paraplanSettings.HorizontalSpeed;
        _currentDirection = paraplanSettings.CurrentDirection;
    }

    private void ApplyLift()
    {
        _verticalSpeed = paraplanSettings.VerticalSpeed;

        if (_liftForce != 0)
        {
            _verticalSpeed += _liftForce;
        }
        if (_liftForce == 0) 
        {
            _verticalSpeed = paraplanSettings.VerticalSpeed;
        }
    }

    private void ApplyWind()
    {
        _horizontalSpeed = paraplanSettings.HorizontalSpeed;

        if (_windForce != 0)
        {

            _horizontalSpeed += _windForce;
        }
        else 
        {
            _horizontalSpeed = paraplanSettings.HorizontalSpeed;
        }
    }

    private void Move()
    {
        // Обновляем позицию
        if (!_currentDirection)
        {
            EntityRB2D.linearVelocityX = _horizontalSpeed * _forcesModifier;
        }
        else 
        {
            EntityRB2D.linearVelocityX = -_horizontalSpeed * _forcesModifier;
        }
        EntityRB2D.linearVelocityY = _verticalSpeed * 2f;
    }

    // Метод для изменения силы подъема (термический поток)
    public void SetLiftForce(float newLiftForce)
    {
        _liftForce = newLiftForce;
    }

    // Метод для изменения силы ветра (ускорение/замедление)
    public void SetWindForce(float newWindForce)
    {
        _windForce = newWindForce;
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

    public float GetAGL()
    {
        if (terrainCollider == null) return float.MaxValue;

        float paragliderX = transform.position.x;
        float paragliderY = transform.position.y;

        var closestPoint = terrainCollider.points
            .Select(p => terrainCollider.transform.TransformPoint(p))
            .OrderBy(p => Mathf.Abs(p.x - paragliderX))
            .FirstOrDefault();
        return (paragliderY - closestPoint.y) * 10f;
    }
}

