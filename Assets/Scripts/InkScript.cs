using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class InkScript : MonoBehaviour {
    public static event Action<Story> OnCreateStory;

	// serialized fields
	[SerializeField] GameObject rightHandController;
	[SerializeField] GameObject leftHandController;
	[SerializeField] private Canvas canvas = null;
	[SerializeField] private TextAsset inkJSONAsset = null;
	[SerializeField] private AudioSource _audioSource;
    [SerializeField] List<AudioClip> _audioClips;
	[SerializeField] private GameObject recruiterAvatar;
	[SerializeField] private GameObject lucaAvatar;
	[SerializeField] private OVRLipSyncContextMorphTarget avatarLucaLipSync;
	[SerializeField] private OVRLipSyncContextMorphTarget avatarRecruiterLipSync;
	[SerializeField] private Button testButton = null; // to test without connecting hmd
	[SerializeField] private ToTextFile logFile;
	[SerializeField] private GameObject fadeScreen;
	[SerializeField] private GameObject sendEmailCanvas;

	// UI Prefabs
	[SerializeField] private TextMeshProUGUI textPrefab = null;
	[SerializeField] private Button buttonPrefab = null;

	// public variables
	public Story story;
	GameObject buttonContainer = null;
	public Transform target;

	// private variables
	private string _currentLine;
	private GameObject _phone;
	private PhoneController _phoneController;
	private GameObject _sceneController;
	private Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
	private IsTalkingController _recruiterIsTalking;
	private IsTalkingController _lucaIsTalking;
	private FadeScreen _fadeSequence;

	// variables withtemp or conditional assignment
	string activeAvatarName = "";
	string targetName;

	// flags
	private bool _isPlaying = false;
	private bool _isStoryStopped;
	
    void Awake () {
		// Remove the default message
		// Get the GameObject this script is attached to
        _sceneController = gameObject;
		_audioSource = GetComponent<AudioSource>();
		target = GameObject.Find("FocusTarget").transform;
        _phone = GameObject.Find("Phone_White");
		_phoneController = GameObject.Find("Phone_White").GetComponent<PhoneController>();
		_recruiterIsTalking = recruiterAvatar.GetComponent<IsTalkingController>();
		_lucaIsTalking = lucaAvatar.GetComponent<IsTalkingController>();
		_fadeSequence = fadeScreen.GetComponent<FadeScreen>();

		TriggerRightHandButtonAction dispatcherRight = rightHandController.GetComponent<TriggerRightHandButtonAction>();
		TriggerLeftHandButtonAction dispatcherLeft = leftHandController.GetComponent<TriggerLeftHandButtonAction>();
		
		dispatcherRight.triggerEvent.AddListener(() => HandleTriggerEvent());
        dispatcherLeft.triggerEvent.AddListener(() => HandleTriggerEvent());
		
		InitializeAudioClips();
		RemoveChildren();
		StartStory();
	}

    private void HandleTriggerEvent() {
        if (_isStoryStopped) {
            _isStoryStopped = false;
            RefreshView();
        } else {
            if (_currentLine == "" || activeAvatarName != "Recruiter") return;
            logFile.AppendLine(_currentLine);
        }
    }

    private void InitializeAudioClips()
    {
        foreach(var clip in _audioClips)
		{
			_clips.Add(clip.name.ToLower(), clip);
		}
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory () {
		story = new Story (inkJSONAsset.text);
        if(OnCreateStory != null) OnCreateStory(story);
		RefreshView();

		// testButton.onClick.AddListener(delegate 
		// {
		// 	if (_currentLine == "") return;
		// 	logFile.AppendLine(_currentLine);
		// });
	}
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Remove all the UI on screen
		RemoveChildren ();
		
		if (_isPlaying || _isStoryStopped) return;

		// Read all the content until we can't continue any more
		while (story.canContinue) {
				if (_isPlaying) return;

			// Continue gets the next line of the story
			string text = story.Continue ();
			_currentLine = text;
			// This removes any white space from the text.
			text = text.Trim();

			float startOffset = 0.0f;
			bool foundLookAtTag = false; // Flag to track if "LookAt" tag is found
			// reset speaking avatar
			EnableSpeakingAvatar("");
			rightHandController.GetComponent<XRRayInteractor>().enabled = false;
			leftHandController.GetComponent<XRRayInteractor>().enabled = false;

			foreach(var tag in story.currentTags)
			{
				if (tag.StartsWith("Show")) {
					// Display the text on screen!
					CreateContentView(text);
				}

				if (tag.StartsWith("LookAt."))
				{
					foundLookAtTag = true; // Set flag to true if "LookAt" tag is found
					Debug.Log("luca is looked at" + story.currentTags);
					targetName = tag.Substring("LookAt.".Length, tag.Length - "LookAt.".Length);
       				if (targetName == "FirstPerson"){
						target.position = new Vector3(1.6f,1.052f,-3.9f);
					}
					if (targetName == "Phone") {
						target.position = new Vector3(2.526f,0.956f,-3.06f);
					}
				} 

				if (tag.StartsWith("StartOffset."))
				{
					targetName = tag.Substring("StartOffset.".Length, tag.Length - "StartOffset.".Length);
					float.TryParse(targetName, out float startOffsetFloat);
					startOffset = startOffsetFloat;
				}

				if (tag.StartsWith("Fade.")) {
					fadeScreen.SetActive(true);
					targetName = tag.Substring("Fade.".Length, tag.Length - "Fade.".Length);
					if (targetName == "Out")
					{
						StartCoroutine(_fadeSequence.FadeOut());
					}
					if (targetName == "In")
					{
						recruiterAvatar.SetActive(false);
						_phone.SetActive(false);
						StartCoroutine(_fadeSequence.FadeIn());
					}
				}

				if (tag.StartsWith("Phone.")) {
					targetName = tag.Substring("Phone.".Length, tag.Length - "Phone.".Length);
					if (targetName == "Start")
					{
						_phoneController.isIncomingCall = true;
					}
					if (targetName == "End")
					{
						_phoneController.isIncomingCall = false;
					}
				}

				if (tag.StartsWith("Stop"))
                {
                    _isStoryStopped = true;

					// TODO can be commented out for VR use
                    // Clear previous listeners to avoid multiple additions
                    testButton.onClick.RemoveAllListeners();
                    testButton.onClick.AddListener(delegate
                    {
                        _isStoryStopped = false;
                        RefreshView();
                    });

					

                    // Break out of the loop to prevent further processing until the button is clicked
                    return;
                }

				if (!foundLookAtTag)
				{
					target.position = new Vector3(2.82f,1.052f,-2.59f);
					foundLookAtTag = false;
				}

				if (tag.StartsWith("Avatar."))
				{
					activeAvatarName = tag.Substring("Avatar.".Length, tag.Length - "Avatar.".Length);
					EnableSpeakingAvatar(activeAvatarName);
				}

				if (tag.StartsWith("Clip."))
				{
					var clipName = tag.Substring("Clip.".Length, tag.Length - "Clip.".Length);
					_isPlaying = true;
					StartCoroutine(PlayClip(clipName, startOffset));
				}
			}
		}

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			// if there are choices, create a button container
			SetupButtonContainer();
			for (int i = 0; i < story.currentChoices.Count; i++) {
				Choice choice = story.currentChoices [i];
				// GameObject buttonContainer = new GameObject("ButtonContainer");
				// buttonContainer.transform.SetParent(canvas.transform, false);
				Button button = CreateChoiceView (choice.text.Trim ());
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton (choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			// enable interactor again
			rightHandController.GetComponent<XRRayInteractor>().enabled = true;
			leftHandController.GetComponent<XRRayInteractor>().enabled = true;
			
			sendEmailCanvas.SetActive(true);
			Button choice = CreateChoiceView("End of story.\nRestart?");
			choice.onClick.AddListener(delegate{
				StartStory();
			});
		}
	}

	private void SetupButtonContainer()
	{
		buttonContainer = new GameObject("ButtonContainer");
		buttonContainer.transform.SetParent(canvas.transform, false);
		// Ensure it has a HorizontalLayoutGroup component for proper layout
		HorizontalLayoutGroup containerLayoutGroup = buttonContainer.AddComponent<HorizontalLayoutGroup>();
		// containerLayoutGroup.childControlWidth = false;
		containerLayoutGroup.childForceExpandWidth = false;
		// containerLayoutGroup.controlChildSize = true;
		// containerLayoutGroup.spacing = 0.5f;
		containerLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
	}

	private void EnableSpeakingAvatar(string name)
	{
		if (name == "Luca")
			{
				_sceneController.transform.position = GameObject.FindWithTag("HeadLuca").transform.position;
				_audioSource.spatialBlend = 1.0f;	
				avatarLucaLipSync.enabled = true;
				avatarRecruiterLipSync.enabled = false;			
				_lucaIsTalking.isTalking = true;	
				_recruiterIsTalking.isTalking = false;

			} else if (name == "Recruiter") {
				_sceneController.transform.position = GameObject.FindWithTag("HeadRecruiter").transform.position;
				_audioSource.spatialBlend = 1.0f;	
				avatarLucaLipSync.enabled = false;
				avatarRecruiterLipSync.enabled = true;
				_recruiterIsTalking.isTalking = true;
				_lucaIsTalking.isTalking = false;	
		} else {
			avatarRecruiterLipSync.enabled = false;	
			_recruiterIsTalking.isTalking = false;
			avatarLucaLipSync.enabled = false;
			_lucaIsTalking.isTalking = false;
			_audioSource.spatialBlend = 0.0f;	
		}
	}

	private IEnumerator PlayClip(string name, float startOffset)
	{
		if (_clips.TryGetValue(name.ToLower(), out var clip))
		{
			_audioSource.clip = clip;
			yield return new WaitForSeconds(startOffset);
			_audioSource.Play();

			// Wait until the audio clip is finished playing
			yield return new WaitForSeconds(clip.length);
			_isPlaying = false;
			RefreshView();
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		RefreshView();
	}

	// Creates a textbox showing the the line of text
	void CreateContentView (string text) {
		TextMeshProUGUI storyText = Instantiate (textPrefab) as TextMeshProUGUI;
		storyText.text = text;
		storyText.transform.SetParent (canvas.transform, false);
	}

public string GetActiveAvatarName()
{
	return activeAvatarName;
}

public float GetRemainingClipDuration()
{
    // Ensure _audioSource is not null and has an assigned clip
    if (_audioSource != null && _audioSource.clip != null)
    {
        return _audioSource.clip.length - _audioSource.time;
    }
    return 0f;
}


Button CreateChoiceView(string text) {
	// active line visual
	rightHandController.GetComponent<XRRayInteractor>().enabled = true;
	leftHandController.GetComponent<XRRayInteractor>().enabled = true;

    // Creates the buttons from a prefab
    Button choice = Instantiate(buttonPrefab) as Button;
    choice.transform.SetParent(buttonContainer.transform, false);

    // Gets the text from the button prefab
    TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
    choiceText.text = text;

    // Make the button expand to fit the text
    HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
    layoutGroup.childForceExpandHeight = false;

    return choice;
}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (canvas.transform.GetChild (i).gameObject);
		}
	}
}
