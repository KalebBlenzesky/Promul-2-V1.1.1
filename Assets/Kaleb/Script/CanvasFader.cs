using UnityEngine;

public class CanvasFader : MonoBehaviour
{
    public Animator fadeAnimator;
    public GameObject currentCanvas;
    public GameObject nextCanvas;

    public void SwitchCanvasWithFade()
    {
        StartCoroutine(FadeAndSwitch());
    }

    private System.Collections.IEnumerator FadeAndSwitch()
    {
        // Fade out (ke hitam)
        fadeAnimator.SetTrigger("DoFadeOut");
        yield return new WaitForSeconds(0.5f); // tunggu animasi fade hitam selesai

        // Ganti halaman (canvas)
        currentCanvas.SetActive(false);
        nextCanvas.SetActive(true);

        // Fade in (dari hitam ke tampilan baru)
        fadeAnimator.SetTrigger("DoFadeIn");
    }
}
