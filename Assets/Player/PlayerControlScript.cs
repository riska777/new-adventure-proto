using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace AdventureProto
{
    public class PlayerControlScript : MonoBehaviour
    {
        public Camera cam;
        public NavMeshAgent navMeshAgent;
        public GameObject player;
        public GameObject targetActionPointer;
        public GameObject companionPrefab;
        public GameObject testWeapon;
        public LayerMask LayerMaskWalkable;
        public LayerMask LayerMaskEnemies;
        public float attackRange = 2f; // Adjust this to your desired range
        public float companionOffset = -4f;

        private CapsuleCollider playerCollider;
        private Animator animator;
        private PlayerInputActions playerInputActions;
        private WeaponController weaponController;
        private PlayerAnimationEventsController animatorEvents;
        private Move moveController;

        private CombatTarget enemyTarget;
        private GameObject activeWeaponObject;
        private GameObject companionInstance;
        private NavMeshAgent companionNavMeshAgent;
        private bool isStanding;
        private bool isMoving;
        private bool isAttacking;
        private bool isRolling = false;

        private bool isPrimaryWeaponActive = false;

        private void Awake()
        {
            // Setup collider
            playerCollider = GetComponent<CapsuleCollider>();
            // Setup weaponController
            weaponController = GetComponent<WeaponController>();
            // Setup Animator, add AnimationEvents script
            animator = GetComponent<Animator>();
            animatorEvents = GetComponent<PlayerAnimationEventsController>();

            UtilMethods.CheckAnimator(animator);

            // Setup Player Input Actions
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
            playerInputActions.Player.Stand.performed += Stand;
            playerInputActions.Player.Stand.canceled += Stand;
            playerInputActions.Player.Action.performed += Action;
            playerInputActions.Player.Roll.performed += Roll;
            playerInputActions.Player.TogglePrimaryWeapon.performed += TogglePrimaryWeapon;

            // Listen for the animator's animation events.
            animatorEvents.OnDisableHitbox.AddListener(OnDisableHitbox);
            animatorEvents.OnEnableHitbox.AddListener(OnEnableHitbox);
            animatorEvents.OnRollEnd.AddListener(RollEnd);
            animatorEvents.OnAttackEnd.AddListener(AttackEnd);

            // Setup Player Weapon State
            //weaponController = GetComponent<PlayerWeaponController>();
            if (companionPrefab)
            {
                SpawnCompanion();
            }
        }

        private void Start()
        {

        }

        private void Update()
        {
            UpdateVelocity();
            UpdateLookDirection();
            CheckForAttacking();
        }

        #region Companion
        void SpawnCompanion()
        {
            // Perform a raycast to determine the ground height
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
            {
                // Get the hit point and adjust the spawn position accordingly
                Vector3 adjustedSpawnPosition = hit.point + Vector3.up * 0.1f; // Lift the character slightly above the ground
                // Instantiate the character prefab at the adjusted spawn position
                companionInstance = Instantiate(companionPrefab, adjustedSpawnPosition, player.transform.rotation);
                companionInstance.transform.position += getCompanionOffset();
                companionNavMeshAgent = companionInstance.GetComponent<NavMeshAgent>();
            }
        }

        Vector3 getCompanionOffset()
        {
            return new Vector3(companionOffset, 0f, companionOffset);
        }
        #endregion

        #region Movement and Actions

        private void MoveToCursor(NavMeshAgent navMeshAgentObj)
        {
            // Navigate player
            isAttacking = false;
            navMeshAgentObj.isStopped = false;
            Physics.Raycast(GetCamRay(), out RaycastHit hitPoint);
            navMeshAgentObj.SetDestination(hitPoint.point);

            // Navigate companion with Vector3 offset
            if (companionInstance)
            {
                Vector3 companionOffset = getCompanionOffset();
                companionNavMeshAgent.SetDestination(hitPoint.point - companionOffset);
            }
        }

        private void MoveToDest(Vector3 dest, NavMeshAgent navMeshAgentObj)
        {
            isAttacking = false;
            navMeshAgentObj.isStopped = false;
            navMeshAgentObj.SetDestination(dest);
        }

        private void CheckForAttacking()
        {
            if (!enemyTarget) return;

            bool isEnemyInRange = UtilMethods.IsObjectInRange(transform, enemyTarget.transform, attackRange);

            if (isEnemyInRange)
            {
                // Perform the attack
                navMeshAgent.isStopped = true;
                isAttacking = true;
                weaponController.Attack(enemyTarget);
                enemyTarget = null;
            }
            else
            {
                navMeshAgent.SetDestination(enemyTarget.transform.position);
            }
        }

        private void HandleClickAction()
        {
            Ray ray = GetCamRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMaskEnemies | LayerMaskWalkable))
            {
                // Check if the clicked object has an Enemy component
                CombatTarget enemy = hit.collider.GetComponent<CombatTarget>();
                bool isWalkable = hit.transform.gameObject.layer == 9;

                if (isWalkable && !isAttacking)
                {
                    targetActionPointer.transform.position = hit.point;
                    MoveToCursor(navMeshAgent);
                } 
                else if (enemy)
                {
                    enemyTarget = enemy;
                    bool isEnemyInRange = UtilMethods.IsObjectInRange(transform, enemyTarget.transform, attackRange);
                    Debug.Log($"Enemy {enemy} in range: {isEnemyInRange}");
                    if (isEnemyInRange && !isAttacking)
                    {
                        // Attack enemy
                        navMeshAgent.isStopped = true;
                        isAttacking = true;
                    }
                    else
                    {
                        if (!isAttacking)
                        {
                            MoveToDest(enemyTarget.transform.position, navMeshAgent);
                        }
                    }
                }
                else
                {
                    Debug.Log("nothing to do");
                }
            }
        }

        private void UpdateVelocity()
        {
            Vector3 velocity = navMeshAgent.velocity;
            float playerSpeed = velocity.magnitude;
            animator.SetFloat("speed", playerSpeed);
        }

        private void UpdateLookDirection()
        {
            // Rotate towards the target enemy
            if (isAttacking && enemyTarget)
            {
                UtilMethods.UpdateLookDirection(transform, enemyTarget.transform);
            }
        }

        public void Roll(InputAction.CallbackContext context)
        {
            if (!isRolling)
            {
                isRolling = true;
                MoveToCursor(navMeshAgent);
                animator.SetTrigger("rolling");
            }
            else
            {
                Debug.Log($"Rolling Not Possible   Rolling: {isRolling}");
            }
        }

        public void Action(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                HandleClickAction();
            }
        }

        public void Stand(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("stand start");
                isStanding = true;
            }

            if (context.canceled)
            {
                Debug.Log("stand stop");
                isStanding = false;
            }
        }

        public void TogglePrimaryWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                weaponController.TogglePrimaryWeapon();
            }
        }

        #endregion

        #region Weapon Handling

        #endregion

        #region Animation Event Callbacks
        public void OnEnableHitbox()
        {
            Debug.Log("Hitbox Enabled");
            playerCollider.enabled = true;
        }

        public void OnDisableHitbox()
        {
            Debug.Log("Hitbox Disabled");
            playerCollider.enabled = false;
        }

        public void RollEnd()
        {
            Debug.Log("Roll End");
            if (isRolling) { isRolling = false; }
        }

        public void AttackEnd()
        {
            Debug.Log("Attack End");
            if (isAttacking) { isAttacking = false; }
        }

        #endregion

        #region Trigger

        private void OnTriggerEnter(Collider collider)
        {
            // Implement TriggerEnter
        }
        private void OnTriggerExit(Collider collider)
        {
            // Implement TriggerExit
        }

        #endregion

        #region Misc

        private Ray GetCamRay()
        {
            return cam.ScreenPointToRay(Input.mousePosition);
        }

        private RaycastHit GetCamRayHit()
        {
            RaycastHit hit;
            Physics.Raycast(GetCamRay(), out hit);
            return hit;
        }

        #endregion
    }

}
