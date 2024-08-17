using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeOut : MonoBehaviour
{
    FadeInOutEffect fade;

    private void Start()
    {
        fade = GetComponent<FadeInOutEffect>();
        Debug.Assert(fade != null, "Error (Null Reference) : 페이트 효과가 존재하지 않습니다.");

        SceneManager.sceneLoaded += fade.SceneLoadedFadeIn;
        SceneManager.sceneUnloaded += fade.SceneUnLoadedFadeOut;
        
        StartCoroutine(fade.FadeIn(2.0f, true));
    }
}
