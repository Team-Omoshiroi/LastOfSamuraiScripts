using System.Collections.Generic;
using UnityEngine;

namespace Characters.Player
{
    public class EnemyWeapon : MonoBehaviour
    {
        [SerializeField] private Collider myCollider;

        private int damage;
        private float knockback;
        private float attackCounter = 0;
        private float attackTimer = 0.5f;
        public bool isAttacking = false;
        private Collider collider;

        private List<Collider> alreadyColliderWith = new List<Collider>();

        public int Damage { get { return damage; } set { damage = value; } }

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        private void Update()
        {
            CheckColiderOnOff();

            if (isAttacking)
            {
                collider.enabled = true;
            }
            else if (!isAttacking)
            {
                collider.enabled = false;
            }
        }

        private void OnEnable()
        {
            alreadyColliderWith.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == myCollider) return;
            if (alreadyColliderWith.Contains(other)) return;

            alreadyColliderWith.Add(other);

            if (other.CompareTag("Player"))
            {
                isAttacking = false;
                Debug.Log($"Damaged : {damage}");
                other.GetComponent<HealthModule>().TakeDamage(damage);
            }

            // if(other.TryGetComponent(out ForceReceiver forceReceiver))
            // {
            //     var direction = (other.transform.position - myCollider.transform.position).normalized;
            //     forceReceiver.AddForce(direction * knockback);
            // }
        }

        public void SetDamage(int damage)
        {
            this.damage = damage;
            // this.knockback = knockback;
        }

        private void SetAttack()
        {

        }

        public void CheckColiderOnOff()
        {
            //대기 상태라면 타이머를 체크한다.
            if (isAttacking)
            {
                attackCounter += Time.deltaTime;
                if (attackCounter >= attackTimer)
                {
                    isAttacking = false;
                    attackCounter = 0f;
                }
            }
            else
            {
                attackCounter = 0f;
            }
        }
    }
}