using UnityEngine;
using UnityEngine.Video;

public class ControllerVideos : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public LevelTransicao levelTransicao;

    void Start()
    {
        levelTransicao = FindAnyObjectByType<LevelTransicao>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        Debug.Log("O vídeo terminou!");
        levelTransicao.Transicao("Altior-Fuga");
    }
}
