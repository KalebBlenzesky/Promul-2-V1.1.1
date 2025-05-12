using UnityEngine;

public class ParticleMover : MonoBehaviour
{
    private Vector3 target;
    private float frequency;
    private float moveSpeed;
    private float radius;

    private float angle;
    private float startTime;
    private Vector3 axis;

    public void SetTargetAndParams(Vector3 targetPos, float freq, float amp, float speed)
    {
        target = targetPos;
        frequency = freq;
        moveSpeed = speed;
        radius = Vector3.Distance(transform.position, targetPos);
        startTime = Time.time;

        Vector3 dir = transform.position - targetPos;
        angle = Mathf.Atan2(dir.y, dir.x);

        axis = Vector3.forward;
    }

    void Update()
    {
        float timeElapsed = Time.time - startTime;
        float deltaAngle = frequency * timeElapsed * Mathf.PI * 2;

        radius -= moveSpeed * Time.deltaTime;
        radius = Mathf.Max(radius, 0);

        float x = Mathf.Cos(angle + deltaAngle) * radius;
        float y = Mathf.Sin(angle + deltaAngle) * radius;

        transform.position = target + new Vector3(x, y, 0f);

        if (radius <= 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
