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
        [HideInInspector] public bool isWeaponSwitchActive = false;

        private Animator animator;
        private Weapon currentWeapon;
        private Weapon previousWeapon;
        private PlayerAnimationEventsController animatorEvents;
        private bool isWeaponEquipped = false;
            

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
            Weapon weapon = GetActiveWeapon();
            Debug.Log($"DMG {target.name}   {weapon.GetDamage()}");
        }

        public void TogglePrimaryWeapon()
        {
            if (!isWeaponSwitchActive)
            {
                StartCoroutine(GetTogglePrimaryWeapon());
            } else
            {
                Debug.Log("Weapon switch in progress");
            }
        }

        private IEnumerator GetTogglePrimaryWeapon()
        {
            OnCoroutineStart();
            if(!isWeaponEquipped)
            {
                SetCurrentWeapon(primaryWeapon, false, "twoHandSword", true);

                yield return new WaitForSeconds(0.5f);
                Debug.Log("Unsheath weapon done");
                currentWeapon.Spawn(playerRightHand);
                isWeaponEquipped = true;
            } else
            {
                // Unequip back to unarmed
                SetCurrentWeapon(defaultWeapon, true, "twoHandSword", false);
                yield return new WaitForSeconds(0.5f);
                DestroyPreviousWeaponInstance();
                isWeaponEquipped = false;
            }
            OnCoroutineEnd();
        }

        private void SetCurrentWeapon(Weapon weapon, bool destroyInstance, string animatiorVariableKey, bool animatiorVariableValue)
        {
            Debug.Log($"SetCurrentWeapon {weapon}");
            previousWeapon = currentWeapon;
            currentWeapon = weapon;
            animator.SetBool(animatiorVariableKey, animatiorVariableValue);
        }

        private void DestroyPreviousWeaponInstance()
        {
            previousWeapon.DestroyWeaponInstance();
            previousWeapon = null;
        }

        public void UnequipWeapon()
        {
            StartCoroutine("GetUnequipWeapon");
        }

        private IEnumerator GetUnequipWeapon()
        {
            // Unsheathe logic goes here
            OnCoroutineStart();
            Debug.Log($"Sheathing weapon {currentWeapon}");
            animator.SetBool("twoHandSword", false);
            // Simulating a delay before the method ends
            yield return new WaitForSeconds(0.5f);
            currentWeapon.DestroyWeaponInstance();
            isWeaponEquipped = false;
            OnCoroutineEnd();
        }

        private void SetActiveWeaponOnAwake()
        {
            TogglePrimaryWeapon();
            // Todo: Extend with unsheath primary weapon on awake -- update animator also
        }

        private void OnCoroutineStart()
        {
            isWeaponSwitchActive = true;
        }

        private void OnCoroutineEnd()
        {
            isWeaponSwitchActive = false;
        }
        private Weapon GetActiveWeapon()
        {
            return isWeaponEquipped ? currentWeapon : defaultWeapon;
        }

        #region Animation Events
        public void OnWeaponSwitch()
        {
            Debug.Log("weapon switch event");
        }
        #endregion
    }

}
