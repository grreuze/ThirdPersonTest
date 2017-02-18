using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour {

    public Transform rotator; // Used to get the rotation of the camera
    public float walkForce = 3, rotationSpeed = 12, hSpeed = 3, gravity = 10;

    float xForce, zForce, distToGround, falling;
    Vector3 direction;
    CharacterController controller;
    GameObject platform;

    bool isGrounded {
        get { return Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f); }
    }

    void Start() {
        controller = GetComponent<CharacterController>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        falling = 0;
    }

    void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update() {

        if (isGrounded) {
            falling = 0;
            if (Input.GetButton("Jump"))
                print("jump");
        }
        else {
            falling += Time.deltaTime;

        }

        xForce = Input.GetAxis("Horizontal") * walkForce;
        zForce = Input.GetAxis("Vertical") * walkForce;
        
        if (xForce + zForce != 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotator.rotation, Time.deltaTime * rotationSpeed);
        }

        direction = transform.TransformDirection(new Vector3(xForce * Time.deltaTime, 0, zForce * Time.deltaTime));
        direction.y -= gravity * Time.deltaTime;
        controller.Move(direction);
    }

}
