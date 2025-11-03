using UnityEngine;
using UnityEngine.UI;


public class UIplayer : MonoBehaviour
{
    [Header("Referencia al jugador")]

    public PlayerMovement player;

    [Header("UIEsteamina")]
    public Slider staminaSlider;
    public Image StaminaFill;
   
    [Header("Colores")]
    public Color normalColor = Color.white ;
    public Color BadColor = Color.darkRed ;

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerMovement>();

        staminaSlider.maxValue = player.maxStamina;
        staminaSlider.value = player.currentStamina;

        if (StaminaFill == null)
            StaminaFill = staminaSlider.fillRect.GetComponent<Image>();
        
    }
    private void Update()
    {
        staminaSlider.value = player.currentStamina;

        float percentage = player.currentStamina / player.maxStamina;

        if (percentage < 0.19f)
            StaminaFill.color = BadColor;
        else
        {
            StaminaFill.color = normalColor;
        }
    }
}
