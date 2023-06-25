using UnityEngine;
using UnityEngine.Events;

namespace AdventureProto
{
    [HelpURL("https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html")]
    [System.Serializable]
    public class AnimatorMoveEvent : UnityEvent<Vector3, Quaternion> { }

    public class PlayerAnimationEventsController : MonoBehaviour
    {
        // Event call functions for Animation events.
        public AnimatorMoveEvent OnMove = new AnimatorMoveEvent();
        public UnityEvent OnDisableHitbox = new UnityEvent();
        public UnityEvent OnEnableHitbox = new UnityEvent();
        public UnityEvent OnWeaponSwitch = new UnityEvent();
        public UnityEvent OnRollEnd = new UnityEvent();
        public UnityEvent OnAttackEnd = new UnityEvent();
        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnShoot = new UnityEvent();
        public UnityEvent OnFootR = new UnityEvent();
        public UnityEvent OnFootL = new UnityEvent();
        public UnityEvent OnLand = new UnityEvent();

        // Components.
        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Hit() => OnHit.Invoke();
        public void Shoot() => OnShoot.Invoke();
        public void FootR() => OnFootR.Invoke();
        public void FootL() => OnFootL.Invoke();
        public void Land() => OnLand.Invoke();
        public void WeaponSwitch() => OnWeaponSwitch.Invoke();
        public void DisableHitbox() => OnDisableHitbox.Invoke();
        public void EnableHitbox() => OnEnableHitbox.Invoke();
        public void RollEndEvent() => OnRollEnd.Invoke();
        public void AttackEndEvent() => OnAttackEnd.Invoke();

        // Used for animations that contain root motion to drive the character’s
        // position and rotation using the “Motion” node of the animation file.
        void OnAnimatorMove()
        {
            if (!animator) { return; }
            OnMove.Invoke(animator.deltaPosition, animator.rootRotation);
        }
    }

}
