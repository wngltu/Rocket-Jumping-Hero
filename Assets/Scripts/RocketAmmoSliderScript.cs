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
    public Image newSlider1;
    public Image newSlider2;

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
        {
            sliderCanvasGroup.alpha = 1;
            newSlider1.color = new Color(newSlider1.color.r, newSlider1.color.g, newSlider1.color.b, 1);
            newSlider2.color = new Color(newSlider2.color.r, newSlider2.color.g, newSlider2.color.b, 1);
        }

        if (currentRockets == playerMovement.maxRockets && newSlider1.color.a > 0)
        {
            newSlider1.color = new Color(newSlider1.color.r, newSlider1.color.g, newSlider1.color.b, newSlider1.color.a - .03f);
            newSlider2.color = new Color(newSlider2.color.r, newSlider2.color.g, newSlider2.color.b, newSlider2.color.a - .03f);
        }
        else if (newSlider1.color.a < 0)
        {
            newSlider1.color = new Color(newSlider1.color.r, newSlider1.color.g, newSlider1.color.b, 0);
            newSlider2.color = new Color(newSlider2.color.r, newSlider2.color.g, newSlider2.color.b, 0);
        }
            

    }
}
