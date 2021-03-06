﻿using UnityEngine;

[AddComponentMenu("Camera/Third Person Camera")]
public class ThirdPersonCamera : MonoBehaviour {
    
    [Header("Position")]
    public Transform target;
    public float distance = 12;
    public Vector2 offsetFar = new Vector2(0, 2),
                   offsetClose = new Vector2(2, 0);
    #region Temporary Offset
        Vector2 _tempOffset;
        bool offsetOverriden;
        public Vector2 temporaryOffset {
            set {
                _tempOffset = value;
                offsetOverriden = true;
            }
        }
    #endregion

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
    Vector2 offset;
    Transform my;

    float yaw, pitch;
    float maxDistance, idealDistance;
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        my = transform;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        camPosition = transform.position;

        idealDistance = distance;
        maxDistance = canZoom ? zoomDistance.max : idealDistance;
    }

    void OnApplicationFocus(bool hasFocus) {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        if (target) {
            float clampedX = Mathf.Clamp(Input.GetAxis("Mouse X") * (idealDistance/distance), -mouseSpeedLimit.x, mouseSpeedLimit.x); // Avoid going too fast (makes weird lerp)
            yaw += clampedX * rotationSpeed.x * distance * 0.02f;

            float clampedY = Mathf.Clamp(Input.GetAxis("Mouse Y") * (idealDistance/distance), -mouseSpeedLimit.y, mouseSpeedLimit.y); // Avoid going too fast (makes weird lerp)
            pitch -= clampedY * rotationSpeed.y * distance * 0.02f;
            pitch = pitchRotationLimit.Clamp(pitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            if (canZoom) Zoom(Input.GetAxis("Mouse ScrollWheel"));
            
            offset.x = Mathf.Lerp(offsetClose.x, offsetFar.x, distance / maxDistance);
            offset.y = Mathf.Lerp(offsetClose.y, offsetFar.y, distance / maxDistance);

            Vector3 targetPosition = target.position;

            if (offsetOverriden) {
                targetPosition.x += _tempOffset.x;
                targetPosition.y += _tempOffset.y;
                _tempOffset = Vector2.Lerp(_tempOffset, Vector2.zero, Time.deltaTime * cameraSpeed);
                if (Vector2.Distance(_tempOffset, Vector2.zero) < .01f)
                    offsetOverriden = false;
            }
            
            camRotation = rotation;

            CheckForCollision();

            negDistance.z = -distance;
            Vector3 targetWithOffset = targetPosition + my.right * offset.x + my.up * offset.y;
            camPosition = rotation * negDistance + targetWithOffset;

            //my.LookAt(targetWithOffset);

            SmoothMovement();

            target.rotation = Quaternion.Euler(0, yaw, 0); // Reoriente the character's rotator
        }
    }

    void CheckForCollision() {
        negDistance.z = -idealDistance;
        Vector3 idealPosition = camRotation * negDistance + target.position; // Virtually ideal position that does not take offset into account to avoid infinite back and forth

        RaycastHit hit;
        bool blockedByAWall = Physics.Linecast(target.position, idealPosition, out hit) 
                            && hit.transform.tag != "Player" && !hit.collider.isTrigger;

        float fixedDistance = blockedByAWall ? hit.distance : idealDistance;
        
        distance = Mathf.Lerp(distance, fixedDistance, Time.deltaTime * cameraSpeed);
    }

    void SmoothMovement() {
        float t = smoothMovement ? Time.deltaTime * cameraSpeed : 1;
        my.localPosition = Vector3.Slerp(my.localPosition, camPosition, t);
        my.localRotation = Quaternion.Slerp(my.localRotation, camRotation, t);
    }

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}