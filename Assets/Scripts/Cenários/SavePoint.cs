using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static Vector2 CheckpointPosition = new Vector2(-84.76f, 25.64f);
    public static bool CheckpointAnim = false;
    public static bool CheckpointAnim2 = false;

    //Parte da introdução
    public static bool JumpApres = false;
    public static bool AttackApres = false;
    public static bool WallApres = false;
    public static bool DashApres = false;

    //Menu / Reset Checkpoint
    public static string nomeCenaMenu = "Altior-Quarto";
}