using UnityEngine;
using UnityEngine.SceneManagement;

public class FPController : MonoBehaviour {

    public Transform rotator; // Used to get the rotation of the camera
    public float walkForce = 3, runForce = 6, hSpeed = 3, gravity = 10, fallToDeath = 3;

    float xForce, zForce, distToGround, falling;
    Vector3 direction;
    CharacterController controller;
    GameObject platform;

    bool isGrounded {
        get { return Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f); }
    }
    float force {
        get {
            if (Input.GetMouseButton(1)) {
                Cursor.lockState = CursorLockMode.Locked;
                return runForce;
            } else return walkForce;
        }
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
            if (falling >= fallToDeath) ReloadScene();
        }

        xForce = Input.GetAxis("Horizontal") * force;
        zForce = Input.GetAxis("Vertical") * force;
        
        if (xForce + zForce != 0) {
            transform.rotation = rotator.rotation;
        }

        direction = transform.TransformDirection(new Vector3(xForce * Time.deltaTime, 0, zForce * Time.deltaTime));
        direction.y -= gravity * Time.deltaTime;
        controller.Move(direction);
    }

}
