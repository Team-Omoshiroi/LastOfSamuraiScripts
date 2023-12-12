using System.Collections;
using UnityEngine;

namespace Characters.Player.StateMachines.TargetLock
{
    public class TargetLockIcon : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void OnEnable()
        {
            if (target == null && Camera.main != null)
            {
                target = Camera.main.transform;
            }
            
            StartCoroutine(LookAtTarget());
        }

        private IEnumerator LookAtTarget()
        {
            while(gameObject.activeInHierarchy)
            {
                var dir = target.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
                yield return null;
            }
        }
    }
}