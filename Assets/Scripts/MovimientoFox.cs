using UnityEngine;

public class MovimientoFox : MonoBehaviour
{
    public float speed = 1.0f;         // Velocidad de movimiento del zorro
    public float moveDistance = 6.0f;  // Distancia a moverse en cada segmento

    private float distanceRemainingInSegment; // Distancia que le falta por recorrer en el segmento actual

    // Start se llama antes del primer frame update
    void Start()
    {
        // Inicializamos la distancia que necesita recorrer en el primer segmento
        distanceRemainingInSegment = moveDistance;
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Calculamos cuánto se moverá este frame
        float moveThisFrame = speed * Time.deltaTime;

        // Verificamos si el movimiento de este frame es mayor o igual a lo que queda del segmento
        if (moveThisFrame >= distanceRemainingInSegment)
        {
            // Si es así, solo movemos la distancia exacta que faltaba
            transform.Translate(-Vector3.forward * distanceRemainingInSegment);

            // Luego giramos 180 grados sobre el eje Y (vertical)
            transform.Rotate(0f, 180f, 0f); // Equivalente a transform.Rotate(Vector3.up, 180f)

            // Y reiniciamos la distancia a recorrer para el nuevo segmento
            distanceRemainingInSegment = moveDistance;
        }
        else
        {
            // Si no, movemos la cantidad calculada para este frame
            transform.Translate(-Vector3.forward * moveThisFrame);

            // Y restamos esa cantidad a la distancia que queda en el segmento
            distanceRemainingInSegment -= moveThisFrame;
        }
    }
}