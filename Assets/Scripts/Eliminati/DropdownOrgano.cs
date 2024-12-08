using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownOrgano : MonoBehaviour
{

    [SerializeField] private Dropdown dropdownOrgano;
    [SerializeField] private List<string> items = new List<string>(7);

    //public Text TextBox;
    // Start is called before the first frame update
    void Start()
    {
        dropdownOrgano.ClearOptions();

        //Aggiungo alla lista gli Items
        items.Add("Colecisti");
        items.Add("Fegato");
        items.Add("Milza");
        items.Add("Pancreas");
        items.Add("Rene");
        items.Add("Tiroide");
        items.Add("Vescica");

        dropdownOrgano.AddOptions(items);

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
