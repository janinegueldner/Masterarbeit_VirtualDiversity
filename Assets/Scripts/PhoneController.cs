using UnityEngine;

public class PhoneController : MonoBehaviour
{
    // Reference to the Screen child GameObject's MeshRenderer component
    public GameObject screen;

    // Materials for incoming call and no call
    public Material incomingCallMaterial;
    public Material defaultMaterial;
    public AudioSource source;
    public AudioClip clip;

    public bool isIncomingCall = false;

    private bool _isAudioPlaying = false;

    // Class-level variable to hold the MeshRenderer component
    private MeshRenderer meshRenderer;

    void Start() 
    {
        // Get the MeshRenderer component from the target object and assign it to the class-level variable
        meshRenderer = screen.GetComponent<MeshRenderer>();

        // Set the initial material
        meshRenderer.material = defaultMaterial;
    }

    void Update()
    {
        meshRenderer.material = isIncomingCall ? incomingCallMaterial : defaultMaterial;
        if (!isIncomingCall) {
            if (!_isAudioPlaying) return;
            stopAudio();
            return;
        };
        if (_isAudioPlaying) return;

        startAudio();
    }

    void stopAudio()
    {
        _isAudioPlaying = false;
        source.Stop();
    }

    void startAudio()
    {
        _isAudioPlaying = true;
        source.PlayOneShot(clip);
    }
}
