using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;
using System;

public class DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textScript;
    public List<DialogueLine> dialogueLines;
    public bool isActive = true;
    public bool isFinished = false;
    public float textSpeed = 0.05f;

    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;
    public static event Action<Transform> OnDialogueStarted_HaveTarget;

    public Transform target;
    private int index;
    private AudioSource audioSource;
    [SerializeField] private bool isPaused = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    private void OnEnable()
    {
        PauseGameInput.Instance.OnGamePaused += OnPauseGame;
        PauseGameInput.Instance.OnGameResume += OnResumeGame;
        if (textName == null || textScript == null)
        {
            Debug.LogError("TextMeshProUGUI components are not assigned.");
            return;
        }
        StartDialogue();
        isFinished = false;
    }

    private void OnDisable()
    {
        Debug.Log("Set active false");
        PauseGameInput.Instance.OnGamePaused -= OnPauseGame;
        PauseGameInput.Instance.OnGameResume -= OnResumeGame;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (textScript.text == dialogueLines[index].dialogText)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textScript.text = dialogueLines[index].dialogText;
            }
        }
    }


    void StartDialogue()
    {
        index = 0;
        textName.text = string.Empty;
        textScript.text = string.Empty;
        OnDialogueStart?.Invoke();
        if (target != null)
        {
            OnDialogueStarted_HaveTarget?.Invoke(target);
        }
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (isPaused)
        {
            yield return null;
        }
        DialogueLine currentLine = dialogueLines[index];
        textName.text = currentLine.characterName;
        textScript.text = string.Empty;
        if (audioSource != null)
        {
            audioSource.Stop();
            if (currentLine.dialogVoice != null)
            {
                audioSource.PlayOneShot(currentLine.dialogVoice);
            }
        }

        foreach (char c in currentLine.dialogText.ToCharArray())
        {
            textScript.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    void NextLine()
    {
        if (index < dialogueLines.Count - 1)
        {
            index++;
            // textScript.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            OnDialogueEnd?.Invoke();
            isActive = false;
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            gameObject.SetActive(false);
            isFinished = true;
        }
    }
    public void LoadComponents(TextMeshProUGUI nameComponent, TextMeshProUGUI scriptComponent)
    {
        this.textName = nameComponent;
        this.textScript = scriptComponent;
    }

    public void LoadConversation(List<DialogueLine> newDialogueLines)
    {
        this.dialogueLines = newDialogueLines;
        gameObject.SetActive(true);
        StartDialogue();
    }

    public void OnPauseGame()
    {
        isPaused = true;
        if (audioSource != null)
        {
            audioSource.Pause();
        }

    }

    public void OnResumeGame()
    {
        isPaused = false;
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }
    
}