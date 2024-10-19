using UnityEngine;
using System.Collections;

[System.Serializable]
public class MapTransforms
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void VRMapping()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class AvatarController : MonoBehaviour
{
    [SerializeField] private MapTransforms head;
    [SerializeField] private MapTransforms rightHand;
    [SerializeField] private MapTransforms leftHand;
    [SerializeField] private float turnSmoothness;
    [SerializeField] Transform ikHead;
    [SerializeField] Vector3 headBodyOffset;
    private bool _isUpdated = false;

    void Start()
    {
		StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        _isUpdated = true;
        yield return new WaitForSeconds(5);
    }

    private void LateUpdate()
    {
        if (!_isUpdated) return;

        // transform.position = ikHead.position + headBodyOffset;
        // transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(ikHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.VRMapping();
        leftHand.VRMapping();
        rightHand.VRMapping();
    }
}
