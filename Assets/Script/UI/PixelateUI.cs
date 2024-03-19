using UnityEngine;
using UnityEngine.SceneManagement;

public class PixelateUI : MonoBehaviour
{
    [SerializeField] private Canvas pixelCanvas;    // ���� ȭ���� �ȼ��� ����� ĵ����

    void Start()
    {

    }

    void Update()
    {
        // ���� ���� ���� ���� �ƴ� ��� ��� �Ұ���
        if (pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name != "Final")
            pixelCanvas.gameObject.SetActive(false);

        // ���� ���� ���� ���� ��� ĵ���� ��� ����
        else if (!pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name == "Final")
            pixelCanvas.gameObject.SetActive(true);
    }
}
