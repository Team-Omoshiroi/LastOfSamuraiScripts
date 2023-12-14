using UnityEngine;

namespace Characters.Player
{
    public class EffectModule : MonoBehaviour
    {
        [SerializeField] private GameObject effect1;
        [SerializeField] private GameObject effect2;
        [SerializeField] private GameObject effect3_1;
        [SerializeField] private GameObject effect3_2;
        [SerializeField] private GameObject effect4;

        private float effect1_Time;
        private float effect2_Time;
        private float effect3_1_Time;
        private float effect3_2_Time;
        private float effect4_Time;

        [SerializeField] private float exitTime = 1.1f;

        public void ActivateEffect1()
        {
            effect1_Time = Time.time;
            effect1.SetActive(true);
        }

        public void ActivateEffect2()
        {
            effect2_Time = Time.time;
            effect2.SetActive(true);
        }

        public void ActivateEffect3_1()
        {
            effect3_1_Time = Time.time;
            effect3_1.SetActive(true);
        }
        
        public void ActivateEffect3_2()
        {
            effect3_2_Time = Time.time;
            effect3_2.SetActive(true);
        }

        public void ActivateEffect4()
        {
            effect4_Time = Time.time;
            effect4.SetActive(true);
        }

        private void Update()
        {
            if (Time.time - effect1_Time >= exitTime && effect1.activeInHierarchy)
            {
                effect1.SetActive(false);
            }
            if (Time.time - effect2_Time >= exitTime && effect2.activeInHierarchy)
            {
                effect2.SetActive(false);
            }
            if (Time.time - effect3_1_Time >= exitTime && effect3_1.activeInHierarchy)
            {
                effect3_1.SetActive(false);
            }
            if (Time.time - effect3_2_Time >= exitTime && effect3_2.activeInHierarchy)
            {
                effect3_2.SetActive(false);
            }
            if (Time.time - effect4_Time >= exitTime && effect4.activeInHierarchy)
            {
                effect4.SetActive(false);
            }
        }
    }
}