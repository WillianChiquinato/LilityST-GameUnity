using UnityEngine;
using DG.Tweening;

public class Fader_Effect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeDuracao;

    public void Fade(bool fadeDirecao)
    {
        float target = fadeDirecao ? 1f : 0f;
        spriteRenderer.DOFade(target, fadeDuracao);
    }
}
