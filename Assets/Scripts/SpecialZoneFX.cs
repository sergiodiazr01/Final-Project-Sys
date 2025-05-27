using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpecialZoneFX : MonoBehaviour
{
    public ParticleSystem entryEffect;
    public AudioClip entrySound;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            if (entryEffect != null)
                entryEffect.Play();

            if (entrySound != null && audioSource != null)
                audioSource.PlayOneShot(entrySound);

            Debug.Log("FX activado en zona: " + gameObject.name);
        }
    }
}
