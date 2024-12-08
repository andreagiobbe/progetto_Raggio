using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;

public class DropdownConcatenator : MonoBehaviour
{
    public TMP_Dropdown dropDownOrgano;
    public TMP_Dropdown dropDownDiagnosi;
    public Button verificaButton;
    public NNModel onnxAsset;                       // rete neurale formato ONNX
    private DialogueScriptOnClick dialogueScript;   // riferimento allo script per scrivere risultato della predizione
    private RawImage imageToRecognise;              // immagine su cui fare predizione
    private IWorker worker;                         // worker per ONNX
    private string inference;                       // risultato della predizione
    private readonly string[] classes = {"Colecisti Malata", "Colecisti Sana", "Fegato Sana",
        "Milza Malata", "Milza Sana", "Pancreas Sana", "Rene Malata",
        "Rene Sana", "Tiroide Sana", "Vescica Malata", "Vescica Sana"};   // classi del dataset
    public string risultato;

    void Start()
    {
        // Assicura che il bottone sia collegato all'evento
        verificaButton.onClick.AddListener(ConcatenateDropdownValues);
    }

    void ConcatenateDropdownValues()
    {
        // Ottiene il testo selezionato dai dropdown
        string organo = dropDownOrgano.options[dropDownOrgano.value].text;
        string diagnosi = dropDownDiagnosi.options[dropDownDiagnosi.value].text;

        // Concatena le stringhe con uno spazio
        risultato = organo + " " + diagnosi;

        // Stampa il risultato nella console di Unity
        Debug.Log("Risultato concatenato: " + risultato);

        // Trova il GameObject "Picture" e assegna la sua RawImage
        GameObject pictureObject = GameObject.Find("Picture");

        if (pictureObject != null)
        {
            dialogueScript = GetComponent<DialogueScriptOnClick>();
            imageToRecognise = pictureObject.GetComponent<RawImage>();
            if (imageToRecognise == null)
            {
                Debug.LogError("Il GameObject 'Picture' non ha un componente RawImage!");
            }
        }
        else
        {
            Debug.LogError("GameObject 'Picture' non trovato nella scena!");
        }

        
        // Instanziamo la rete neurale in formato ONNX
        try
        {
            using (var worker = onnxAsset.CreateWorker())
            {
                using (var input = new Tensor((Texture2D)(imageToRecognise.texture), channels: 3))
                {
                    var output = worker.Execute(input).PeekOutput();            // rete neurale processa l'input
                    inference = ComputeClass(output.ToReadOnlyArray());         // genera testo della predizione
                    dialogueScript.ShowDialogue(inference, imageToRecognise);   // crea dialogo per mostrare predizione
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error during model execution " + e.Message);
        }
        
    }

    // Genera testo della predizione (classe e probabilità)
    string ComputeClass(float[] nnOutput)
    {
        float maxProb = Mathf.Max(nnOutput);
        int maxIndex = FindMaxIndex(nnOutput);

        if (classes[maxIndex] == risultato)
        {
            return "BRAVO!! l'immagine con probabilità " + (maxProb * 100).ToString()
                + "% è l'organo seguente: " + classes[maxIndex].ToString();
        }
        else

            return "RIPROVA!! l'immagine con probabilità " + (maxProb * 100).ToString()
                    + "% è l'organo seguente: " + classes[maxIndex].ToString();
    }

    // Trova indice della classe (probabilità massima)
    int FindMaxIndex(float[] arr)
    {
        int maxIdx = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > arr[maxIdx])
                maxIdx = i;
        }

        return maxIdx;
    }
}
