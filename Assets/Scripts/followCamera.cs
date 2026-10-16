using System;
using UnityEngine;

public class followCamera : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Transform target;
    private void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
