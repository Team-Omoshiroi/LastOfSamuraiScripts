using UnityEngine;

namespace Characters.Player
{
    public class EquipmentModule : MonoBehaviour
    {
        [SerializeField] private GameObject katanaHandPosition;
        [SerializeField] private GameObject katana;
        [SerializeField] private GameObject katanaPelvisPosition;

        private GameObject currentWeaponInHand;
        private GameObject currentWeaponInPelvis;

        private void Start()
        {
            currentWeaponInPelvis = Instantiate(katana, katanaPelvisPosition.transform);
        }

        public void UnsheatheKatana()
        {
            currentWeaponInHand = Instantiate(katana, katanaHandPosition.transform);
            Destroy(currentWeaponInPelvis);
        }

        public void SheatheKatana()
        {
            currentWeaponInPelvis = Instantiate(katana, katanaPelvisPosition.transform);
            Destroy(currentWeaponInHand);
        }

        // public void StartDealDamage()
        // {
        //     currentWeaponInHand.GetComponentInChildren<DamageDealer>().StartDealDamage();
        // }
        // public void EndDealDamage()
        // {
        //     currentWeaponInHand.GetComponentInChildren<DamageDealer>().EndDealDamage();
        // }
    }
}