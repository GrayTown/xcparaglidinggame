using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParagliderController))]
public class ParagliderInput : MonoBehaviour
{
    private ParagliderController _paraglider;
    private PlayerInput _playerInput;

    public float _climbTime = 0f; // Время удержания клеванты
    public bool _isTurning = false; // Флаг, определяющий, начинается ли разворот

    public float MaxTurnTime = 1f; // Максимальное время для разворота (2 секунды)
    public float _currentTurnSpeed = 0f; // Текущая скорость разворота

    public float TurnSmoothness = 5f; // Параметр для плавности (больше - плавнее)

    private float _lastInputTime = -0.2f; // Время последнего нажатия (инициализируем значением < 0, чтобы первый ввод был разрешен)
    public float InputDelay = 0.2f; // Задержка между нажатиями (в секундах)

    private void Awake()
    {
        _paraglider = GetComponent<ParagliderController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        // Проверяем, прошло ли достаточно времени с последнего нажатия
        if (Time.time - _lastInputTime >= InputDelay)
        {
            // Если нажатие по оси X
            if (moveInput.x != 0)
            {
                _climbTime += Time.deltaTime; // Увеличиваем время удержания

                // Если кнопка удерживается достаточно долго, начинаем разворот
                if (_climbTime >= MaxTurnTime)
                {
                    _isTurning = true;
                }

                // Обновляем время последнего ввода
                _lastInputTime = Time.time;
            }
            else
            {
                // Если отпустили кнопку раньше 2 секунд, разворот не произошел
                if (_climbTime < MaxTurnTime)
                {
                    _isTurning = false;
                }

                _climbTime = 0f; // Сбрасываем таймер
            }

            // Если разворот начался, передаем команду на управление парапланом
            if (_isTurning)
            {
                // Плавный разворот влево или вправо с использованием интерполяции
                float targetTurnSpeed = moveInput.x > 0 ? 1f : -1f; // Целевая скорость разворота
                _currentTurnSpeed = Mathf.Lerp(_currentTurnSpeed, targetTurnSpeed, Time.deltaTime * TurnSmoothness); // Плавный переход

                // Только факт смены направления (не меняем логику самого метода SetDirection)
                _paraglider.SetDirection(_currentTurnSpeed < 0); // Разворот влево (если < 0) или вправо (если > 0)
                if (_climbTime != 0) _climbTime = 0;
            }
        }
    }
}


