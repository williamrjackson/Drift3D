using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {

    public Transform target;
    public float followSmoothTime = 0.3F;
    public float TurnSmoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, followSmoothTime);
        Vector3 targetLookRot = target.position - transform.position;
        if (targetLookRot != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSmoothTime * Time.deltaTime);
        }
    }
}
