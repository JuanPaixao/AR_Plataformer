using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    private CharacterController _charController;
    public float rotationValue;
    public float forwardForce;
    public float speed;
    public float jumpForce, gravityForce;

    private Vector3 _direction = Vector3.zero;
    private Vector3 normalizeZeroFloor = Vector3.zero;
    private Transform _transformCamera;
    private Vector3 _forward, _moveCamForward, _movement;

    private Animator _animator;
    public bool isJumping, isGrounded, isMoving;
    public GameObject eggParticle, featherParticle, starParticle;
    public int objects;
    public Light starLight;

    public AudioClip eggSound, featherSound, starSound, winSound, loseSound, starAppearsSound, jumpSound;
    private AudioSource _audioSource;
    public AudioSource musicAudioSource;
    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _transformCamera = Camera.main.transform;
    }

    private void Update()
    {
        _moveCamForward = Vector3.Scale(_transformCamera.forward, new Vector3(1, 0, 1)).normalized;
        _movement = CrossPlatformInputManager.GetAxis("Vertical") * _moveCamForward + CrossPlatformInputManager.GetAxis("Horizontal") * _transformCamera.right; // x axis

        _direction.y -= gravityForce * Time.deltaTime;

        _charController.Move(_direction * Time.deltaTime);
        _charController.Move(_movement * speed * Time.deltaTime);

        if (_movement.magnitude > 1f)
        {
            _movement.Normalize();
        }
        _movement = transform.InverseTransformDirection(_movement);
        _movement = Vector3.ProjectOnPlane(_movement, normalizeZeroFloor);
        rotationValue = Mathf.Atan2(_movement.x, _movement.z);
        forwardForce = _movement.z;

        _charController.SimpleMove(Physics.gravity);
        float turnSpeed = Mathf.Lerp(180, 360, forwardForce);
        transform.Rotate(0, rotationValue * turnSpeed * Time.deltaTime, 0);

        if (forwardForce != 0)
        {
            isMoving = true;
            _animator.SetBool("isMoving", isMoving);
        }
        else
        {
            isMoving = false;
            _animator.SetBool("isMoving", isMoving);
        }

        if (_charController.isGrounded)
        {
            isJumping = false;
            _animator.SetBool("isJumping", isJumping);
        }
        else
        {
            isJumping = true;
            _animator.SetBool("isJumping", isJumping);
        }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (_charController.isGrounded)
            {
                _direction.y = jumpForce;
                _audioSource.PlayOneShot(jumpSound, 1);
            }

        }

    }

    //SimpleMove
    //  private void Update()
    // {
    //     _forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * speed;
    //     transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * rotationValue * Time.deltaTime, 0));

    //     _charController.Move(_forward * Time.deltaTime);
    //     _charController.SimpleMove(Physics.gravity);
    //     Debug.Log(Input.GetAxis("Vertical"));

    //     if (Input.GetAxis("Vertical") >= 0.1f)
    //     {
    //         isMoving = true;
    //         _animator.SetBool("isMoving", isMoving);
    //     }
    //     else
    //     {
    //         isMoving = false;
    //         _animator.SetBool("isMoving", isMoving);
    //     }
    //     if (Input.GetButton("Jump"))
    //     {
    //         if (_charController.isGrounded)
    //         {
    //             direction.y = jumpForce;
    //         }
    //         direction.y -= gravityForce * Time.deltaTime;
    //         _charController.Move(direction * Time.deltaTime);
    //     }
    //     if (_charController.isGrounded)
    //     {
    //         isJumping = false;
    //         _animator.SetBool("isJumping", isJumping);
    //     }
    //     else
    //     {
    //         isJumping = true;
    //         _animator.SetBool("isJumping", isJumping);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            musicAudioSource.Stop();
            _audioSource.PlayOneShot(loseSound, 1);
            ReloadLevel();
        }
        if (other.gameObject.CompareTag("Egg"))
        {
            Instantiate(eggParticle, other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            objects++;
            _audioSource.PlayOneShot(eggSound, 1);
        }
        if (other.gameObject.CompareTag("Feather"))
        {
            Instantiate(featherParticle, other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            objects++;
            _audioSource.PlayOneShot(featherSound, 1);
        }
        if (other.gameObject.CompareTag("Star"))
        {
            if (objects >= 17)
            {
                Instantiate(starParticle, other.gameObject.transform.position, Quaternion.identity);
                other.gameObject.SetActive(false);
                _audioSource.PlayOneShot(starSound, 1);
                _audioSource.PlayOneShot(winSound, 1);
                musicAudioSource.Stop();
                ReloadLevel();
            }
        }
        if (objects >= 17)
        {
            starLight.enabled = true;
            _audioSource.PlayOneShot(starAppearsSound, 1);
        }
    }
    private void ReloadLevel()
    {
        StartCoroutine(Restart());
    }
    private IEnumerator Restart()
    {
        yield return new WaitForSecondsRealtime(3.25f);
        SceneManager.LoadScene("Adventure");
    }
}
