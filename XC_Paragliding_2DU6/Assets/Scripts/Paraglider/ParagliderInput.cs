using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParagliderController))]
public class ParagliderInput : MonoBehaviour
{
    private ParagliderController _paraglider;
    private PlayerInput _playerInput;

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
            _paraglider.SetDirection(false); // Двигаемся вправо
        }
        else if (moveInput.x < 0)
        {
            _paraglider.SetDirection(true); // Двигаемся влево
        }
    }
}
