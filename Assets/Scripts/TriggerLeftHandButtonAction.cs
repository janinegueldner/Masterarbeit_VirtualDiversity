using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TriggerLeftHandButtonAction : MonoBehaviour
{
    public InputActionProperty triggerButtonAction;
    public UnityEvent triggerEvent;
    private List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
    private bool devicesInitialized = false;
    private bool hasTriggeredOnce = false; // Flag to track if trigger value has changed from 0 to 1 and then back to 0

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the action is enabled
        triggerButtonAction.action.Enable();

        // Log the start to debug
        Debug.Log("TriggerButtonAction Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (!devicesInitialized)
        {
            // Initialize devices if not yet done
            UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, devices);

            if (devices.Count == 0)
            {
                Debug.LogWarning("No left-handed devices found. Retrying...");
                return; // Exit the update loop early and retry in the next frame
            }
            else
            {
                foreach (var device in devices)
                {
                    Debug.Log("Left-handed device found: " + device.name);
                    UnityEngine.XR.HapticCapabilities capabilities;
                    if (device.TryGetHapticCapabilities(out capabilities))
                    {
                        if (capabilities.supportsImpulse)
                        {
                            Debug.Log("Device supports haptics: " + device.name);
                        }
                        else
                        {
                            Debug.Log("Device does not support haptics: " + device.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to get haptic capabilities for device: " + device.name);
                    }
                }

                // Set the flag to true as devices are now initialized
                devicesInitialized = true;
            }
        }

        // Read the trigger value
        float triggerValue = triggerButtonAction.action.ReadValue<float>();
        Debug.Log("Trigger Value: " + triggerValue);

        // If the trigger value is above a threshold and has not triggered before, send haptic feedback
        if (triggerValue > 0.1f && !hasTriggeredOnce)
        {
            // event should be triggered to listen to in ink script
            triggerEvent.Invoke();

            foreach (var device in devices)
            {
                UnityEngine.XR.HapticCapabilities capabilities;
                if (device.TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        Debug.Log("Sending haptic impulse to device: " + device.name);
                        uint channel = 0;
                        float amplitude = 0.5f;
                        float duration = 1.0f;
                        device.SendHapticImpulse(channel, amplitude, duration);
                    }
                    else
                    {
                        Debug.Log("Device does not support impulse: " + device.name);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get haptic capabilities for device: " + device.name);
                }
            }

            // Set the flag to true to indicate that trigger has occurred once
            hasTriggeredOnce = true;
        }

        // If the trigger value goes back to 0, reset the flag
        if (triggerValue <= 0.1f)
        {
            hasTriggeredOnce = false;
        }
    }
}
