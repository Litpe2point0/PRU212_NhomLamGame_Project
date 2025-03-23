using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider health;
    [SerializeField] PlayerSwitch playerSwitch;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
        
    }
    private void Start()
    {
        healthText.text = playerSwitch.GetCurrentPlayerHealth().ToString() + "/" + playerSwitch.GetCurrentPlayerMaxHealth().ToString();
    }
    private void Update()
    {
        if(playerSwitch.GetPlayer1Active())
        {
            health.maxValue = playerSwitch.GetCurrentPlayerMaxHealth() / 100;
            health.value = playerSwitch.GetCurrentPlayerHealth() / 100;
        } else
        {
            health.maxValue = playerSwitch.GetCurrentPlayerMaxHealth() / 50;
            health.value = playerSwitch.GetCurrentPlayerHealth() / 50;
        }
        healthText.text = playerSwitch.GetCurrentPlayerHealth().ToString() + "/" + playerSwitch.GetCurrentPlayerMaxHealth().ToString();
    }
}
