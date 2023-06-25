using UnityEngine;

namespace AdventureProto
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon")]
    public class Weapon : ScriptableObject
    {
        [Header("Weapon Details")]
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] bool unhanded;
        [SerializeField] float weaponDamage;
        [SerializeField] float weaponRange;
        [Header("Weapon Positioning")]
        [SerializeField] float weaponPositionX = 0.114f;
        [SerializeField] float weaponPositionY = 0.005f;
        [SerializeField] float weaponPositionZ = -1.463f;
        [SerializeField] float weaponRotationX = 1.776f;
        [SerializeField] float weaponRotationY = -9.346f;
        [SerializeField] float weaponRotationZ = 1.293f;

        private GameObject activeWeaponInstance;

        public void Spawn(GameObject hand)
        {
            if (weaponPrefab)
            {
                activeWeaponInstance = Instantiate(weaponPrefab);
                activeWeaponInstance.transform.parent = hand.transform;
                activeWeaponInstance.transform.localPosition = new Vector3(weaponPositionX, weaponPositionY, weaponPositionZ); // Adjust the position
                activeWeaponInstance.transform.localRotation = Quaternion.Euler(weaponRotationX, weaponRotationY, weaponRotationZ); // Adjust the rotation
            }
        }

        public void DestroyWeaponInstance()
        {
            Debug.Log($"Destroying weapon {activeWeaponInstance}");
            Destroy(activeWeaponInstance);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public bool GetUnhanded()
        {
            return unhanded;
        }
    }
}