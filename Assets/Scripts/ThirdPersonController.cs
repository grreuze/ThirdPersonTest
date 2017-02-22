using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour {

    public Transform rotator; // Used to get the rotation of the camera

    [Header("Movement")]
    public float walkForce = 700;
    public float rotationSpeed = 12;

    [Header("Jump")]
    public int numberOfJumps = 1;
    public float jumpSpeed = .06f;
    public float gravity = .2f;
    public float maxFallingSpeed = .06f;

    float xForce, zForce;
    float jumpForce;
    Vector3 direction;
    Vector3 distToGround;
    
    new ThirdPersonCamera camera;

    CharacterController controller;
    GameObject platform;
    Rigidbody rb;
    
    float verticalVelocity;
    bool isGrounded;
    float reachedMaxSpeed;

    void Start() {
        camera = FindObjectOfType<ThirdPersonCamera>();

        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        distToGround.y = - (GetComponent<Collider>().bounds.extents.y + .1f);
        jumpForce = jumpSpeed;
    }

    void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Jump
    int jumpsRemaining;

    void Jump() {
        if (jumpsRemaining == numberOfJumps && !controller.isGrounded)
            jumpsRemaining--; // The first jump can only be done on the ground
        if (jumpsRemaining > 0) {
            jumpsRemaining--;
            verticalVelocity = jumpSpeed;
        }
    }
    #endregion

    #region Impact
    public float fallImpact = 8;
    public MinMax impact = new MinMax(0.7f, 3);

    void Impact() {
        float impactStrength = Mathf.Min(reachedMaxSpeed * fallImpact + impact.min, impact.max);
        camera.temporaryOffset = new Vector2(0, -impactStrength);
    }
    #endregion

    float deltaTime;
    void Update() {
        deltaTime = Time.deltaTime;

        reachedMaxSpeed = verticalVelocity <= -maxFallingSpeed ? reachedMaxSpeed + deltaTime : 0;

        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0) 
            Jump();

        else if (controller.isGrounded) {

            if (reachedMaxSpeed > 0)
                Impact();

            jumpsRemaining = numberOfJumps;
            verticalVelocity = -gravity * deltaTime;

        } else if (reachedMaxSpeed == 0) 
            verticalVelocity -= gravity * deltaTime;

        xForce = Input.GetAxis("Horizontal");
        zForce = Input.GetAxis("Vertical");
        if (xForce != 0 || zForce != 0) // If we are moving, rotate in the direction of the camera
            transform.rotation = Quaternion.Lerp(transform.rotation, rotator.rotation, deltaTime * rotationSpeed);
        
        direction.x = xForce * deltaTime;
        direction.y = verticalVelocity;
        direction.z = zForce * deltaTime;
        direction = transform.TransformDirection(direction) * walkForce;
        
        controller.Move(direction * deltaTime);

        isGrounded = Physics.Linecast(transform.position, transform.position + distToGround);
    }
}
