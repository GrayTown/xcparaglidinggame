using System.Linq;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    [Header("Настройки ветров")]
    public WindGeneratorSettings settings;

    [SerializeField] private PolygonCollider2D levelBounds;
    [SerializeField] private float gizmosPointSize = 5f;

    private float[] windSpeeds;
    private float[] targetWindSpeeds;
    private float windTimer;

    private void Start()
    {
        WindZoneLayerSetup();
        windSpeeds = new float[settings.windZonesHeight.Length];
        targetWindSpeeds = new float[settings.windZonesHeight.Length];
        GenerateInitialWind();
    }

    private void Update()
    {
        windTimer += Time.deltaTime;
        if (windTimer >= settings.windUpdateInterval)
        {
            UpdateTargetWindSpeeds();
            windTimer = 0f;
        }
        SmoothWindChange();
    }

    private void WindZoneLayerSetup()
    {
        // Получаем все точки из levelBounds
        var path = levelBounds.GetPath(0);
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
        for (int i = 0; i < windSpeeds.Length; i++)
        {
            windSpeeds[i] = Random.Range(settings.minWindSpeed, settings.maxWindSpeed);
            targetWindSpeeds[i] = windSpeeds[i];
        }
    }

    private void UpdateTargetWindSpeeds()
    {
        for (int i = 0; i < targetWindSpeeds.Length; i++)
        {
            targetWindSpeeds[i] = Random.Range(settings.minWindSpeed, settings.maxWindSpeed);
        }
    }

    private void SmoothWindChange()
    {
        for (int i = 0; i < windSpeeds.Length; i++)
        {
            windSpeeds[i] = Mathf.MoveTowards(windSpeeds[i], targetWindSpeeds[i], settings.windChangeSpeed * Time.deltaTime);
        }
    }

    public float GetWindSpeedAtHeight(float height)
    {
        for (int i = 0; i < settings.windZonesHeight.Length - 1; i++)
        {
            if (height >= settings.windZonesHeight[i] && height < settings.windZonesHeight[i + 1])
            {
                float t = Mathf.InverseLerp(settings.windZonesHeight[i], settings.windZonesHeight[i + 1], height);
                return Mathf.Lerp(windSpeeds[i], windSpeeds[i + 1], t);
            }
        }
        return windSpeeds[windSpeeds.Length - 1];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            float windSpeed = GetWindSpeedAtHeight(paraglider.transform.position.y);
            paraglider._currentHorizontalSpeed = windSpeed;
            Debug.Log("Вошел в другую зону ветра.-----------------------------------------");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            float windSpeed = GetWindSpeedAtHeight(paraglider.transform.position.y);
            paraglider._currentHorizontalSpeed = windSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            paraglider._currentHorizontalSpeed = 0f;
            Debug.Log("Вышел из старой зоны ветра.==========================================");
        }
    }

    // ✅ Рисуем точку в редакторе - тут дофига чего мне не надо сейчас, но иногда надо :)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; // Цвет точки
        Gizmos.DrawSphere(transform.position, gizmosPointSize); // Радиус 0.5 для наглядности
       
        ////for wind gizmos

        //var leftUpCorner = new Vector3(levelBounds.GetPath(0).ElementAt(0).x, levelBounds.GetPath(0).ElementAt(0).y, 0);
        //var rightUpCorner = new Vector3(levelBounds.GetPath(0).ElementAt(1).x, levelBounds.GetPath(0).ElementAt(1).y, 0);
        //var rightDownCorner = new Vector3(levelBounds.GetPath(0).ElementAt(2).x, levelBounds.GetPath(0).ElementAt(2).y, 0);
        //var leftDownCorner = new Vector3(levelBounds.GetPath(0).ElementAt(3).x, levelBounds.GetPath(0).ElementAt(3).y, 0);

        //Gizmos.color = Color.blue; // Цвет точки
        //Gizmos.DrawLine(leftUpCorner, rightUpCorner);
        //Gizmos.DrawLine(rightUpCorner, rightDownCorner);
        //Gizmos.DrawLine(rightDownCorner, leftDownCorner);
        //Gizmos.DrawLine(leftDownCorner, leftUpCorner);

        for (int i = 0; i < settings.windZonesHeight.Length; i++) 
        {
            var leftUpCorner2 = new Vector3(levelBounds.GetPath(0).ElementAt(0).x, settings.windZonesHeight[i], 0);
            var rightUpCorner2 = new Vector3(levelBounds.GetPath(0).ElementAt(1).x, settings.windZonesHeight[i], 0);
            //var rightDownCorner2 = new Vector3(levelBounds.GetPath(0).ElementAt(2).x, levelBounds.GetPath(0).ElementAt(2).y + windZonesHeight[i], 0);
            //var leftDownCorner2 = new Vector3(levelBounds.GetPath(0).ElementAt(3).x, levelBounds.GetPath(0).ElementAt(3).y + windZonesHeight[i], 0);

            Gizmos.color = Color.blue; // Цвет точки
            Gizmos.DrawLine(leftUpCorner2, rightUpCorner2);
            //Gizmos.DrawLine(rightUpCorner2, rightDownCorner2);
            //Gizmos.DrawLine(rightDownCorner2, leftDownCorner2);
            //Gizmos.DrawLine(leftDownCorner2, leftUpCorner2);
        }


    }
}
