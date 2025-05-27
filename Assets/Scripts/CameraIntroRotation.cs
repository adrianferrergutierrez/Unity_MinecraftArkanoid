using UnityEngine;

public class CameraIntroRotation : MonoBehaviour
{
    public Transform targetToOrbit; // El centro de la escena
    public float duration = 5f;     // Tiempo total de rotación
    public float rotationSpeed = 30f;
    public Vector3 finalOffset = new Vector3(0, 5, -10); // Posición final fija respecto al centro

    private float timer = 0f;
    private bool rotating = true;
    private Vector3 initialOffset;
    private Quaternion finalRotation;

    void Start()
    {
        if (targetToOrbit == null)
        {
            Debug.LogError("CameraIntroRotation: Falta asignar targetToOrbit.");
            enabled = false;
            return;
        }

        initialOffset = transform.position - targetToOrbit.position;
        finalRotation = Quaternion.LookRotation(targetToOrbit.position - (targetToOrbit.position + finalOffset));
    }

    void Update()
    {
        if (!rotating) return;

        timer += Time.deltaTime;

        if (timer < duration)
        {
            float angle = rotationSpeed * Time.deltaTime;
            initialOffset = Quaternion.AngleAxis(angle, Vector3.up) * initialOffset;
            transform.position = targetToOrbit.position + initialOffset;
            transform.LookAt(targetToOrbit.position);
        }
        else
        {
            rotating = false;

            // Posición y rotación finales fijas
            transform.position = targetToOrbit.position + finalOffset;
            transform.rotation = finalRotation;
        }
    }
}
