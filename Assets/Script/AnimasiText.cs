using System.Collections;
using UnityEngine;

public class AnimasiText : MonoBehaviour
{
    public float duration = 0.3f; // Durasi animasi
    public GameObject Object; // Objek yang akan dianimasikan
    public Vector3 holdScale;
    public Vector3 popScale;

    private bool isVisible = false; // Status objek (terlihat atau tidak)

    private void Awake()
    {
        if (Object == null)
        {
            Debug.LogError("Object belum di-assign di Inspector!");
            return;
        }

        Object.transform.localScale = Vector3.zero; // Mulai dalam kondisi tersembunyi
    }

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(ShowAnimation());
        isVisible = true;
    }

    public void Hide()
    {
        if (isVisible) // Pastikan hanya bisa Hide jika sedang tampil
        {
            isVisible = false;
            StopAllCoroutines();
            StartCoroutine(HideAnimation());
        }
    }

    private IEnumerator ShowAnimation()
    {
        float elapsedTime = 0;
        float halfTime = duration / 2;

        // Step 1: Membesar ke popScale
        while (elapsedTime < halfTime)
        {
            Object.transform.localScale = Vector3.Lerp(Vector3.zero, popScale, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Object.transform.localScale = popScale;

        // Step 2: Menyesuaikan ke holdScale
        elapsedTime = 0;
        while (elapsedTime < halfTime)
        {
            Object.transform.localScale = Vector3.Lerp(popScale, holdScale, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Object.transform.localScale = holdScale;
    }

    private IEnumerator HideAnimation()
    {
        float elapsedTime = 0;
        float halfTime = duration / 2;

        // Step 1: Membesar sedikit ke popScale sebelum mengecil
        while (elapsedTime < halfTime)
        {
            Object.transform.localScale = Vector3.Lerp(holdScale, popScale, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Object.transform.localScale = popScale;

        // Step 2: Mengecil hingga hilang
        elapsedTime = 0;
        while (elapsedTime < halfTime)
        {
            Object.transform.localScale = Vector3.Lerp(popScale, Vector3.zero, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Object.transform.localScale = Vector3.zero;
    }
}
