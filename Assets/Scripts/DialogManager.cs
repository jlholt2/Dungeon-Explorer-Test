using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public bool dialogOn { get { return _dialogOn; } }

    private Queue<string> sentences;
    private bool _dialogOn;
    private bool canAdvanceText;
    private bool skipText;
    private bool endOfText;

    [SerializeField] private float textSpeed;
    [SerializeField] private TextMeshProUGUI dialogBoxText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject dialogBoxPanel;

    [Header("Dialog Box Position")]
    RectTransform rectTransform;
    [SerializeField] private float dialogBoxHeight; // the height of the dialog box from top to bottom
    [SerializeField] private float dialogBoxWidth; // the width of the dialog box
    [SerializeField] private float dialogBoxAppearTime; // the width of the dialog box

    Coroutine dialogSetCoroutine = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        sentences = new Queue<string>(); _dialogOn = false;
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        nameText.text = "";
        dialogBoxText.text = "";
        //rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;

        skipText = true;
        canAdvanceText = true;
    }

    private void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }

        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            if (!skipText)
            {
                skipText = true;
            }
            if (!canAdvanceText && endOfText)
            {
                canAdvanceText = true;
            }
        }
    } 

    public IEnumerator StartDialog(Dialog[] dialogQueue)
    {
        if (_dialogOn)
        {
            yield break;
        }
        else
        {
            _dialogOn = true;
        }
        //GameObject.Find("OWPlayer").GetComponent<OWPlayerController>().eventInteraction = false;
        //GameObject.Find("OWPlayer").GetComponent<OWPlayerInput>().acceptInputs = false;
        try
        {
            Debug.Log("Starting conversation with " + GetNames(dialogQueue));
        }
        catch (System.Exception)
        {
            Debug.LogError("ERROR: Dialog array is empty!");
            throw;
        }

        sentences.Clear();

        // Bring Dialog Box on screen
        float waitTime = 0;
        while (waitTime < dialogBoxAppearTime)
        {
            waitTime += Time.deltaTime;
            rectTransform.anchorMax = new Vector2(1, Mathf.Lerp(0, 0.4f, waitTime / dialogBoxAppearTime));
            //rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, dialogBoxWidth, waitTime / dialogBoxAppearTime), Mathf.Lerp(0, dialogBoxHeight, waitTime / dialogBoxAppearTime));
            yield return new WaitForEndOfFrame();
        }

        // type out message one character at a time
        // ^- this is done by typing out the whole message into a string to be displayed and then making later characters invisible

        for (int i = 0; i < dialogQueue.Length; i++)
        {
            foreach (string sentence in dialogQueue[i].sentences)
            {
                sentences.Enqueue(sentence);
            }

            while (sentences.Count > 0)
            {
                endOfText = false;
                nameText.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(dialogQueue[i].nameColor) + ">" + dialogQueue[i].name;
                nameText.alignment = GetTextAlignment(dialogQueue[i].nameAlignment);
                StartCoroutine(TypeLine(GetNextSentence()));
                //if(sentences.Count > 0)
                {
                    yield return new WaitUntil(() => canAdvanceText);
                }
            }
        }
        canAdvanceText = false;
        continueButton.gameObject.SetActive(false);

        // once dialog is complete, get rid of dialog box
        nameText.text = "";
        dialogBoxText.text = "";

        waitTime = 0;
        while (waitTime < dialogBoxAppearTime)
        {
            waitTime += Time.deltaTime;
            rectTransform.anchorMax = new Vector2(1, Mathf.Lerp(0.4f, 0, waitTime / dialogBoxAppearTime));
            //rectTransform.sizeDelta = new Vector2(Mathf.Lerp(dialogBoxWidth, 0, waitTime / dialogBoxAppearTime), Mathf.Lerp(dialogBoxHeight, 0, waitTime / dialogBoxAppearTime));
            yield return new WaitForEndOfFrame();
        }

        _dialogOn = false;

        if(dialogSetCoroutine != null)
        {
            dialogSetCoroutine = null;
        }

        yield return new WaitUntil(() => GameObject.Find("OWPlayer").GetComponent<OWPlayerLook>().CameraMatchesTarget());

        //GameObject.Find("OWPlayer").GetComponent<OWPlayerController>().eventInteraction = true;
        //GameObject.Find("OWPlayer").GetComponent<OWPlayerInput>().acceptInputs = true;

        GameManager.instance.advanceEvent = true;
    }

    private string GetNextSentence()
    {
        return sentences.Dequeue();
    }

    private IEnumerator TypeLine(string dialogLine)
    {
        canAdvanceText = false;
        skipText = false;
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true);
        string typeLine = "";

        for (int charIndex = 1; charIndex <= dialogLine.Length; charIndex++)
        {
            if (skipText)
            {
                charIndex = dialogLine.Length;
                skipText = false;
                canAdvanceText = false;
            }
            typeLine = dialogLine.Substring(0, charIndex);
            typeLine += "<color=#00000000>" + dialogLine.Substring(charIndex) + "</color>";
            float typeTime = 0;
            while (typeTime < textSpeed)
            {
                typeTime += 0.01f * Time.deltaTime;
                if (skipText)
                {
                    typeTime = textSpeed;
                    yield return new WaitForEndOfFrame();
                    canAdvanceText = false;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            dialogBoxText.text = typeLine;
        }
        endOfText = true;
        skipButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    public void OnClickContinueButton()
    {
        continueButton.gameObject.SetActive(false);
        canAdvanceText = true;
    }
    public void OnClickSkipButton()
    {
        skipText = true;
    }

    private string GetNames(Dialog[] dialogQueue)
    {
        List<string> names = new List<string>();
        try
        {
            for (int i = 0; i < dialogQueue.Length; i++)
            {
                if(names.Count < 1)
                {
                    names.Add(dialogQueue[i].name);
                }
                for (int j = 0; j < names.Count; j++)
                {
                    if (dialogQueue[i].name != names[j]) // this statement is to make sure that the currently checked name is not already in the names list
                    {
                        if (j == names.Count - 1)
                        {
                            names.Add(dialogQueue[i].name);
                        }
                    }
                    else
                    {
                        j = names.Count;
                    }
                }
            }
            string returnString = "";
            for (int i = 0; i < names.Count; i++)
            {
                if(i != 0)
                {
                    returnString += " & ";
                }
                returnString += names[i];
                if(i == names.Count - 1)
                {
                    returnString += ".";
                }
            }
            return returnString;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private TextAlignmentOptions GetTextAlignment(TextAlignment alignment)
    {
        if (alignment == TextAlignment.Right)
        {
            return TextAlignmentOptions.Right;
        }
        return TextAlignmentOptions.Left;
    }

    public void PlayDialogSet(DialogSet[] dialogSets)
    {
        StartCoroutine(StartDialogSet(dialogSets));
    }
    public void PlayDialog(Dialog[] dialogs)
    {
        StartCoroutine(StartDialog(dialogs));
    }

    public IEnumerator StartDialogSet(DialogSet[] dialogSets)
    {
        dialogSetCoroutine = null;
        foreach (DialogSet dialogSet in dialogSets)
        {
            dialogSetCoroutine = StartCoroutine(StartDialog(dialogSet.dialogs));
            while(dialogSetCoroutine != null)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
