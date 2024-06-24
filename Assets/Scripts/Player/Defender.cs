using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Defender
{
    public void Defender(Vector2 direcao);

    public float returnSpeed { get; set; }
}
