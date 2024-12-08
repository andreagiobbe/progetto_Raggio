using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownDiagnosi : MonoBehaviour
{

    [SerializeField] private Dropdown dropdownDiagnosi;
    [SerializeField] private List<string> items = new List<string>(2);

    //public Text TextBox;
    // Start is called before the first frame update
    void Start()
    {
        dropdownDiagnosi.ClearOptions();

        //Aggiungo alla lista gli Items
        items.Add("Sana");
        items.Add("Malata");


        dropdownDiagnosi.AddOptions(items);

        //DropdownItemSelected(dropdown);

        //dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }
    /*
    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        TextBox.text = dropdown.options[index].text;
    }
    */
}
