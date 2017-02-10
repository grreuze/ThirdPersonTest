using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {

    public Transform target;

    public float distance = 12, idealDistance = 12;
    public float xSpeed = 15, ySpeed = 15;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public bool canZoom;

    public float smooth = 4;

    Vector3 camPosition, negDistance;
    Quaternion camRotation;

    float x = 0.0f;
    float y = 0.0f;
    
    void Start() {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate() {
        if (target) {

            if (Physics.Linecast(transform.position, transform.position + transform.right) || Physics.Linecast(transform.position, transform.position - transform.right)) {
                distance -= Mathf.Abs(Input.GetAxis("Mouse X"));
            } else {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            }
            
            y -= Input.GetAxis("Mouse Y") * ySpeed * distance * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            if (canZoom) {
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            }

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit)) {
                distance -= hit.distance;
            }
            // Reset distance to Ideal Distance somehow??
            negDistance.z = -distance;
            Vector3 position = rotation * negDistance + target.position;

            camRotation = rotation;
            camPosition = position;

            smoothMovement();

            target.rotation = Quaternion.Euler(0, x, 0); // Reoriente the character's rotator
        }
    }

    void smoothMovement() {
        float t = Time.deltaTime * smooth;
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