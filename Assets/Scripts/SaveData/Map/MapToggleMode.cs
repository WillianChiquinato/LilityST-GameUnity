using UnityEngine;

public class MapToggleMode : MonoBehaviour
{
    public bool freeMoveEnabled = false;

    private MapCameraFollowPlayer follow;
    private MapPan pan;

    void Start()
    {
        follow = GetComponent<MapCameraFollowPlayer>();
        pan = GetComponent<MapPan>();
    }

    void Update()
    {
        follow.enabled = !freeMoveEnabled;
        pan.enabled = freeMoveEnabled;
    }

    public void ToggleFreeMoveBtn()
    {
        freeMoveEnabled = !freeMoveEnabled;
        GameManager.instance.player.canMove = !freeMoveEnabled;
    }
}
