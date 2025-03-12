using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParagliderController))]
public class ParagliderInput : MonoBehaviour
{
    private ParagliderController _paraglider;
    private PlayerInput _playerInput;

    private float _holdTime = 0f; // Время удержания кнопки
    private bool _isMovingRight = false; // Флаг для определения направления

    private void Awake()
    {
        _paraglider = GetComponent<ParagliderController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        if (moveInput.x > 0)
        {
            if (!_isMovingRight) // Если сменили направление
            {
                _holdTime = 0f; // Сбрасываем время при смене направления
                _isMovingRight = true;
            }
            _holdTime += Time.deltaTime; // Увеличиваем время удержания кнопки
            _paraglider.SetDirection(false, _holdTime); // Двигаемся вправо
        }
        else if (moveInput.x < 0)
        {
            if (_isMovingRight) // Если сменили направление
            {
                _holdTime = 0f; // Сбрасываем время при смене направления
                _isMovingRight = false;
            }
            _holdTime += Time.deltaTime; // Увеличиваем время удержания кнопки
            _paraglider.SetDirection(true, _holdTime); // Двигаемся влево
        }
    }
}


