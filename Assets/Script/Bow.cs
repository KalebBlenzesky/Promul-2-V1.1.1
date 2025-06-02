using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections.Generic;
using TMPro;
using System.Globalization;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.UI;
using UnityEngine.Events;

public class Bow : MonoBehaviour
{
    [Header("Object")]
    public GameObject targetObject;
    public LineRenderer stringbow;
    private KeywordRecognizer keywordRecognizer;

    private Vector3 currentMiddlePos;
    private Vector3 targetMiddlePos;
    public float pullSpeed = 5f;

    private Vector3 defaultMiddlePos;
    private Vector3 pulledMiddlePos;
    public bool isPulling = false;

    [Header("Bow")]
    public GameObject arrowPrefabs;
    public Transform point;
    private GameObject currentArrow;
    private Vector3 pointDefaultPos;
    private Vector3 pointPulledPos;
    public float pointPullSpeed = 3f;
    public float maxPullDistance = 1f;

    [Header("Trajectory")]
    public LineRenderer trajectoryLine;
    public int trajectoryResolution = 30;
    public float forceMultiplier = 10f;
    private float currentForce = 0f;
    private float targetForce = 0f;
    public float forceLerpSpeed = 5f;

    [Header("Command Right and Left")]
    [SerializeField]
    private List<VoiceRightLeft> RightLeft = new List<VoiceRightLeft>();
    private Dictionary<string, int> rightLeftMappings = new Dictionary<string, int>();

    [Header("Command Up and Down")]
    [SerializeField]
    private List<VoiceUpDown> UpDown = new List<VoiceUpDown>();
    private Dictionary<string, int> upDownMappings = new Dictionary<string, int>();

    [Header("Command Ready and Fire")]
    [SerializeField]
    private List<Ready> ReadyCommand = new List<Ready>();
    private HashSet<string> readyCommands = new HashSet<string>();

    [Header("Ready and Fire Events")]
    public UnityEvent ReadyEvents;
    public UnityEvent FireEvents;

    [SerializeField]
    private List<Fire> FireCommand = new List<Fire>();
    private HashSet<string> fireCommands = new HashSet<string>();

    private string[] resetwords = { "reset", "restart" };

    [Header("Command Bad Words")]
    [SerializeField]
    private List<BadWordss> BadWord = new List<BadWordss>();
    private HashSet<string> wordBad = new HashSet<string>();

    [Header("Preference")]
    public TMP_Dropdown microphoneDropdown;
    public TextMeshProUGUI Subtitles;
    public string Say;
    public float rotationSpeed = 2f;
    private Quaternion targetRotation;

    [Header("UI")]
    public Slider forceSlider;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private string selectedMicrophone;
    private List<string> availableMicrophones = new List<string>();

    private float lastVoiceTime;
    private bool isFading = false;

    [Header ("Audio")]
    public List<AudioClip> readyAudioClips;
    public List<AudioClip> fireAudioClips;
    public AudioSource audioSource;


    void Start()
    {
        Subtitles.text = "";
        PopulateDropdown();
        InitializeVoiceRecognition();

        if (stringbow != null && stringbow.positionCount >= 3)
        {
            defaultMiddlePos = stringbow.GetPosition(1);
            pulledMiddlePos = new Vector3(1f, defaultMiddlePos.y, defaultMiddlePos.z);
            currentMiddlePos = defaultMiddlePos;
        }

        targetRotation = targetObject.transform.rotation;
        pointDefaultPos = point.localPosition;
        pointPulledPos = new Vector3(point.localPosition.x, point.localPosition.y, -1f);
    }


    void PopulateDropdown()
    {
        microphoneDropdown.ClearOptions();
        availableMicrophones = new List<string>(Microphone.devices);

        if (availableMicrophones.Count > 0)
        {
            microphoneDropdown.AddOptions(availableMicrophones);
            selectedMicrophone = availableMicrophones[0];
        }
        else
        {
            microphoneDropdown.AddOptions(new List<string> { "No Microphones Found" });
            selectedMicrophone = null;
        }

        microphoneDropdown.onValueChanged.AddListener(OnMicrophoneSelected);
    }

    void OnMicrophoneSelected(int index)
    {
        if (index >= 0 && index < availableMicrophones.Count)
        {
            selectedMicrophone = availableMicrophones[index];
            Debug.Log("Microphone switched to: " + selectedMicrophone);
        }
    }
    // ---= Recognition Commands =---
    // ---= Do not Change THis =---
    void InitializeVoiceRecognition()
    {
        rightLeftMappings.Clear();
        foreach (var vc in RightLeft)
        {
            rightLeftMappings[vc.command] = vc.multiplier;
        }

        upDownMappings.Clear();
        foreach (var vu in UpDown)
        {
            upDownMappings[vu.command] = vu.multiplier;
        }

        wordBad.Clear();
        foreach (var badWord in BadWord)
        {
            wordBad.Add(badWord.bad);
        }

        readyCommands.Clear();
        foreach (var r in ReadyCommand)
        {
            readyCommands.Add(r.command);
        }

        fireCommands.Clear();
        foreach (var f in FireCommand)
        {
            fireCommands.Add(f.command);
        }


        List<string> allKeywords = new List<string>(rightLeftMappings.Keys);
        allKeywords.AddRange(upDownMappings.Keys);
        allKeywords.AddRange(resetwords);
        allKeywords.AddRange(wordBad);
        allKeywords.AddRange(readyCommands);
        allKeywords.AddRange(fireCommands);

        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }

        keywordRecognizer = new KeywordRecognizer(allKeywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
        keywordRecognizer.Start();

        targetRotation = targetObject.transform.rotation;
    }

    // ---= Strings Thing =---
    void UpdateStringBow()
    {
        if (stringbow != null)
        {
            stringbow.positionCount = 3;
            stringbow.SetPosition(1, currentMiddlePos);
        }
    }

    // ---= Voice Commands =---
    // ---= You Can Change This =---
    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        Say = args.text;

        if (wordBad.Contains(Say))
        {
            ShowSubtitles("HEY DONT SAY THAT!!!");
        }
        else if (rightLeftMappings.TryGetValue(Say, out int rightLeftMultiplier))
        {
            rotationY += rightLeftMultiplier * -10f;
            targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            ShowSubtitles(Say);
        }
        else if (upDownMappings.TryGetValue(Say, out int upDownMultiplier))
        {
            rotationX += upDownMultiplier * -10f;
            targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            ShowSubtitles(Say);
        }
        else if (Array.Exists(resetwords, word => word == Say))
        {
            rotationX = 0;
            rotationY = 0;
            targetRotation = Quaternion.identity;
            ShowSubtitles(Say);
        }
        // ---= Ready Commands =---
        else if (readyCommands.Contains(Say))
        {
            isPulling = true;
            ReadyEvents?.Invoke();
            if (currentArrow == null)
            {
                currentArrow = Instantiate(arrowPrefabs, point.position, point.rotation);
                currentArrow.transform.SetParent(transform);
            }

            PlayRandomAudio(readyAudioClips);
            ShowSubtitles(Say);
        }
        // ---= Fire Commands =---
        else if (fireCommands.Contains(Say))
        {
            if (currentArrow != null)
            {
                FireEvents?.Invoke();
                Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
                float pullAmount = Vector3.Distance(currentMiddlePos, defaultMiddlePos) / maxPullDistance;
                pullAmount = Mathf.Clamp01(pullAmount);

                Arrow arrow = currentArrow.GetComponent<Arrow>();
                if (arrow != null)
                {
                    currentArrow.transform.SetParent(null);
                    Vector3 force = point.forward * (pullAmount * forceMultiplier);
                    arrow.Fire(force);
                }

                currentArrow = null;
            }

            isPulling = false;
            currentForce = 0f;

            PlayRandomAudio(fireAudioClips);
            ShowSubtitles(Say);
        }
        else
        {
            ShowSubtitles("Sorry, I Don't Understand, Please Try Again");
        }
    }

    void Update()
    {
        targetObject.transform.rotation = Quaternion.Lerp(targetObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        targetMiddlePos = isPulling ? pulledMiddlePos : defaultMiddlePos;

        currentMiddlePos = Vector3.Lerp(currentMiddlePos, targetMiddlePos, Time.deltaTime * pullSpeed);
        UpdateStringBow();

        if (Time.time - lastVoiceTime > 3f && !isFading)
        {
            StartCoroutine(FadeOutSubtitles());
        }

        Vector3 targetPointPos = isPulling ? pointPulledPos : pointDefaultPos;
        point.localPosition = Vector3.Lerp(point.localPosition, targetPointPos, Time.deltaTime * pointPullSpeed);

        if (currentArrow != null)
        {
            currentArrow.transform.position = Vector3.Lerp(currentArrow.transform.position, point.position, Time.deltaTime * pointPullSpeed);
        }

        if (currentArrow != null && isPulling)
        {
            float pullAmount = Vector3.Distance(currentMiddlePos, defaultMiddlePos) / maxPullDistance;
            pullAmount = Mathf.Clamp01(pullAmount);
            targetForce = pullAmount * forceMultiplier;

            currentForce = Mathf.Lerp(currentForce, targetForce, Time.deltaTime * forceLerpSpeed);
            Vector3 forceDirection = point.forward * currentForce;

            SimulateTrajectory(currentArrow.transform.position, forceDirection);
        }
        else
        {
            currentForce = 0f;
            trajectoryLine.positionCount = 0;
        }

        if (forceSlider != null)
        {
            forceSlider.value = currentForce;
        }
    }
    void SimulateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        trajectoryLine.positionCount = trajectoryResolution;

        Vector3 gravity = Physics.gravity;
        float timestep = 0.1f;
        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i * timestep;
            Vector3 point = startPosition + initialVelocity * t + 0.5f * gravity * t * t;
            trajectoryLine.SetPosition(i, point);
        }
    }


    void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }

    private void ShowSubtitles(string text)
    {
        Subtitles.text = ToTitleCase(text);
        Subtitles.alpha = 1f;
        lastVoiceTime = Time.time;
        isFading = false;
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator FadeOutSubtitles()
    {
        isFading = true;
        float duration = 1.5f;
        float startAlpha = Subtitles.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Subtitles.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        Subtitles.text = "";
        isFading = false;
    }

    private string ToTitleCase(string input)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(input);
    }

    public string[] GetAvailableMicrophones()
    {
        return Microphone.devices;
    }

    private void PlayRandomAudio(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0 || audioSource == null) return;

        int index = UnityEngine.Random.Range(0, clips.Count);
        audioSource.PlayOneShot(clips[index]);
    }
}