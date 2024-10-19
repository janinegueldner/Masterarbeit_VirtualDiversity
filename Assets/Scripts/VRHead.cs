using UnityEngine;
using UnityEngine.XR;

public class VRHead : MonoBehaviour
{
    public float radius = 0.5f; // The radius of the invisible circle
    public float fixedYPosition = 1.348f; // The fixed Y position of the camera

    private Vector3 centerPoint; // The initial position of the main camera

    void Start()
    {
        // Set the center point to the initial position of the main camera
        centerPoint = transform.position;
    }

    void Update()
    {
        // Get the rotation information of the VR headset (HMD)
        Quaternion vrRotation = InputTracking.GetLocalRotation(XRNode.CenterEye);

        // Convert the rotation to an angle on the Y-axis (horizontal)
        float angle = Mathf.Atan2(2 * (vrRotation.y * vrRotation.w + vrRotation.x * vrRotation.z), 1 - 2 * (vrRotation.y * vrRotation.y + vrRotation.x * vrRotation.x)) * Mathf.Rad2Deg;

        // Calculate the position on the circle based on the angle
        Vector3 newPosition = centerPoint + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

        // Adjust the camera position to maintain the fixed Y position
        newPosition.y = fixedYPosition;

        // Adjust the camera position
        transform.position = newPosition;
    }
}
