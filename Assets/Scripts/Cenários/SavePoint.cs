using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static Vector2 CheckpointPosition = new Vector2(-97.18f, 24.04f);
    public static bool CheckpointAnim = false;
    public static bool CheckpointAnim2 = false;

    //Parte da introdução
    public static bool JumpApres = true;
    public static bool AttackApres = true;
    public static bool WallApres = true;
    public static bool DashApres = true;
    public static bool ArcoApres = true;

    //Menu / Reset Checkpoint
    public static string nomeCenaMenu = "Altior-Quarto";
}