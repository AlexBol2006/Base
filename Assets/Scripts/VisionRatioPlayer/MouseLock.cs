using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody; // Referencia al objeto Player
    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Oculta y bloquea el cursor
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f); // Limita la rotación vertical

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Cámara vertical
        playerBody.Rotate(Vector3.up * mouseX); // Rotación horizontal del cuerpo
    }
}
