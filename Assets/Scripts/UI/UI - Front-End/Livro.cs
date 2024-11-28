using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Livro : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    [SerializeField] int index = 0;
    [SerializeField] bool rotate = false;

    public void InitialState()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].rotation = Quaternion.identity;
        }

        pages[0].SetAsLastSibling();
    }

    public void GoToPage(int targetIndex)
    {
        if (rotate || targetIndex < 0 || targetIndex >= pages.Count || targetIndex == index)
        {
            return;
        }

        float angle = targetIndex > index ? 180 : 0;
        int previousIndex = index;
        index = targetIndex;

        pages[targetIndex].SetAsLastSibling();
        pages[previousIndex].SetAsLastSibling();


        StartCoroutine(Rotate(pages[previousIndex], pages[targetIndex], angle));
    }

    private IEnumerator Rotate(Transform currentPage, Transform targetPage, float targetAngle)
    {
        rotate = true;


        float currentAngle = currentPage.localRotation.eulerAngles.y;
        float elapsed = 0;

        while (elapsed < pageSpeed)
        {
            elapsed += Time.deltaTime;

            float angle = Mathf.Lerp(currentAngle, targetAngle, elapsed / pageSpeed);
            currentPage.localRotation = Quaternion.Euler(0, angle, 0);
            targetPage.localRotation = Quaternion.Euler(0, targetAngle == 180 ? 0 : 180, 0);

            yield return null;
        }

        currentPage.localRotation = Quaternion.Euler(0, targetAngle, 0);
        targetPage.localRotation = Quaternion.Euler(0, targetAngle == 180 ? 0 : 180, 0);

        rotate = false;
    }
}
