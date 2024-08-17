using UnityEngine;
using UnityEngine.SceneManagement;

public class PixelateUI : MonoBehaviour
{
    [SerializeField] private Canvas pixelCanvas;    // 게임 화면을 픽셀로 만드는 캔버스
    [SerializeField] private Camera pixelInCarema;  // 픽셀 카메라

    private void Start()
    {

    }

    private void Update()
    {
        // 현재 최종 보스 맵이 아닌 경우 사용 불가능
        if (pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name != "Final")
        {
            pixelCanvas.gameObject.SetActive(false);
            pixelInCarema.gameObject.SetActive(false);
        }

        // 현재 최종 보스 맵인 경우 캔버스 사용 가능
        else if (!pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name == "Final")
        {
            pixelCanvas.gameObject.SetActive(true);
            pixelInCarema.gameObject.SetActive(true);
        }
    }
}
