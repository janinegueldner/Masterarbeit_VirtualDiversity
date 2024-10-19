using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class EyeGazing : MonoBehaviour
{
    [SerializeField] private GameObject focusTarget;
    [SerializeField] private GameObject eyeTarget;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject leftEye;
    [SerializeField] private GameObject rightEye;

    public float yOffset;

    public float updateInterval = 2.0f; // Interval in seconds
    public float maxAngle = 10f; // Maximum angle in degrees

    private bool _focusTargetMoved = false;

    void Start()
    {
        // Start the coroutine to update the position at intervals
        StartCoroutine(UpdatePositionAtIntervals());
    }

    IEnumerator UpdatePositionAtIntervals()
    {
        while (true)
        {
            if (!_focusTargetMoved)
            {
                yield return new WaitForSeconds(updateInterval);

                // Enable LookAtConstraint
                leftEye.GetComponent<LookAtConstraint>().enabled = true;
                rightEye.GetComponent<LookAtConstraint>().enabled = true;

                // Update the position with a random angle
                UpdateEyeTargetPosition();
            }
            else
            {
                _focusTargetMoved = false;
            }
            yield return null;
        }
    }
    void UpdateEyeTargetPosition()
    {
        // Get positions of focus target and head
        Vector3 focusTargetPosition = focusTarget.transform.position;
        Vector3 headPosition = head.transform.position;

        // Calculate direction vector from head to focus target
        Vector3 direction = focusTargetPosition - headPosition;
        direction.y = 0f; // eye movement is horizontal

        // Randomize angle within range
        float angle = Random.Range(-maxAngle, maxAngle);

        // Calculate perpendicular vector
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 perpendicular = rotation * direction.normalized;

        // Calculate eye target position maintaining the same distance from head
        Vector3 eyeTargetPosition = headPosition + perpendicular.normalized * Vector3.Distance(headPosition, focusTargetPosition);

        // Add yOffset to the y position
        eyeTargetPosition.y += yOffset;

        // Set eye target position
        eyeTarget.transform.position = eyeTargetPosition;
    }
}
