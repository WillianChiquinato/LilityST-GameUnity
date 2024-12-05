using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Config_Options : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;

    public void SetVolume(float Volume)
    {
        audioMixer.SetFloat("Volume", Volume);
    }

    public void SetQualidade(int IndexQualidade)
    {
        QualitySettings.SetQualityLevel(IndexQualidade);
    }

    public void SetTela_Cheia(bool Tela_Cheia)
    {
        Screen.fullScreen = Tela_Cheia;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
