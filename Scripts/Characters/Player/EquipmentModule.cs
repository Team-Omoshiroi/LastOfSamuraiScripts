using System;
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

        public void Start()
        {
            currentWeaponInPelvis = Instantiate(katana, katanaPelvisPosition.transform);
            GetComponent<Player>().Weapon = currentWeaponInPelvis.GetComponent<Weapon>();
            
            currentWeaponInPelvis.GetComponent<Collider>().enabled = false;
            currentWeaponInPelvis.GetComponent<Weapon>().myCollider = GetComponent<Collider>();
        }

        public void UnsheatheKatana()
        {
            currentWeaponInHand = Instantiate(katana, katanaHandPosition.transform);
            GetComponent<Player>().Weapon = currentWeaponInHand.GetComponent<Weapon>();
            
            currentWeaponInHand.GetComponent<Collider>().enabled = false;
            currentWeaponInHand.GetComponent<Weapon>().myCollider = GetComponent<Collider>();
            
            Destroy(currentWeaponInPelvis);
        }

        public void SheatheKatana()
        {
            currentWeaponInPelvis = Instantiate(katana, katanaPelvisPosition.transform);
            GetComponent<Player>().Weapon = currentWeaponInPelvis.GetComponent<Weapon>();
            
            currentWeaponInPelvis.GetComponent<Collider>().enabled = false;
            currentWeaponInPelvis.GetComponent<Weapon>().myCollider = GetComponent<Collider>();
            
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