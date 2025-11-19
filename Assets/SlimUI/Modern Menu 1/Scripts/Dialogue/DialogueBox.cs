using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textScript;
    public string characterName;
    public string[] lines;
    public float textSpeed = 0.3f;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        if (textName == null || textScript == null)
        {
            Debug.LogError("TextMeshProUGUI components are not assigned.");
            return;
        }
        textName.text = characterName;
        textScript.text = string.Empty;
        StartDialogue();
    }

    public void LoadComponents(TextMeshProUGUI nameComponent, TextMeshProUGUI scriptComponent)
    {
        this.textName = nameComponent;
        this.textScript = scriptComponent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textScript.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textScript.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textScript.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textScript.text += "\n";
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
