using System.Collections;
using UnityEngine;

public enum RopeType
{
    Normal,
    ClimbInput
}

public class RopeController : MonoBehaviour
{
    public RopeType ropeType = RopeType.Normal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerRopeController>();
            if (player != null)
            {
                player.SetCurrentRope(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerRopeController>();
            if (player != null)
            {
                player.ClearCurrentRope(this);
            }
        }
    }

    public Vector3 GetSnapPosition(Vector3 playerPos)
    {
        return new Vector3(transform.position.x, playerPos.y, 0);
    }
}
