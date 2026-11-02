using UnityEngine;

public class FallingNote : MonoBehaviour
{
    private float startTime;
    private float speed;
    private float hitLineY;

    public void Initialize(float start, float fallSpeed, float hitY)
    {
        startTime = start;
        speed = fallSpeed;
        hitLineY = hitY;
    }

    void Update()
    {
        float y = hitLineY
            + (startTime - Time.timeSinceLevelLoad) * speed
            + transform.localScale.y / 2f;

        transform.position = new Vector3(
            transform.position.x,
            y,
            transform.position.z
        );
    }
}