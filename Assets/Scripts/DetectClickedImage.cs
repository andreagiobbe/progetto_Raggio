using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Script della Main Camera
public class DetectClickedImage : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !PauseMenu.isPaused)
            DetectImage();
    }

    // Da un click su un'immagine ricava l'immagine stessa
    // La rete neurale farà inferenza su quest'ultima
    void DetectImage()
    {
        // Controlliamo che il mouse sia sopra elementi UI
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Raycast a tutti gli elementi UI sotto al mouse
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        // Loop su tutti i risultati del raycast
        foreach (RaycastResult result in raycastResults)
        {
            GameObject clickedObject = result.gameObject;
         
            // Escludiamo i bottoni (vogliamo solo le immagini)
            if (clickedObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                continue;
            }

            // Se l'oggetto clickato ha una componente Image o RawImage 
            // chiamiamo la rete neurale per fare inferenza
            if (clickedObject.GetComponent<Image>() != null || clickedObject.GetComponent<RawImage>() != null)
            {
                GetComponent<GetInferenceOnClick>().GetInference(clickedObject);
            }
        }
    }   
}
