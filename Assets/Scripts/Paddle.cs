using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(move, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -limit, limit);
        transform.position = newPos;
    }
}
