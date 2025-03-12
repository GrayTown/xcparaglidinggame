using System.Collections;
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

    [SerializeField] private AudioSource variometerAudio; // Ссылка на AudioSource
    [SerializeField] private AudioClip bzzzSound; // Второй звук (bzzz bzzz)

    private float lastLiftForce; // Предыдущее значение
    private float updateThreshold = 0.5f; // Минимальное изменение для обновления
    private float updateInterval = 1f; // Интервал обновлений (секунды)

    private float minLift = -7f; // Минимальное значение подъема
    private float maxLift = 7f;  // Максимальное значение подъема
    private float minPitch = 0.6f; // Минимальная тональность
    private float maxPitch = 1.5f; // Максимальная тональность
    private float minVolume = 1.0f; // Громкость при слабом подъеме
    private float maxVolume = 1.0f; // Громкость при сильном подъеме
    private bool isPlaying = false; // Флаг состояния звука
    private Coroutine stopRoutine;
    private Coroutine bzzzRoutine;
    private bool shouldPlayBzzz = false;


    private void Awake()
    {
        EntityRB2D = GetComponent<Rigidbody2D>();
        terrainCollider = GameObject.FindGameObjectWithTag("GroundLanding").GetComponent<PolygonCollider2D>();
        EntityRB2D.gravityScale = 1f;
        ApplySettings();
    }

    private void Start()
    {
        // Если на старте подъём равен 0, сразу запускаем "bzzz"
        if (Mathf.Approximately(_liftForce, 0f))
        {
            shouldPlayBzzz = true;
            StartCoroutine(PlayBzzzSound());
        }
        StartCoroutine(UpdateSoundRoutine()); // Запускаем обновление раз в updateInterval
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
        EventManager.Instance.Publish("Speed", Mathf.RoundToInt(_horizontalSpeed / 1000 * 3600).ToString());
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

    public void SetLiftForce(float newLiftForce)
    {
        if (Mathf.Abs(newLiftForce - lastLiftForce) < updateThreshold) return;
        lastLiftForce = newLiftForce;
        _liftForce = newLiftForce;

        // Обновляем состояние для bzzz
        if (Mathf.Approximately(_liftForce, 0f))
        {
            shouldPlayBzzz = true;
        }
        else
        {
            shouldPlayBzzz = false;
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
        if (terrainCollider == null) return float.MaxValue;

        float paragliderX = transform.position.x;
        float paragliderY = transform.position.y;

        var closestPoint = terrainCollider.points
            .Select(p => terrainCollider.transform.TransformPoint(p))
            .OrderBy(p => Mathf.Abs(p.x - paragliderX))
            .FirstOrDefault();
        return (paragliderY - closestPoint.y) * 10f;
    }

    private IEnumerator UpdateSoundRoutine()
    {
        while (true)
        {
            if (variometerAudio == null) yield break;

            // Если подъём равен 0, проигрываем bzzz
            if (shouldPlayBzzz)
            {
                if (isPlaying)
                {
                    if (stopRoutine == null) stopRoutine = StartCoroutine(FadeOutAndStop());
                }

                // Если bzzz не проигрывается, запускаем его
                if (bzzzRoutine == null && bzzzSound != null)
                {
                    bzzzRoutine = StartCoroutine(PlayBzzzSound());
                }
            }
            else
            {
                if (!isPlaying)
                {
                    variometerAudio.Play();
                    isPlaying = true;
                    if (stopRoutine != null) StopCoroutine(stopRoutine);
                    stopRoutine = null;
                }

                // Прекращаем проигрывание bzzz при наличии подъема
                if (bzzzRoutine != null)
                {
                    StopCoroutine(bzzzRoutine);
                    bzzzRoutine = null;
                }

                float normalizedLift = Mathf.InverseLerp(minLift, maxLift, _liftForce);
                variometerAudio.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedLift);
                variometerAudio.volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Abs(normalizedLift));
            }

            yield return new WaitForSeconds(updateInterval); // Интервал между обновлениями
        }
    }

    private IEnumerator FadeOutAndStop()
    {
        while (variometerAudio.volume > 0)
        {
            variometerAudio.volume -= Time.deltaTime / 0.5f;
            yield return null;
        }
        variometerAudio.Stop();
        isPlaying = false;
        stopRoutine = null;
    }

    private IEnumerator PlayBzzzSound()
    {
        // Проигрываем bzzz, если подъём 0
        while (shouldPlayBzzz)
        {
            variometerAudio.pitch = 0.5f;
            variometerAudio.volume = 1f;
            variometerAudio.PlayOneShot(bzzzSound);
            yield return new WaitForSeconds(2f); // Интервал 2-3 сек
        }
        bzzzRoutine = null; // Заканчиваем проигрывание после выхода из термика
    }
}
