using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RigidbodyParticleWind : MonoBehaviour
{
    public Transform obj;
    ParticleSystem particlesSystem;
    ParticleSystem.Particle[] particles;
    Rigidbody myRigidbody;

    void Start()
    {
        particlesSystem = gameObject.GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[1];
        myRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        particlesSystem.GetParticles(particles);

        myRigidbody.velocity += particles[0].velocity;
        particles[0].position = myRigidbody.position;
        particles[0].velocity = Vector3.zero;

        particlesSystem.SetParticles(particles, 1);
    }
}