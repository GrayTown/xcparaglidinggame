using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ParagliderController : MonoBehaviour, IFlightEntity
{

    public ParaplanSettings paraplanSettings;
    public Rigidbody2D EntityRB2D { get; set; }

    [SerializeField] private AudioSource _variometerAudio; // Ссылка на AudioSource
    [SerializeField] private AudioClip _bzzzSound; // Второй звук (bzzz bzzz)

    private PolygonCollider2D _terrainCollider;

    private Coroutine _stopRoutine;
    private Coroutine _bzzzRoutine;

    private float _horizontalSpeed = 10f;
    private float _verticalSpeed = -1f;
    private bool _currentDirection = false; // false - вправо, true - влево
    private float _liftForce = 0f;
    private float _windForce = 0f;
    private float _forcesModifier = 2f;

    private float _totalDistance = 0f;
    private Vector2 _startPoint = new Vector2(0,0);
    private Vector2 _currentPoint = new Vector2(0, 0);

    private float _lastLiftForce; // Предыдущее значение
    private float _updateThreshold = 0.5f; // Минимальное изменение для обновления
    private float _updateInterval = 1f; // Интервал обновлений (секунды)

    private float _minLift = -7f; // Минимальное значение подъема
    private float _maxLift = 7f;  // Максимальное значение подъема
    private float _minPitch = 0.6f; // Минимальная тональность
    private float _maxPitch = 1.5f; // Максимальная тональность
    private float _minVolume = 1.0f; // Громкость при слабом подъеме
    private float _maxVolume = 1.0f; // Громкость при сильном подъеме
    private bool _isPlaying = false; // Флаг состояния звука
    private bool _shouldPlayBzzz = false;

    private void Awake()
    {
        EntityRB2D = GetComponent<Rigidbody2D>();
        _terrainCollider = GameObject.FindGameObjectWithTag("GroundLanding").GetComponent<PolygonCollider2D>();
        EntityRB2D.gravityScale = 1f;
        ApplySettings();
    }

    private void Start()
    {
        // Если на старте подъём равен 0, сразу запускаем "bzzz"
        if (Mathf.Approximately(_liftForce, 0f))
        {
            _shouldPlayBzzz = true;
            StartCoroutine(PlayBzzzSound());
        }
        StartCoroutine(UpdateSoundRoutine()); // Запускаем обновление раз в updateInterval
        _startPoint = EntityRB2D.transform.position;
        _currentPoint = _startPoint;
        _totalDistance = Vector2.Distance(_currentPoint, _startPoint);
    }

    private void FixedUpdate()
    {
        _totalDistance = Vector2.Distance(_startPoint, EntityRB2D.transform.position)/100;
        // Применяем подъем в термическом потоке (если есть)
        ApplyLift();

        // Применяем ускорение/замедление по оси X (если есть)
        ApplyWind();

        Move();
    }

    private void Update()
    {
        EventManager.Instance.Publish("AGL", Mathf.RoundToInt(GetAGL()).ToString());
        EventManager.Instance.Publish("Speed", Mathf.RoundToInt(_horizontalSpeed / 1000 * 3600).ToString());
        EventManager.Instance.Publish("WindSpeed", Mathf.RoundToInt(_windForce).ToString());
        EventManager.Instance.Publish("ThermalLift", Mathf.RoundToInt(_verticalSpeed).ToString());
        EventManager.Instance.Publish("TotalDistance", Mathf.RoundToInt(_totalDistance).ToString());
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

    public void SetLiftForce(float newLiftForce)
    {
        if (Mathf.Abs(newLiftForce - _lastLiftForce) < _updateThreshold) return;
        _lastLiftForce = newLiftForce;
        _liftForce = newLiftForce;

        // Обновляем состояние для bzzz
        if (Mathf.Approximately(_liftForce, 0f))
        {
            _shouldPlayBzzz = true;
        }
        else
        {
            _shouldPlayBzzz = false;
        }
    }

    // Метод для изменения силы ветра (ускорение/замедление)
    public void SetWindForce(float newWindForce)
    {
        _windForce = newWindForce;
    }

    public void SetDirection(bool isLeft, float holdTime)
    {
        // Вычисляем динамический порог на основе holdTime
        float threshold = Mathf.Max(0.5f, paraplanSettings.turnSpeed / (1f + holdTime));

        if (holdTime > threshold)
        {
            if (_currentDirection == isLeft) return;
            _currentDirection = isLeft;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public float GetAGL()
    {
        if (_terrainCollider == null) return float.MaxValue;

        float paragliderX = transform.position.x;
        float paragliderY = transform.position.y;

        var closestPoint = _terrainCollider.points
            .Select(p => _terrainCollider.transform.TransformPoint(p))
            .OrderBy(p => Mathf.Abs(p.x - paragliderX))
            .FirstOrDefault();
        return (paragliderY - closestPoint.y) * 10f;
    }

    private IEnumerator UpdateSoundRoutine()
    {
        while (true)
        {
            if (_variometerAudio == null) yield break;

            // Если подъём равен 0, проигрываем bzzz
            if (_shouldPlayBzzz)
            {
                if (_isPlaying)
                {
                    if (_stopRoutine == null) _stopRoutine = StartCoroutine(FadeOutAndStop());
                }

                // Если bzzz не проигрывается, запускаем его
                if (_bzzzRoutine == null && _bzzzSound != null)
                {
                    _bzzzRoutine = StartCoroutine(PlayBzzzSound());
                }
            }
            else
            {
                if (!_isPlaying)
                {
                    _variometerAudio.Play();
                    _isPlaying = true;
                    if (_stopRoutine != null) StopCoroutine(_stopRoutine);
                    _stopRoutine = null;
                }

                // Прекращаем проигрывание bzzz при наличии подъема
                if (_bzzzRoutine != null)
                {
                    StopCoroutine(_bzzzRoutine);
                    _bzzzRoutine = null;
                }

                float normalizedLift = Mathf.InverseLerp(_minLift, _maxLift, _liftForce);
                _variometerAudio.pitch = Mathf.Lerp(_minPitch, _maxPitch, normalizedLift);
                _variometerAudio.volume = Mathf.Lerp(_minVolume, _maxVolume, Mathf.Abs(normalizedLift));
            }

            yield return new WaitForSeconds(_updateInterval); // Интервал между обновлениями
        }
    }

    private IEnumerator FadeOutAndStop()
    {
        while (_variometerAudio.volume > 0)
        {
            _variometerAudio.volume -= Time.deltaTime / 0.5f;
            yield return null;
        }
        _variometerAudio.Stop();
        _isPlaying = false;
        _stopRoutine = null;
    }

    private IEnumerator PlayBzzzSound()
    {
        // Проигрываем bzzz, если подъём 0
        while (_shouldPlayBzzz)
        {
            _variometerAudio.pitch = 0.5f;
            _variometerAudio.volume = 1f;
            _variometerAudio.PlayOneShot(_bzzzSound);
            yield return new WaitForSeconds(2f); // Интервал 2-3 сек
        }
        _bzzzRoutine = null; // Заканчиваем проигрывание после выхода из термика
    }
}
