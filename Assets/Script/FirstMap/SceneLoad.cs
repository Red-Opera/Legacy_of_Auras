using UnityEngine;

public class SceneLoad : MonoBehaviour
{
    private FadeInOutEffect fadeIn; 

    public float fadeTime = 1.0f;

    public void Awake()
    {
        fadeIn = GetComponent<FadeInOutEffect>();

        if (fadeIn == null)
            fadeIn = gameObject.AddComponent<FadeInOutEffect>();
    }

    public void Start()
    {
        StartCoroutine(fadeIn.FadeIn(fadeTime));
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
