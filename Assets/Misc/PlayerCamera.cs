using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform target;

    void Update()
    {
        if (target) { transform.position = target.position; }
    }
}
