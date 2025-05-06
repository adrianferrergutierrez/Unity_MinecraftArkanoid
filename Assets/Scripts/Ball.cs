using UnityEngine;

//pelota
public class Ball3D : MonoBehaviour
{
    public float launchForce = 500f;
    private Rigidbody rb;
    private bool launched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.forward * launchForce);
            launched = true;
        }
        if(Input.GetKeyDown(KeyCode.R)) transform.position = new Vector3(0, 10, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Endline"))
        {
            // Reiniciar escena o vida
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    
}
