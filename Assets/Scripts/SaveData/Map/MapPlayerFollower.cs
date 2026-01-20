using UnityEngine;

public class MapPlayerFollower : MonoBehaviour
{
    public Vector2 mapScale = Vector2.one;

    void LateUpdate()
    {
        if (!MapBridge.Instance || !MapBridge.Instance.playerWorld)
            return;

        Vector3 pos = MapBridge.Instance.playerWorld.position;

        transform.position = new Vector3(
            pos.x * mapScale.x,
            pos.y * mapScale.y,
            transform.position.z
        );
    }
}
