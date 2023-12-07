using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RocketAmmoSliderScript : MonoBehaviour
{
    PlayerMovement playerMovement;
    public TMP_Text rocketText;
    public Slider rocketReloadSlider;
    public CanvasGroup sliderCanvasGroup;

    private int currentRockets;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        rocketText = GetComponentInChildren<TMP_Text>();
        rocketReloadSlider = GetComponentInChildren<Slider>();
        sliderCanvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rocketReloadSlider.value = playerMovement.rocketReloadSlider.value;
        currentRockets = playerMovement.rockets;
        rocketText.text = currentRockets.ToString();

        if (currentRockets == playerMovement.maxRockets && sliderCanvasGroup.alpha > 0)
            sliderCanvasGroup.alpha -= .03f;
        else if (sliderCanvasGroup.alpha < 0)
            sliderCanvasGroup.alpha = 0;

        if (currentRockets < playerMovement.maxRockets)
            sliderCanvasGroup.alpha = 1;
    }
}
