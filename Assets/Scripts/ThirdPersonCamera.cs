using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class ThirdPersonCamera : MonoBehaviour {
    
    [Header("Position")]
    public Transform target;
    public float distance = 12;
    public float yOffsetFar = 2, yOffsetClose = 0;

    [Header("Movement")]
    public Vector2 rotationSpeed = new Vector2(15, 15);
    public Vector2 mouseSpeedLimit = new Vector2(10, 10);
    public MinMax pitchRotationLimit = new MinMax(-20, 80);
    public bool smoothMovement = true;
    public float cameraSpeed = 10; // this name is bad

    #region Zoom
    [Header("Zoom")]
    public bool canZoom;
    public float zoomSpeed = 5;
    public MinMax zoomDistance = new MinMax(2, 12);

    void Zoom(float value) {
        if (value != 0) idealDistance = zoomDistance.Clamp(distance - value * zoomSpeed);
    }
    #endregion

    Vector3 camPosition, negDistance;
    Quaternion camRotation;

    float x, y;
    float maxDistance, idealDistance;
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        camPosition = transform.position;

        idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : idealDistance;
    }

    void OnApplicationFocus(bool hasFocus) {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (target) {
            Vector3 position = transform.position;

            float clampedX = Mathf.Clamp(Input.GetAxis("Mouse X"), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
            x += clampedX * rotationSpeed.x * distance * 0.02f;

            float clampedY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
            y -= clampedY * rotationSpeed.y * distance * 0.02f;
            y = pitchRotationLimit.Clamp(y);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));

            RaycastHit hit;
            if (Physics.Linecast(target.position, position, out hit) 
                && hit.transform.tag != "Player"
                && !hit.collider.isTrigger) {

                distance = hit.distance;
            } else {
                distance = Mathf.Lerp(distance, idealDistance, Time.deltaTime * cameraSpeed);
            }
            negDistance.z = -distance;

            float yOffset = Mathf.Lerp(yOffsetClose, yOffsetFar, distance / maxDistance);
            
            camRotation = rotation;
            camPosition = rotation * negDistance + target.position + transform.up * yOffset;
            
            SmoothMovement();

            target.rotation = Quaternion.Euler(0, x, 0); // Reoriente the character's rotator
        }
    }

    void SmoothMovement() {
        float t = smoothMovement ? Time.deltaTime * cameraSpeed : 1;
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