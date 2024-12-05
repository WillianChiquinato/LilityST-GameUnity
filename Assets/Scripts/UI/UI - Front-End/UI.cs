using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public RectTransform[] Materias;

    public void FlipToPage(int targetPage)
    {
        //Revisao mais para frente
        switch (targetPage)
        {
            case 0:
                Materias[0].SetAsLastSibling();
                break;
            case 1:
                Materias[1].SetAsLastSibling();
                break;
            case 2:
                Materias[2].SetAsLastSibling();
                break;
            case 3:
                Materias[3].SetAsLastSibling();
                break;
            case 4:
                Materias[4].SetAsLastSibling();
                break;
            default:
                Debug.Log("Nenhum dos itens");
                break;
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            _menu.SetActive(true);
        }
    }

}
