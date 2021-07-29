using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Audio/Play Audio On Collision")]
public class PlayAudioOnCollision : MonoBehaviour
{
    [SerializeField] float collisionSpeedToPlay = 2f;
    [SerializeField] AudioClip[] collisionClips;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > collisionSpeedToPlay)
        {
            audioSource.PlayOneShot(collisionClips.GetRandom());
        }
    }
}