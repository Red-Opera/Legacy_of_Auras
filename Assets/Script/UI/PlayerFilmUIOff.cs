using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFilmUIOff : MonoBehaviour
{
    [SerializeField] private GameObject ui;     // �÷��̾� UI

    private bool isOnceOff = false;

    void Start()
    {
        
    }

    void Update()
    {
        // �ش� ���� ���� �������� ��� �ٷ� UI�� ��
        if (SceneManager.GetActiveScene().name == "Final" && !isOnceOff)
        {
            isOnceOff = true;
            ui.SetActive(false);
        }

        // ��ȭ�� ���� ��� UI ����� �ٽ� ��
        if (BossSceneFilm.isFilmEnd)
        {
            ui.SetActive(true);
            enabled = false;
        }
    }
}
