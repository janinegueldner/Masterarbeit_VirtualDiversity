// Smooth towards the target

using UnityEngine;
using System.Collections;

public class MoveTowardGoal : MonoBehaviour
{
    public Transform target;
    public Transform head;

    // smooth damp speed 
    [Range(0,100)] public float speed;
    Vector3 _velocity = new Vector3(0, 0, 0);

    void Update()
    {
        head.position = Vector3.SmoothDamp(head.position, target.position, ref _velocity, Time.deltaTime * speed);    
    }
}