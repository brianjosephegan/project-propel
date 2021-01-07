using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] float rotationThrust = 100f;
    [SerializeField] float upwardThrust = 100f;

    [SerializeField] ParticleSystem bubbleParticles;
    [SerializeField] AudioClip bubbleSFX;

    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] AudioClip explosionSFX;

    [SerializeField] ParticleSystem successParticles;
    [SerializeField] AudioClip successSFX;

    Rigidbody submarineRigidBody;
    AudioSource submarineAudioSource;

    bool isTransitioning;

    // Start is called before the first frame update
    void Start()
    {
        submarineRigidBody = GetComponent<Rigidbody>();
        submarineAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransitioning) { return; }

        RespondToThrustInput();
        RespondToRotateInput();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning) { return; }

        switch (collision.gameObject.tag)
        {
            case "Respawn":
                {
                    break;
                }
            case "Finish":
                {
                    StartSuccessSequence();
                    break;
                }
            default:
                {
                    StartDeathSequence();
                    break;
                }
        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        submarineAudioSource.Stop();
        bubbleParticles.Stop();
        submarineAudioSource.PlayOneShot(successSFX);
        successParticles.Play();
        FindObjectOfType<SceneLoader>().Invoke("LoadNextScene", 1f);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        submarineAudioSource.Stop();
        bubbleParticles.Stop();
        submarineAudioSource.PlayOneShot(explosionSFX);
        explosionParticles.Play();
        FindObjectOfType<SceneLoader>().Invoke("LoadCurrentScene", 1f);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = upwardThrust * Time.deltaTime;
        submarineRigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!bubbleParticles.isPlaying)
        {
            bubbleParticles.Play();
        }

        if (!submarineAudioSource.isPlaying)
        {
            submarineAudioSource.PlayOneShot(bubbleSFX);
        }
    }

    private void RespondToRotateInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateManually(rotationThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateManually(-rotationThrust * Time.deltaTime);
        }
    }

    private void StopApplyingThrust()
    {
        bubbleParticles.Stop();
        submarineAudioSource.Stop();
    }

    private void RotateManually(float rotationThisFrame)
    {
        submarineRigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        submarineRigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
