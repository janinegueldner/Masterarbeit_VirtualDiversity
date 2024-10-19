using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EyeGazingController : MonoBehaviour
{
    [SerializeField] private GameObject focusTarget;
    [SerializeField] private GameObject leftEyeLuca;
    [SerializeField] private GameObject rightEyeLuca;
    [SerializeField] private GameObject leftEyeRecruiter;
    [SerializeField] private GameObject rightEyeRecruiter;

    // Update is called once per frame
    void Update()
    {
        // Check if the focus target has moved
        if (focusTarget.transform.hasChanged)
        {
            focusTarget.transform.hasChanged = false;

            // Disable LookAtConstraint immediately
            leftEyeLuca.GetComponent<LookAtConstraint>().enabled = false;
            rightEyeLuca.GetComponent<LookAtConstraint>().enabled = false;
            leftEyeRecruiter.GetComponent<LookAtConstraint>().enabled = false;
            rightEyeRecruiter.GetComponent<LookAtConstraint>().enabled = false;
        }
    }
}
