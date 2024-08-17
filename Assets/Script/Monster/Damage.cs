using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private MonsterState state;

    private void SetDamage(GameObject target)
    {
        if (target.name == "Model")
            PlayerHPBar.SetDamage(state.attack);
    }


    private void OnCollisionEnter(Collision collision)
    {
        SetDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        SetDamage(other.gameObject);
    }
}
