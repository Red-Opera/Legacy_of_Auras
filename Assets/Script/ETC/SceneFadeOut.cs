using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeOut : MonoBehaviour
{
    FadeInOutEffect fade;

    private void Start()
    {
        fade = GetComponent<FadeInOutEffect>();
        Debug.Assert(fade != null, "Error (Null Reference) : ����Ʈ ȿ���� �������� �ʽ��ϴ�.");

        SceneManager.sceneLoaded += fade.SceneLoadedFadeIn;
        SceneManager.sceneUnloaded += fade.SceneUnLoadedFadeOut;
        
        StartCoroutine(fade.FadeIn(2.0f, true));
    }
}
