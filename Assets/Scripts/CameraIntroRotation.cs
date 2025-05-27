using UnityEngine;

public class CameraIntroRotation : MonoBehaviour
{
    public Transform targetToOrbit; // Centro de la escena
    public float duration = 5f;     // Duraci�n total
    public Vector3 finalOffset = new Vector3(0, 5, -10); // Posici�n final deseada

    private float timer = 0f;
    private bool rotating = true;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 endPosition;
    private Quaternion endRotation;

    void Start()
    {
        if (targetToOrbit == null)
        {
            Debug.LogError("CameraIntroRotation: Falta asignar targetToOrbit.");
            enabled = false;
            return;
        }

        // Guardar posici�n y rotaci�n iniciales
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Calcular posici�n y rotaci�n finales
        endPosition = targetToOrbit.position + finalOffset;
        endRotation = Quaternion.LookRotation(targetToOrbit.position - endPosition);
    }

    void Update()
    {
        if (!rotating) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // Interpolaci�n suave
        transform.position = Vector3.Slerp(startPosition, endPosition, t);
        transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        if (t >= 1f)
        {
            rotating = false;
        }
    }
}
