using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "State/PlayerState")]
public class PlayerState : State
{
    public int money;
    public int exp;

    public int bulletCount;
    public int bulletCurrentMax;
}
