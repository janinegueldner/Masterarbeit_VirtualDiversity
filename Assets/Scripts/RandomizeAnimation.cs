using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomizeAnimation : StateMachineBehaviour
{

    [SerializeField] private string avatarName;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime % 1 > 0.90f)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            Dictionary<string, float> talkingAnimations = new Dictionary<string, float>();

            // Iterate through the animations and add the ones with "talking" in their names to the dictionary
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.ToLower().Contains("talking"))
                {
                    talkingAnimations.Add(clip.name, (clip.length / 0.7f));
                }
            }
            float remainingAudioLength = GameObject.Find("Scene Controller").GetComponent<AudioClipInfo>().GetAudioDurationToEnd();

            // Select a random animation from dictionary
            int randomIndex = Random.Range(0, talkingAnimations.Count);
            string randomAnimation = talkingAnimations.Keys.ElementAt(randomIndex);
            float animationLength = talkingAnimations[randomAnimation];

            if (remainingAudioLength > animationLength && (GameObject.Find("Scene Controller").GetComponent<AudioClipInfo>().GetActiveAvatarName() == avatarName))
            {
                animator.SetInteger("TalkingAnimation", randomIndex);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // reset
        animator.SetInteger("TalkingAnimation", -1);
    }
}
