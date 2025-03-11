using TMPro;
using UnityEngine;

public class ParagliderComputerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _aglText;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<string>("AGL", UpdateAGL, 0);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<string>("AGL", UpdateAGL);
    }

    private void UpdateAGL(string message) 
    {
        _aglText.text = message;
    }
}
