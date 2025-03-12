using TMPro;
using UnityEngine;

public class ParagliderComputerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _aglText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _windSpeedText;
    [SerializeField] private TMP_Text _thermalLiftText;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<string>("AGL", UpdateAGL);
        EventManager.Instance.Subscribe<string>("Speed", UpdateSpeed);
        EventManager.Instance.Subscribe<string>("WindSpeed", UpdateWS);
        EventManager.Instance.Subscribe<string>("ThermalLift", UpdateTL);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<string>("AGL", UpdateAGL);
        EventManager.Instance.Unsubscribe<string>("Speed", UpdateSpeed);
        EventManager.Instance.Unsubscribe<string>("WindSpeed", UpdateWS);
        EventManager.Instance.Unsubscribe<string>("ThermalLift", UpdateTL);
    }

    private void UpdateAGL(string messageA) 
    {
        _aglText.text = messageA + " м.";
    }
    private void UpdateSpeed(string messageS)
    {
        _speedText.text = messageS + " км/ч";
    }
    private void UpdateWS(string messageWS)
    {
        _windSpeedText.text = messageWS + " м/с";
    }
    private void UpdateTL(string messageTL)
    {
        _thermalLiftText.text = messageTL + " м/с";
    }
}
