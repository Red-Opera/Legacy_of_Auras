using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    public static bool isGround = true;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        isGround = true;
    }

    public void OnTriggerStay(Collider other)
    {
        isGround = true;
    }

    public void OnTriggerExit(Collider other)
    {
        isGround = false;
    }
}
