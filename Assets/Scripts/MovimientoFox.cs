using UnityEngine;

public class MovimientoSteve : MonoBehaviour // Puedes mantener MovimientoFox si quieres
{
    public float speed;         // Velocidad de movimiento. Si es negativo, se moverá "hacia atrás" del vector de dirección base.
    public float moveDistance;  // Distancia a moverse en cada segmento.

    private float distanceRemainingInSegment; // Distancia que le falta por recorrer en el segmento actual
    private Vector3 baseMoveDirection = -Vector3.forward; // Dirección base de movimiento local (hacia atrás del Z local)

    void Start()
    {
        // Inicializamos la distancia que necesita recorrer en el primer segmento
        distanceRemainingInSegment = moveDistance;
    }

    void Update()
    {
        // Usamos el valor absoluto de speed para calcular cuánto se mueve,
        // ya que la distancia recorrida siempre es positiva.
        float speedMagnitude = Mathf.Abs(speed);
        float distanceCoveredThisFrame = speedMagnitude * Time.deltaTime;

        // Determinamos la dirección final del movimiento.
        // Si speed es < 0, invertimos la baseMoveDirection.
        // Ejemplo: si speed = -2 y baseMoveDirection = (0,0,-1) [-Vector3.forward]
        //          entonces effectiveDirection = (0,0,1) [Vector3.forward]
        // Si speed = 2 y baseMoveDirection = (0,0,-1)
        //          entonces effectiveDirection = (0,0,-1)
        Vector3 effectiveDirection = baseMoveDirection;
        if (speed < 0)
        {
            effectiveDirection = -baseMoveDirection; // Invierte la dirección base si speed es negativo
        }


        // Verificamos si el movimiento de este frame (en magnitud) es mayor o igual a lo que queda del segmento
        if (distanceCoveredThisFrame >= distanceRemainingInSegment)
        {
            // Si es así, solo movemos la distancia exacta que faltaba en la dirección efectiva
            transform.Translate(effectiveDirection * distanceRemainingInSegment);

            // Luego giramos 180 grados sobre el eje Y (vertical)
            transform.Rotate(0f, 180f, 0f);

            // Y reiniciamos la distancia a recorrer para el nuevo segmento
            distanceRemainingInSegment = moveDistance;
        }
        else
        {
            // Si no, movemos la cantidad calculada para este frame en la dirección efectiva
            transform.Translate(effectiveDirection * distanceCoveredThisFrame);

            // Y restamos esa cantidad (positiva) a la distancia que queda en el segmento
            distanceRemainingInSegment -= distanceCoveredThisFrame;
        }
    }
}