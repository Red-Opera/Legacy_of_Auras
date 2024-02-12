using UnityEngine;

public class DissolveHeightSetter : MonoBehaviour
{
    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetFloat("_DissolveStartHeight", transform.position.y);
    }
}
