using UnityEngine;

public class UIFadeTrigger : MonoBehaviour
{
    public Animator fadeAnimator;

    public void FadeOut()
    {
        fadeAnimator.SetTrigger("DoFadeOut");
    }

    public void FadeIn()
    {
        fadeAnimator.SetTrigger("DoFadeIn");
    }
}
