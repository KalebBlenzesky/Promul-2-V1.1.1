using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFired = false;
    private float lifetime = 5f; // waktu sebelum dihancurkan
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // awalnya diam, belum ditembak
    }

    void Update()
    {
        if (isFired)
        {
            timer += Time.deltaTime;

            // Rotasi mengikuti arah gerakan
            if (rb.velocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // Hancurkan setelah 5 detik
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Fire(Vector3 force)
    {
        isFired = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
