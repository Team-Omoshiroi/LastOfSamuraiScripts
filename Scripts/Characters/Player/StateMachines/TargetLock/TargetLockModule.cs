using UnityEngine;

namespace Characters.Player.StateMachines.TargetLock
{
    public class TargetLockModule : MonoBehaviour
    {
        public Transform currentTarget;

        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private Transform enemyTargetLocator;

        [Tooltip("StateDrivenMethod for Switching Cameras")]
        [SerializeField] private Animator cinemachineAnimator;

        [Header("Settings")]
        [SerializeField] private bool zeroVertLook;
        [SerializeField] private float noticeZone = 10;
        [SerializeField] private float lookAtSmoothing = 2;
        [Tooltip("Angle_Degree")]
        [SerializeField] private float maxNoticeAngle = 60;
        [SerializeField] private float crossHairScale = 0.1f;

        private Transform cam;
        private bool enemyLocked;
        private float currentYOffset;
        private Vector3 pos;

        [SerializeField] private CameraFollowModule cameraFollowModule;
        [SerializeField] private Transform lockOnCanvas;

        private void Start()
        {
            if (Camera.main != null) cam = Camera.main.transform;
            lockOnCanvas.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            cameraFollowModule.lockedTarget = enemyLocked;

            if (!enemyLocked) return;
            if(!TargetOnRange()) ResetTarget();
            LookAtTarget();
        }

        public void FoundTarget(){
            lockOnCanvas.gameObject.SetActive(true);
            cinemachineAnimator.Play("TargetCamera");
            enemyLocked = true;
        }

        public void ResetTarget()
        {
            lockOnCanvas.gameObject.SetActive(false);
            currentTarget = null;
            enemyLocked = false;
            cinemachineAnimator.Play("FollowCamera");
        }

        public Transform ScanNearBy()
        {
            var nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
            var closestAngle = maxNoticeAngle;
            Transform closestTarget = null;
            if (nearbyTargets.Length <= 0) return null;

            foreach (var target in nearbyTargets)
            {
                var dir = target.transform.position - cam.position;
                dir.y = 0;
                var angle = Vector3.Angle(cam.forward, dir);

                if (!(angle < closestAngle)) continue;
                closestTarget = target.transform;
                closestAngle = angle;
            }

            if (!closestTarget ) return null;
            var height = (closestTarget.GetComponent<BoxCollider>().size.y) * (closestTarget.localScale.y);
            var halfH = (height / 2) / 2;
            currentYOffset = height - halfH;
            if(zeroVertLook && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
            var tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
            
            return Blocked(tarPos) ? null : closestTarget;
        }

        private bool Blocked(Vector3 t)
        {
            if (!Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out var hit)) return false;
            return !hit.transform.CompareTag("Enemy");
        }

        private bool TargetOnRange()
        {
            var dis = (transform.position - pos).magnitude;
            return !(dis/2 > noticeZone);
        }

        private void LookAtTarget()
        {
            if(currentTarget == null) {
                ResetTarget();
                return;
            }

            var position = currentTarget.position;
            pos = position + new Vector3(0, currentYOffset, 0);
            
            lockOnCanvas.position = pos;
            lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHairScale);

            enemyTargetLocator.position = pos;
            var dir = position - transform.position;
            dir.y = 0;
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, noticeZone);   
        }
    }
}