using System.Linq;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    [Header("Настройки ветров")]
    public WindGeneratorSettings settings;

    [SerializeField] private PolygonCollider2D _levelBounds;
    [SerializeField] private float _gizmosPointSize = 5f;

    private float[] _windSpeeds;
    private float[] _targetWindSpeeds;
    private float _windTimer;

    private void Start()
    {
        WindZoneLayerSetup();
        _windSpeeds = new float[settings.windZonesHeight.Length];
        _targetWindSpeeds = new float[settings.windZonesHeight.Length];
        GenerateInitialWind();
    }

    private void Update()
    {
        _windTimer += Time.deltaTime;
        if (_windTimer >= settings.windUpdateInterval)
        {
            UpdateTargetWindSpeeds();
            _windTimer = 0f;
        }
        SmoothWindChange();
    }

    private void WindZoneLayerSetup()
    {
        // Получаем все точки из levelBounds
        var path = _levelBounds.GetPath(0);
        float minX = path.Min(p => p.x);
        float maxX = path.Max(p => p.x);

        // Создаем BoxCollider2D для каждой зоны ветра
        for (int i = 0; i < settings.windZonesHeight.Length - 1; i++)
        {
            // Для каждой зоны по Y[i] и Y[i+1] создаем BoxCollider2D
            float lowerY = settings.windZonesHeight[i];
            float upperY = settings.windZonesHeight[i + 1];

            // Вычисляем центр и размер для BoxCollider2D
            Vector2 center = new Vector2((minX + maxX) / 2, (lowerY + upperY) / 2);
            Vector2 size = new Vector2(maxX - minX, upperY - lowerY);

            // Добавляем BoxCollider2D
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.offset = center;
            boxCollider.size = size;
            boxCollider.isTrigger = true;
        }
    }

    private void GenerateInitialWind()
    {
        // Инициализируем предыдущее значение как положительное
        float previousValue = 1f;

        for (int i = 0; i < _windSpeeds.Length; i++)
        {
            // Генерируем новое случайное значение
            _windSpeeds[i] = Random.Range(settings.minWindSpeed, settings.maxWindSpeed);

            // Проверяем условие: если текущее и предыдущее значения положительные
            if (_windSpeeds[i] > 0 && previousValue > 0)
            {
                // Меняем знак на отрицательный
                _targetWindSpeeds[i] = -_windSpeeds[i];
            }
            else
            {
                if (_windSpeeds[i] < 0 && previousValue < 0)
                {
                    // Меняем знак на отрицательный
                    _targetWindSpeeds[i] = _windSpeeds[i];
                }
                else
                {
                    _targetWindSpeeds[i] = _windSpeeds[i];
                }
            }
            // Сохраняем текущее значение для следующей итерации
            previousValue = _windSpeeds[i];
        }
    }

    private void UpdateTargetWindSpeeds()
    {
        for (int i = 0; i < _targetWindSpeeds.Length; i++)
        {
            _targetWindSpeeds[i] = Random.Range(settings.minWindSpeed, settings.maxWindSpeed);
        }
    }

    private void SmoothWindChange()
    {
        for (int i = 0; i < _windSpeeds.Length; i++)
        {
            _windSpeeds[i] = Mathf.MoveTowards(_windSpeeds[i], _targetWindSpeeds[i], settings.windChangeSpeed * Time.deltaTime);
        }
    }

    public float GetWindSpeedAtHeight(float height)
    {
        for (int i = 0; i < settings.windZonesHeight.Length - 1; i++)
        {
            if (height >= settings.windZonesHeight[i] && height < settings.windZonesHeight[i + 1])
            {
                float t = Mathf.InverseLerp(settings.windZonesHeight[i], settings.windZonesHeight[i + 1], height);
                return Mathf.Lerp(_windSpeeds[i], _windSpeeds[i + 1], t);
            }
        }
        return _windSpeeds[_windSpeeds.Length - 1];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IFlightEntity entity = other.GetComponent<IFlightEntity>();
        if (entity != null)
        {
            float windSpeed = GetWindSpeedAtHeight(entity.EntityRB2D.transform.position.y);
            entity.SetWindForce(windSpeed);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IFlightEntity entity = other.GetComponent<IFlightEntity>();
        if (entity != null)
        {
            float windSpeed = GetWindSpeedAtHeight(entity.EntityRB2D.transform.position.y);
            entity.SetWindForce(windSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IFlightEntity entity = other.GetComponent<IFlightEntity>();
        if (entity != null)
        {
            float windSpeed = GetWindSpeedAtHeight(entity.EntityRB2D.transform.position.y);
            entity.SetWindForce(0);
        }
    }

    // ✅ Рисуем точку в редакторе - тут дофига чего мне не надо сейчас, но иногда надо :)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; // Цвет точки
        Gizmos.DrawSphere(transform.position, _gizmosPointSize); // Радиус 0.5 для наглядности

        for (int i = 0; i < settings.windZonesHeight.Length; i++) 
        {
            var leftUpCorner2 = new Vector3(_levelBounds.GetPath(0).ElementAt(0).x, settings.windZonesHeight[i], 0);
            var rightUpCorner2 = new Vector3(_levelBounds.GetPath(0).ElementAt(1).x, settings.windZonesHeight[i], 0);
            Gizmos.color = Color.blue; // Цвет точки
            Gizmos.DrawLine(leftUpCorner2, rightUpCorner2);
        }


    }
}
