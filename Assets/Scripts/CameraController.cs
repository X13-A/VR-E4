using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 70f; // Vitesse de déplacement de la caméra
    public float sensitivity = 4f; // Sensibilité de la souris

    private float yaw = 0.0f; // Rotation horizontale initiale
    private float pitch = 0.0f; // Rotation verticale initiale

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            return;

        // Mouvement avec les touches WASD/ZQSD
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float y = 0.0f; // Déplacement vertical initial

        // Gérer le mouvement vers le haut avec la touche espace
        if (Input.GetKey(KeyCode.Space))
        {
            y = speed * Time.deltaTime; // Déplacez la caméra vers le haut
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            y = -speed * Time.deltaTime; // Déplacez la caméra vers le bas
        }

        transform.Translate(x, y, z);
        
        // Rotation avec la souris
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limiter la rotation verticale pour éviter le retournement complet

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
