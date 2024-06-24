using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventário_Controller : MonoBehaviour
{
    [SerializeField]
    private UI_Inventário InventárioUI;

    public int TamanhoInventário = 10;

    void Start()
    {
        InventárioUI.InventoryUI(TamanhoInventário);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(InventárioUI.isActiveAndEnabled == false)
            {
                InventárioUI.show();
            }
            else
            {
                InventárioUI.hide();
            }
        }
    }
}
