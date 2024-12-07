using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Script della Main Camera
public class DialogueScriptOnClick : MonoBehaviour
{
    public GameObject dialogueWindowPrefab; // prefab della finestra di dialogo
    private TextMeshProUGUI dialogueText;   // testo di dialogo

    public void ShowDialogue(string modelInference, RawImage imageToRecognise)
    {
        // Istanzia la finestra di dialogo se non presente
        if(HasAlreadyDialogueWindow(imageToRecognise.gameObject, "DialoguePanel"))
            return;

        GameObject dialogueWindow = Instantiate(dialogueWindowPrefab, imageToRecognise.transform);
        dialogueWindow.name = "DialoguePanel";  // elimina la scritta "(Clone)" nel nome del prefab istanziato

        // Trova il componente TextMeshProUGUI e lo aggiorna
        dialogueText = dialogueWindow.GetComponentInChildren<TextMeshProUGUI>();
        if(dialogueText != null)
            dialogueText.text = modelInference;
        else Debug.LogError("DialogueText component not found!");

        // Pulsante di chiusura dialogo
        Button closeButton = dialogueWindow.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => CloseDialogue(dialogueWindow));
        }
    }

    // Chiudere la finestra di dialogo
    public void CloseDialogue(GameObject dialogueWindow)
    {
        Destroy(dialogueWindow); // distrugge la finestra di dialogo
    }

    // Controlla se è già stata creata una finestra di dialogo
    public bool HasAlreadyDialogueWindow(GameObject parent, string prefabName)
    {
        foreach(Transform child in parent.transform)
        {
            if(child.gameObject.name == prefabName)
                return true;
        }

        return false;
    }
}
