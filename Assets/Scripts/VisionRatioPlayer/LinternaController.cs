using System.Collections;
using UnityEngine;

public class LinternaController : MonoBehaviour
{
    [Header("Carga")]
    public float maxCharge = 30f;
    public float currentCharge = 30f;
    public float drainRate = 1f;

    [Header("Recarga")]
    public float rechargeRate = 10f;
    public float overheatThreshold = 5f;
    public float cooldownDuration = 10f;
    private float chargeTime = 0f;
    private bool isOverheated = false;
    private float cooldownTimer = 0f;

    [Header("Flash")]
    public float flashRange = 10f;
    public float flashStunDuration = 3f;

    [Header("Referencias")]
    public Light flashlight;
    public AudioSource audioSource;
    public AudioClip manivelaSound;
    public AudioClip overheatSound;
    public AudioClip flashSound;

    void Update()
    {
        // Ajustar intensidad de la luz según la carga
        float normalizedCharge = currentCharge / maxCharge; // Valor entre 0 y 1
        flashlight.intensity = Mathf.Lerp(0.5f, 2f, normalizedCharge); // Ajusta los valores según tu gusto

        if (isOverheated)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                isOverheated = false;
                cooldownTimer = 0f;
            }
            flashlight.enabled = false;
            return;
        }

        // Activar linterna
        if (currentCharge > 0)
        {
            flashlight.enabled = true;
            currentCharge -= drainRate * Time.deltaTime;
        }
        else
        {
            flashlight.enabled = false;
        }

        // Recargar con manivela
        if (Input.GetKey(KeyCode.R))
        {
            chargeTime += Time.deltaTime;
            currentCharge += rechargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
            audioSource.clip = manivelaSound;
            if (!audioSource.isPlaying) audioSource.Play();

            if (chargeTime >= overheatThreshold)
            {
                TriggerOverheat();
            }
        }
        else
        {
            chargeTime = 0f;
            if (audioSource.clip == manivelaSound) audioSource.Stop();
        }

        // Activar flash
        if (Input.GetMouseButtonDown(1) && currentCharge >= maxCharge * 0.8f)
        {
            TriggerFlash();
            currentCharge = 0f;
        }
    }

    void TriggerOverheat()
    {
        isOverheated = true;
        audioSource.PlayOneShot(overheatSound);
        // Aquí puedes activar efectos visuales como vapor o parpadeo
    }

    void TriggerFlash()
    {
        currentCharge = 0f;
        StartCoroutine(FlashBurst());

        // Aturdir enemigos cercanos
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, flashRange);
        foreach (Collider enemy in hitEnemies)
        {
            //if (enemy.TryGetComponent(out EnemyAI ai))
            //{
            //    ai.Stun(flashStunDuration);
            //}
        }
    }

    IEnumerator FlashBurst()
    {
        flashlight.intensity = 40f; // Superluminosidad
        flashlight.spotAngle = 120f; // Ángulo más amplio para el destello
        audioSource.PlayOneShot(flashSound);

        // Efecto visual opcional: pantalla blanca, bloom, distorsión
        yield return new WaitForSeconds(0.5f); // Duración del destello

        flashlight.intensity = 0f;
        flashlight.spotAngle = 40f; // Restaurar ángulo normal
        flashlight.enabled = false;
    }
}
