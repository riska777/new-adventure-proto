using System.Collections;
using UnityEngine;

namespace AdventureProto
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("Weapon Models")]
        public GameObject twoHandSword;

        [Header("Debug Options")]

        [HideInInspector] public bool isWeaponSwitching = false;
        private Animator animator;
        private PlayerControlScript playerController;

        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            playerController = GetComponent<PlayerControlScript>();
        }

        public void Start()
        {
            // Listen for the animator's weapon switch event.
            var animatorEvents = animator.gameObject.GetComponent<PlayerAnimationEventsController>();
        }

        #region Weapons Management

        #endregion
    }

}
