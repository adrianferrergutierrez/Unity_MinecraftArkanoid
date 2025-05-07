    using UnityEngine;
    using System.Collections.Generic; // Necesario para usar List

    public class GameManager : MonoBehaviour
    {
        public GameObject ballPrefab; 
        private List<GameObject> activeBalls = new List<GameObject>();

        // Referencia a la pala para saber d�nde spawnear la primera bola (opcional)
        private GameObject pala;

        // ** M�todo que se ejecuta al inicio del juego/escena **
        void Start()
        {
       
            pala = GameObject.FindGameObjectWithTag("Pala");
            Vector3 initialSpawnPosition = pala.transform.position + Vector3.forward * 2.5f + Vector3.up * 1.0f + Vector3.right * 0.25f;
            InstantiateNewBall(initialSpawnPosition); // Llama al m�todo auxiliar para crear y registrar la bola
        }

    
        public void AddBall(GameObject newBall)
        {
            if (newBall != null && !activeBalls.Contains(newBall))
            {
                activeBalls.Add(newBall);
            }
        }

        // M�todo para quitar una bola de la lista de seguimiento (cuando se destruye) (IGUAL QUE ANTES)
        public void RemoveBall(GameObject ballToRemove)
        {
            if (ballToRemove != null && activeBalls.Contains(ballToRemove))
            {
                activeBalls.Remove(ballToRemove);

                // ** L�gica de Fin de Juego si pierdes la �ltima bola **
                if (activeBalls.Count == 0)
                {
                    Debug.Log("��ltima bola perdida! Fin del juego (implementar l�gica aqu�).");
                    // Aqu� ir�a tu l�gica de Game Over, como mostrar un panel, reiniciar nivel, etc.
                }
            }
        }

        // M�todo llamado por el script de la manzana para activar la multibola (IGUAL QUE ANTES)
        public void ActivateMultiball()
        {
            Debug.LogError("�Holaaa!");

            List<Vector3> positionsToSpawn = new List<Vector3>();
            foreach (GameObject ball in activeBalls)
            {
                if (ball != null)
                {
                    positionsToSpawn.Add(ball.transform.position);
                }
            }

            if (positionsToSpawn.Count == 0)
            {
                Debug.LogWarning("Multibola activada, pero no se encontraron bolas existentes para duplicar.");
                return;
            }

            foreach (Vector3 spawnPos in positionsToSpawn)
            {
                InstantiateNewBall(spawnPos); // Reutilizamos el m�todo auxiliar
            }
        }

        // M�todo auxiliar para instanciar una sola bola y a�adirla al seguimiento (IGUAL QUE ANTES)
        private void InstantiateNewBall(Vector3 spawnPosition)
        {
            Debug.LogError("�Holaaa2!");

            if (ballPrefab != null)
            {
                GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
                AddBall(newBall);

                Rigidbody rb = newBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                   //   rb.AddForce(Vector3.,ForceMode.Impulse)
                }
            }
            else
            {
                Debug.LogError("�Prefab de Bola no asignado en el GameManager!");
            }
        }
    }