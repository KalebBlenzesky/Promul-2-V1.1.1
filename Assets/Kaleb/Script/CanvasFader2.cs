using UnityEngine;

public class CanvasFader2 : MonoBehaviour
{
    public Animator fadeAnimator;
    public GameObject currentCanvas;
    public GameObject nextCanvas;

    public void SwitchCanvasWithFade2()
    {
        StartCoroutine(FadeAndSwitch2());
    }

    private System.Collections.IEnumerator FadeAndSwitch2()
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
