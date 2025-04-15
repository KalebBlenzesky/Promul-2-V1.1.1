using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections.Generic;
using TMPro;
using System.Globalization;

public class Bow : MonoBehaviour
{
    [Header("Object")]
    public GameObject targetObject;
    public LineRenderer stringbow;
    private KeywordRecognizer keywordRecognizer;

    private Vector3 currentMiddlePos;
    private Vector3 targetMiddlePos;
    public float pullSpeed = 5f;


    [Header("Command Right and Left")]
    [SerializeField]
    private List<VoiceRightLeft> RightLeft = new List<VoiceRightLeft>();
    private Dictionary<string, int> rightLeftMappings = new Dictionary<string, int>();

    [Header("Command Up and Down")]
    [SerializeField]
    private List<VoiceUpDown> UpDown = new List<VoiceUpDown>();
    private Dictionary<string, int> upDownMappings = new Dictionary<string, int>();

    [Header("Command Reay and Fire")]
    [SerializeField]
    private List<Ready> ReadyCommand = new List<Ready>();
    private Dictionary<string, int> readyMappings = new Dictionary<string, int>();

    [SerializeField]
    private List<Ready> FireCommand = new List<Ready>();
    private Dictionary<string, int> fireMappings = new Dictionary<string, int>();

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

    private float rotationX = 0f;
    private float rotationY = 0f;

    private string selectedMicrophone;
    private List<string> availableMicrophones = new List<string>();

    private float lastVoiceTime;
    private bool isFading = false;

    void Start()
    {
        Subtitles.text = "";
        PopulateDropdown();
        InitializeVoiceRecognition();

        currentMiddlePos = stringbow.GetPosition(2);
        targetMiddlePos = stringbow.GetPosition(1);
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

        List<string> allKeywords = new List<string>(rightLeftMappings.Keys);
        allKeywords.AddRange(upDownMappings.Keys);
        allKeywords.AddRange(resetwords);
        allKeywords.AddRange(wordBad);

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

    void UpdateStringBow()
    {
        if (stringbow != null)
        {
            stringbow.positionCount = 3;
            stringbow.SetPosition(1, currentMiddlePos);
        }
    }

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
        else
        {
            ShowSubtitles("Sorry, I Don't Understand, Please Try Again");
        }
    }

    void Update()
    {
        targetObject.transform.rotation = Quaternion.Lerp(targetObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Update posisi tengah tali
        currentMiddlePos = Vector3.Lerp(currentMiddlePos, targetMiddlePos, Time.deltaTime * pullSpeed);
        UpdateStringBow();

        if (Time.time - lastVoiceTime > 3f && !isFading)
        {
            StartCoroutine(FadeOutSubtitles());
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
}