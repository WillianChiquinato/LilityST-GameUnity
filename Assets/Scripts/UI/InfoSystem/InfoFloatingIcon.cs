using System.Collections;
using TMPro;
using UnityEngine;

public class InfoFloatingIcon : MonoBehaviour
{
    public static InfoFloatingIcon Instance;
    public GameObject floatingPanel;
    public TextMeshProUGUI texto;

    private void Awake() => Instance = this;

    public void MostrarIcone(string titulo)
    {
        StopAllCoroutines();
        StartCoroutine(AnimarIcone(titulo));
    }

    private IEnumerator AnimarIcone(string titulo)
    {
        floatingPanel.SetActive(true);
        texto.text = $"Nova anotação: {titulo}";
        CanvasGroup cg = floatingPanel.GetComponent<CanvasGroup>();

        cg.alpha = 0;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            cg.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            cg.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            cg.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            cg.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        floatingPanel.SetActive(false);
    }
}
