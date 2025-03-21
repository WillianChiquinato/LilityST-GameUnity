using UnityEngine;

public class FilhoteDetectPlayer : MonoBehaviour
{
    public FilhoteDragão FilhoteDragão;

    void Awake()
    {
        FilhoteDragão = GetComponentInParent<FilhoteDragão>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && FilhoteDragão.targetObjects == null)
        {
            FilhoteDragão.FugaLility = true;
            if (FilhoteDragão.transform.localScale.x < 0)
            {
                FilhoteDragão.transform.localScale = new Vector3(FilhoteDragão.transform.localScale.x, FilhoteDragão.transform.localScale.y, FilhoteDragão.transform.localScale.z);
            }
            else
            {
                FilhoteDragão.transform.localScale = new Vector3(-FilhoteDragão.transform.localScale.x, FilhoteDragão.transform.localScale.y, FilhoteDragão.transform.localScale.z);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
