using UnityEngine;

public interface IBlockDamage
{
    bool CanBlock(Transform attacker);
    void OnBlock();
}
