using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

// Script della Main Camera
public class ImagesManager : MonoBehaviour
{
    private string resourceImagesPath = "Datasets/BallsDataset/test/";   // path da cui prendere le immagini
    private readonly int numImages = 1; // numero di immagini da generare
    private GameObject imagesCanvas;    // Canvas dove posizionare le immagini
    public GameObject rawImagePrefab;   // Prefab delle immagini

    // Start is called before the first frame update
    void Start()
    {
        imagesCanvas = GameObject.Find("ImagesCanvas");
        CreateImages(numImages);
        RandomizeImages(resourceImagesPath, numImages);        
    }
    
    // Crea numImages istanze di RawImage sulla ImagesCanvas
    void CreateImages(int numImages)
    {
        float spacing = 20f; // distanza tra immagini adiacenti

        RectTransform canvasRect = imagesCanvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;      // larghezza della canvas
        float canvasHeight = canvasRect.rect.height;    // altezza della canvas

        // Calcola lo spazio occupato da tutte le immagini tenendo conto della distanza tra esse
        float totalSpacing = spacing * (numImages - 1);
        float totalImageWidth = Mathf.Min((canvasWidth - totalSpacing) / numImages, canvasHeight * 0.5f); // dimensione un'immagine
        float totalOccupiedWidth = totalImageWidth * numImages + totalSpacing; // larghezza complessiva di immagini + spacing

        // Posizione verticale delle immagini
        float yPos = canvasHeight *3 / 5;

        // Posizione orizzontale iniziale (per centrare le immagini)
        float startXPos = -(totalOccupiedWidth / 2) + totalImageWidth / 2;

        for (int i = 0; i < numImages; i++)
        {
            // Istanzia il prefab
            GameObject newImage = Instantiate(rawImagePrefab, imagesCanvas.transform);
            newImage.name = "Picture";
            newImage.GetComponentInChildren<TextMeshProUGUI>().text = newImage.name;
            RectTransform imageRect = newImage.GetComponent<RectTransform>();

            // Rendi l'immagine quadrata
            imageRect.sizeDelta = new Vector2(totalImageWidth, totalImageWidth);

            // Calcola la posizione x per ogni immagine
            float xPos = startXPos + i * (totalImageWidth + spacing);

            // Posizione delle anchor
            imageRect.anchoredPosition = new Vector2(xPos, yPos - (canvasHeight / 2));
        }

        Debug.Log(numImages.ToString() + " images have been created and centered!");
    }


    // Genera numImages nuove immagini a caso da imagesPath
    void RandomizeImages(string imagesPath, int numImages)
    {
        // Lista di tutte le immagini valide (con RawImage) su ImagesCanvas
        List<RawImage> children = new List<RawImage>();
        foreach (Transform child in imagesCanvas.transform)
        {
            RawImage rawImage = child.GetComponent<RawImage>();
            if (rawImage != null)
            {
                children.Add(rawImage);
            }
            else
            {
                Debug.LogWarning($"Il GameObject '{child.name}' non ha un componente RawImage e sarà ignorato.");
            }
        }

        // Debug per verificare il numero di RawImage trovate
        Debug.Log($"Numero di RawImage trovate: {children.Count}");

        if (children.Count == 0)
        {
            Debug.LogWarning("Nessuna RawImage trovata nella ImagesCanvas. Verifica la configurazione.");
            return;
        }

        // Lista delle immagini disponibili
        imagesPath = imagesPath.Replace("\\", "/").TrimEnd('/');
        string[] texturePaths = Directory.GetFiles("Assets/Resources/" + imagesPath, "*.*", SearchOption.AllDirectories);
        List<string> validExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
        List<string> validImagesPath = new List<string>();

        foreach (string imagePath in texturePaths)
        {
            if (validExtensions.Contains(Path.GetExtension(imagePath).ToLower()))
            {
                validImagesPath.Add(resourceImagesPath + Path.GetFileNameWithoutExtension(imagePath));
            }
        }

        if (validImagesPath.Count == 0)
        {
            Debug.LogWarning($"Nessuna immagine valida trovata nel percorso: {imagesPath}");
            return;
        }

        // Assicurati che numImages non sia maggiore del numero di immagini disponibili
        numImages = Mathf.Min(numImages, validImagesPath.Count);

        // Carica e assegna texture alle immagini
        for (int i = 0; i < numImages; i++)
        {
            int rndIdx = Random.Range(0, validImagesPath.Count);
            string assetPath = validImagesPath[rndIdx].Replace(Application.dataPath, "").Replace("\\", "/");
            validImagesPath.RemoveAt(rndIdx);

            Texture2D texture = Resources.Load<Texture2D>(assetPath);

            if (texture != null)
            {
                children[i].texture = texture;
            }
            else
            {
                Debug.LogWarning($"Impossibile caricare la texture dall'asset path: {assetPath}");
            }
        }

        Debug.Log($"{numImages} immagini randomizzate e assegnate!");
    }


    // Distrugge tutte le immagini figlie della ImagesCanvas
    // ------------------------------------------------------
    // A causa di un bug, in realtà rimuove solo le texture delle
    // immagini figlie di imagesCanvas.
    void DestroyTextures()
    {
        if (imagesCanvas == null)
        {
            Debug.LogWarning("ImagesCanvas non trovato. Verifica che esista e che sia correttamente assegnato.");
            return;
        }

        foreach (Transform child in imagesCanvas.transform)
        {
            if (child.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                // Verifica se il GameObject ha un componente RawImage
                RawImage rawImage = child.gameObject.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.texture = null; // Rimuovi la texture
                }
                else
                {
                    Debug.Log($"Il GameObject '{child.gameObject.name}' non ha un componente RawImage, potrebbe essere un altro elemento come un Dropdown.");
                }

                // Gestione del dialogo se necessario
                DialogueScriptOnClick dialogueScript = GetComponent<DialogueScriptOnClick>();
                if (dialogueScript != null)
                {
                    if (dialogueScript.HasAlreadyDialogueWindow(child.gameObject, "DialoguePanel"))
                    {
                        GameObject dialoguePanel = child.gameObject.transform.Find("DialoguePanel")?.gameObject;
                        if (dialoguePanel != null)
                        {
                            dialogueScript.CloseDialogue(dialoguePanel);
                        }
                    }
                }
            }
        }

        Debug.Log("Textures rimosse!");
    }

    /*
        void DestroyTextures()
        {
            /* 
            Provando a distruggere le immagini preesistenti con Destroy(child.gameObject) 
            portava a un bug: le immagini rimanevano grigie (senza texture) 
            sulla imagesCanvas, e la lista children all'interno di RandomizeImages pareva 
            pareva avesse 2*numImages elementi invece che numImages.

            foreach(Transform child in imagesCanvas.transform)
            {
                if (child.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    // Se non ho chiuso il dialogo, lo chiudo
                    DialogueScriptOnClick dialogueScript = GetComponent<DialogueScriptOnClick>();
                    if (dialogueScript.HasAlreadyDialogueWindow(child.gameObject, "DialoguePanel"))
                    {
                        dialogueScript.CloseDialogue(child.gameObject.transform.Find("DialoguePanel").gameObject);
                    }
                    child.gameObject.GetComponent<RawImage>().texture = null;   // rimozione texture

                    // Destroy(child.gameObject);   // bug
                }
            }

            Debug.Log("Textures rimosse!");
        }
    */

    // Pulsante di refresh
    public void RefreshImages()
    {
        DestroyTextures();
        RandomizeImages(resourceImagesPath, numImages);
    }
}
