using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFilmUIOff : MonoBehaviour
{
    [SerializeField] private GameObject ui;     // 플레이어 UI

    private bool isOnceOff = false;

    void Start()
    {
        
    }

    void Update()
    {
        // 해당 씬이 최종 보스씬인 경우 바로 UI를 끔
        if (SceneManager.GetActiveScene().name == "Final" && !isOnceOff)
        {
            isOnceOff = true;
            ui.SetActive(false);
        }

        // 영화가 끝난 경우 UI 기능을 다시 낌
        if (BossSceneFilm.isFilmEnd)
        {
            ui.SetActive(true);
            enabled = false;
        }
    }
}
