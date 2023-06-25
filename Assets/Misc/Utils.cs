using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureProto
{
    public class UtilMethods : MonoBehaviour
    {
        public static bool CheckAnimator(Animator animator)
        {
            if (!animator)
            {
                Debug.LogError("ERROR: THERE IS NO ANIMATOR COMPONENT ON CHILD OF CHARACTER.");
                Debug.Break();
                return false;
            }

            return true;
        }

        public static bool IsObjectInRange(Transform objectA, Transform objectB, float range)
        {
            return Vector3.Distance(objectA.position, objectB.position) <= range;
        }

        public static void UpdateLookDirection(Transform objectA, Transform objectB)
        {
            Vector3 direction = objectB.position - objectA.position;
            direction.y = 0f; // Keep the rotation in the horizontal plane
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                objectA.rotation = rotation;
            }
        }

        public static bool IsWeaponUnhanded(Weapon weapon)
        {
            return weapon.GetUnhanded();
        }
    }

}
