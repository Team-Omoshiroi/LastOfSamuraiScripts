using UnityEngine;

namespace Characters.Player
{
    public class CameraFollowModule : MonoBehaviour
    { 
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector2 clampAxis = new(60, 60);
    
        [SerializeField] private float followSmoothing = 5;
        [SerializeField] private float rotateSmoothing = 5;
        [SerializeField] private float sensitivity = 60;

        private float rotX, rotY;
        [SerializeField] private bool cursorLocked = false;
        private Transform cameraTransform;

        public bool lockedTarget;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            var targetPosition= target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSmoothing * Time.deltaTime);
        
            if(!lockedTarget) CameraTargetRotation(); else LookAtTarget();
        }

        private void CameraTargetRotation()
        {
            var mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            rotX += (mouseAxis.x * sensitivity) * Time.deltaTime;
            rotY -= (mouseAxis.y * sensitivity) * Time.deltaTime;

            rotY = Mathf.Clamp(rotY, clampAxis.x, clampAxis.y);

            var localRotation = Quaternion.Euler(rotY, rotX, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, localRotation, Time.deltaTime * rotateSmoothing);
        }

        private void LookAtTarget()
        {
            transform.rotation = cameraTransform.rotation;
            var rotation = cameraTransform.eulerAngles;
            rotX = rotation.y;
            rotY = 1.8f;
        }
    }
}