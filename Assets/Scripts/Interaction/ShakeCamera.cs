using Cinemachine;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineImpulseSource[] impulseSources;

    void Awake()
    {
        impulseSources = GetComponents<CinemachineImpulseSource>();
    }

    public void ShakeCutplayRendalla()
    {
        impulseSources[0].GenerateImpulse();
    }

    public void ShakeHitDamage()
    {
        impulseSources[1].GenerateImpulse();
    }
}
