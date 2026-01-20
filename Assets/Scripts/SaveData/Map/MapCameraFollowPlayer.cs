using System.Collections;
using UnityEngine;

public class MapCameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    public float smooth = 6f;

    void Start()
    {
        StartCoroutine(WaitToLoadingMapBridge());
    }

    IEnumerator WaitToLoadingMapBridge()
    {
        yield return new WaitForSeconds(1f);

        target = MapBridge.Instance.playerWorld;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            Time.deltaTime * smooth
        );
    }
}
