using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AutoSave : MonoBehaviour
{
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += SaveCurrentScene;
    }

    private static void SaveCurrentScene(PlayModeStateChange state)
    {
        Debug.Log("state : " + state
            + "/ isPlaying : " + EditorApplication.isPlaying
            + "/ isPlayingOrWillChangePlaymode : " + EditorApplication.isPlayingOrWillChangePlaymode);

        if (EditorApplication.isPlaying == false
                    && EditorApplication.isPlayingOrWillChangePlaymode == true)
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

    }
}
