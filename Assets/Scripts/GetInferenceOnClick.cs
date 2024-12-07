using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

// Script della Main Camera 
public class GetInferenceOnClick : MonoBehaviour
{
    public NNModel onnxAsset;                       // rete neurale formato ONNX
    private DialogueScriptOnClick dialogueScript;   // riferimento allo script per scrivere risultato della predizione
    private RawImage imageToRecognise;              // immagine su cui fare predizione
    private IWorker worker;                         // worker per ONNX
    private string inference;                       // risultato della predizione
    private readonly string[] classes = {"beach ball", "rugby ball"};   // classi del dataset

    public void GetInference(GameObject clickedObject)
    {
        dialogueScript = GetComponent<DialogueScriptOnClick>();
        imageToRecognise = clickedObject.GetComponent<RawImage>();  // immagine di input è quella clickata

        // Instanziamo la rete neurale in formato ONNX
        try
        {
            using(var worker = onnxAsset.CreateWorker())
            {
                using(var input = new Tensor((Texture2D)(imageToRecognise.texture), channels: 3))
                {
                    var output = worker.Execute(input).PeekOutput();            // rete neurale processa l'input
                    inference = ComputeClass(output.ToReadOnlyArray());         // genera testo della predizione
                    dialogueScript.ShowDialogue(inference, imageToRecognise);   // crea dialogo per mostrare predizione
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Error during model execution " + e.Message);
        }
    }

    // Genera testo della predizione (classe e probabilità)
    string ComputeClass(float[] nnOutput)
    {
        float maxProb = Mathf.Max(nnOutput);
        int maxIndex = FindMaxIndex(nnOutput);

        return "Image was recognised as " + classes[maxIndex].ToString() +
         " with probability " + (maxProb*100).ToString() + "%"; 
    }

    // Trova indice della classe (probabilità massima)
    int FindMaxIndex(float[] arr)
    {
        int maxIdx = 0;

        for(int i = 0; i < arr.Length; i++)
        {
            if(arr[i] > arr[maxIdx])
                maxIdx = i;
        }

        return maxIdx;
    }
}
