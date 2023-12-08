using UnityEngine;

public class UISFXManagerScript : MonoBehaviour
{
    public AudioSource hoverButton;
    public AudioSource clickButton;
    public void PlayHoverSFX()
    {
        hoverButton.Play();
    }
    public void PlayClickSFX()
    {
        clickButton.Play();
    }
}
