using AudioSubsystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Audio Player/Play Audio On Collision")]
public class PlayAudioOnCollision : MonoBehaviour
{
    [SerializeField] private float collisionVelocityToPlay = 1f;
    [SerializeField] private string nameString = "";

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > collisionVelocityToPlay)
        {
            Audio.PlayAtPosition(nameString, collision.GetContact(0).point);
        }
    }
}
