using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipInfo : MonoBehaviour
{
    public InkScript inkScript;

    void Start()
    {
        if (inkScript != null)
        {
            float remainingDuration = inkScript.GetRemainingClipDuration();
        }
        else
        {
            Debug.LogWarning("Script reference is not set!");
        }
    }

    void Update()
    {
        GetAudioDurationToEnd();
        GetActiveAvatarName();
    }

    public string GetActiveAvatarName()
    {
        if (inkScript != null)
        {
            return inkScript.GetActiveAvatarName();
        }
        else
        {
            Debug.LogWarning("Script reference is not set!");
            return "";
        }
    }

    public float GetAudioDurationToEnd()
    {
        if (inkScript != null)
        {
            float remainingDuration = inkScript.GetRemainingClipDuration();
            return remainingDuration;
        }
        else
        {
            Debug.LogWarning("Script reference is not set!");
            return 0.0f;
        }
    }
}
