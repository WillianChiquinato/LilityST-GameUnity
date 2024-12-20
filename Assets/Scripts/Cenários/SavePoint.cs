using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static Vector2 CheckpointPosition = new Vector2(-96.92f, 24.04f);
    public static bool CheckpointAnim = false;
    public static bool CheckpointAnim2 = false;

    //Parte da introdução
    public static bool JumpApres = false;
    public static bool AttackApres = false;
    public static bool WallApres = false;
    public static bool DashApres = false;
    public static bool ArcoApres = true;

    //Menu / Reset Checkpoint
    public static string nomeCenaMenu = "Altior-Quarto";
}