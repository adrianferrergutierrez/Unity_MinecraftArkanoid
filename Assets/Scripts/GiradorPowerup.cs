using UnityEngine;

public class GiradorPowerup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float speed = 70.0f;
    private float velocidad_correr = 5.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.forward * velocidad_correr * Time.deltaTime;
        transform.Rotate(Vector3.up * Time.deltaTime * speed);

    }
}
