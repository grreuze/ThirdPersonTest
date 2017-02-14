using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class ThirdPersonCamera : MonoBehaviour {

    public Transform target;

    public float distance = 12, idealDistance = 12;

    public Vector2 speed = new Vector2(15, 15);
    public MinMax pitchRotationLimit = new MinMax(-20, 80);

    public float yOffsetFar = 2, yOffsetClose = 0;

    public float reactionTime = 10;

    [Header("Zoom")]
    public bool canZoom;
    public float zoomSpeed = 5;
    public MinMax zoomDistance = new MinMax(2, 12);
    
    Vector3 camPosition, negDistance;
    Quaternion camRotation;

    float x, y;
    float maxDistance;
    
    void Start() {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        camPosition = transform.position;

        maxDistance = canZoom ? zoomDistance.max : idealDistance;
    }

    void LateUpdate() {
        if (target) {
            Vector3 position = transform.position;

            float clampedX = Mathf.Clamp(Input.GetAxis("Mouse X"), -10, 10); // Avoid going too fast (makes weird lerp)
            x += clampedX * speed.x * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * speed.y * distance * 0.02f;
            y = pitchRotationLimit.Clamp(y);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0) {
                idealDistance = zoomDistance.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
            }

            RaycastHit hit;
            if (Physics.Linecast(target.position, position, out hit)) {
                distance = hit.distance;
            } else {
                distance = Mathf.Lerp(distance, idealDistance, Time.deltaTime * reactionTime);
            }
            negDistance.z = -distance;

            float yOffset = Mathf.Lerp(yOffsetClose, yOffsetFar, distance / maxDistance);

            Vector3 newPosition = rotation * negDistance + target.position;


            camRotation = rotation;
            //camPosition = newPosition;
            camPosition = newPosition + transform.up * yOffset;


            smoothMovement();

            target.rotation = Quaternion.Euler(0, x, 0); // Reoriente the character's rotator
        }
    }

    void smoothMovement() {
        float t = Time.deltaTime * reactionTime;
        transform.position = Vector3.Lerp(transform.position, camPosition, t);
        transform.rotation = Quaternion.Lerp(transform.rotation, camRotation, t);
    }

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}