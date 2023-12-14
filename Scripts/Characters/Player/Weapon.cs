using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    public class Weapon : MonoBehaviour
    {
        public Collider myCollider;

        private int damage;
        private float knockback;

        private List<Collider> alreadyColliderWith = new();
        private float attackCounter = 0;
        private float attackTimer = 0.5f;
        private bool isAttacking;

        private void OnTriggerEnter(Collider other)
        {
            if (other == myCollider) return;
            Debug.Log($"Trigger");
            if (alreadyColliderWith.Contains(other)) return;

            alreadyColliderWith.Add(other);

            if (!other.CompareTag("Enemy")) return;
            Debug.Log($"Damaged : {damage}");

            GetComponent<Collider>().enabled = false;
            other.GetComponent<EnemyStatusController>().TakeDamage(damage);

            // if(other.TryGetComponent(out ForceReceiver forceReceiver))
            // {
            //     var direction = (other.transform.position - myCollider.transform.position).normalized;
            //     forceReceiver.AddForce(direction * knockback);
            // }
        }

        public void SetAttack(int damage)
        {
            ClearCollider();
            this.damage = damage;
            GetComponent<Collider>().enabled = true;
        }

        public void InitAttack()
        {
            ClearCollider();
            GetComponent<Collider>().enabled = false;
        }

        private void ClearCollider()
        {
            alreadyColliderWith.Clear();
        }

        public void CheckColliderOnOff()
        {
            //대기 상태라면 타이머를 체크한다.
            if (isAttacking)
            {
                attackCounter += Time.deltaTime;
                if (attackCounter >= attackTimer)
                { isAttacking = false; attackCounter = 0f;
                }
            }
        }
    }
}