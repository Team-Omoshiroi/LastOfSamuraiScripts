using System;
using UnityEngine;

namespace Characters.Player
{
    public class HealthModule : MonoBehaviour
    {
        private Player player;
        [SerializeField] private int maxHealth = 100;
        public int health;
        public event Action OnDie;

        public bool IsDead => health == 0;

        private void Start()
        {
            player = GetComponent<Player>();
            health = maxHealth;

            OnDie += OnDieEvent;
        }

        public void TakeDamage(int damage)
        {
            if (health == 0) return;
            
            health = Mathf.Max(health - damage, 0);

            if (health == 0)
                OnDie?.Invoke();
            
            Debug.Log($"플레이어 체력 : {health}");
        }

        private void OnDieEvent()
        {
            player.Animator.SetTrigger("Die");
            enabled = false;
        }
    }
}
