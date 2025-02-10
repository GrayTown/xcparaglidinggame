using UnityEngine;

public class ThermalObj : MonoBehaviour 
{
    [Header("настройка терсика")]
    public float thermalLifeTime = 0f;

    [SerializeField]
    [Header("Осталось времени жизни")]
    private float _timeLeft = 0f;

    [SerializeField]
    private bool _timerOn = false;

    private void Start()
    {
        _timeLeft = thermalLifeTime;
        _timerOn = true;
    }

    private void Update()
    {
        if (_timerOn)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
            }
            else
            {
                _timeLeft = 0;
                _timerOn = false;
                Destroy(gameObject);
            }
        }
    }


}
