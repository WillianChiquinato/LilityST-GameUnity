using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class UI : MonoBehaviour
{
    public RectTransform[] Materias;

    public void CervinhoOnTrigger()
    {
        GameManager.instance.cervinhoOnCheckpoint = true;
        GameManager.instance.UISavePoint.SetActive(false);
    }

    public void FlipToPage(int targetPage)
    {
        //Revisao mais para frente
        switch (targetPage)
        {
            case 0:
                Materias[0].SetAsLastSibling();
                Materias[1].gameObject.SetActive(false);
                Materias[2].gameObject.SetActive(false);
                Materias[3].gameObject.SetActive(false);
                Materias[4].gameObject.SetActive(false);

                Materias[0].gameObject.SetActive(true);
                break;
            case 1:
                Materias[1].SetAsLastSibling();
                Materias[0].gameObject.SetActive(false);
                Materias[2].gameObject.SetActive(false);
                Materias[3].gameObject.SetActive(false);
                Materias[4].gameObject.SetActive(false);

                Materias[1].gameObject.SetActive(true);
                break;
            case 2:
                Materias[2].SetAsLastSibling();
                Materias[0].gameObject.SetActive(false);
                Materias[1].gameObject.SetActive(false);
                Materias[3].gameObject.SetActive(false);
                Materias[4].gameObject.SetActive(false);

                Materias[2].gameObject.SetActive(true);
                break;
            case 3:
                Materias[3].SetAsLastSibling();
                Materias[0].gameObject.SetActive(false);
                Materias[1].gameObject.SetActive(false);
                Materias[2].gameObject.SetActive(false);
                Materias[4].gameObject.SetActive(false);

                Materias[3].gameObject.SetActive(true);
                break;
            case 4:
                Materias[4].SetAsLastSibling();
                Materias[0].gameObject.SetActive(false);
                Materias[1].gameObject.SetActive(false);
                Materias[2].gameObject.SetActive(false);
                Materias[3].gameObject.SetActive(false);

                Materias[4].gameObject.SetActive(true);
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
