using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureProto
{
    public class WeaponController : MonoBehaviour
    {
        public Weapon defaultWeapon;
        public Weapon primaryWeapon;
        public GameObject playerLeftHand;
        public GameObject playerRightHand;
        [HideInInspector] public bool IsCoroutineRunning = false;

        private Animator animator;
        private Weapon currentWeapon;
        private PlayerAnimationEventsController animatorEvents;
        private bool isUnsheathed = false;
            

        public enum WeaponType
        {
            Unarmed,
            TwoHandSword
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            animatorEvents = GetComponent<PlayerAnimationEventsController>();
            animatorEvents.OnWeaponSwitch.AddListener(OnWeaponSwitch);
            UtilMethods.CheckAnimator(animator);

            SetActiveWeaponOnAwake();
        }

        public void Attack (CombatTarget target)
        {
            animator.SetTrigger("attacking");
            Weapon weapon = isUnsheathed ? currentWeapon : defaultWeapon;
            Debug.Log($"DMG {target.name}   {weapon.GetDamage()}");
        }

        public void TogglePrimaryWeapon()
        {
 
        }

        public void UnsheathWeapon(WeaponType weaponType)
        {
            StartCoroutine(GetUnsheathWeapon(primaryWeapon));
        }

        private IEnumerator GetUnsheathWeapon(Weapon weapon)
        {
            OnCoroutineStart();
            // Unsheath logic goes here
            Debug.Log($"Unsheathing weapon {weapon}");
            animator.SetBool("twoHandSword", true);
            // Simulating a delay before the method ends
            currentWeapon = weapon;
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Unsheath weapon done");
            currentWeapon.Spawn(playerRightHand);
            isUnsheathed = true;
            OnCoroutineEnd();
        }

        public void SheathWeapon()
        {
            StartCoroutine("GetSheathWeapon");
        }

        private IEnumerator GetSheathWeapon()
        {
            // Unsheathe logic goes here
            OnCoroutineStart();
            Debug.Log($"Sheathing weapon {currentWeapon}");
            animator.SetBool("twoHandSword", false);
            // Simulating a delay before the method ends
            yield return new WaitForSeconds(0.5f);
            currentWeapon.DestroyWeaponInstance();
            isUnsheathed = false;
            OnCoroutineEnd();
        }

        private void SetActiveWeaponOnAwake()
        {
            TogglePrimaryWeapon();
            // Todo: Extend with unsheath primary weapon on awake -- update animator also
        }

        private void OnCoroutineStart()
        {
            IsCoroutineRunning = true;
        }

        private void OnCoroutineEnd()
        {
            IsCoroutineRunning = false;
        }

        #region Animation Events
        public void OnWeaponSwitch()
        {
            Debug.Log("weapon switch event");

        }
        #endregion
    }

}
