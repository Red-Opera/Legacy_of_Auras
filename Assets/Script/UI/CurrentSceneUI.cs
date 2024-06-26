using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurrentSceneUI : MonoBehaviour
{
    public TextMeshProUGUI mapName;   // 맵 이름을 출력할 텍스트

    [SerializeField] private GameObject sceneNameUI;    // 현재 씬 이름을 알려주는 UI
    [SerializeField] private float fadeTime;            // 점점 투명/불투명하게 만드는데 걸리는 시간
    [SerializeField] private float delayTime;           // 대기 시간을 알려주는 변수

    [SerializeField] private List<Image> fadeTarget;    // 페이트 대상

    private List<Color> defaultColor;   // 기존 색깔
    private List<Color> toColor;        // 투명한 색깔

    public void Start()
    {
        SceneManager.sceneLoaded += DisplaySceneName;

        defaultColor = new List<Color>();
        toColor = new List<Color>();

        for (int i = 0; i < fadeTarget.Count; i++)
            defaultColor.Add(fadeTarget[i].color);

        for (int i = 0; i < fadeTarget.Count; i++)
            toColor.Add(new Color(fadeTarget[i].color.r, fadeTarget[i].color.g, fadeTarget[i].color.b, 0));

        DisplaySceneName(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void DisplaySceneName(Scene scene, LoadSceneMode mode)
    {
        sceneNameUI.SetActive(true);
        StartCoroutine(ChangeCurrentSceneName());

        StartCoroutine(StartDisplay());
    }

    private IEnumerator ChangeCurrentSceneName()
    {
        string scneneName = SceneManager.GetActiveScene().name;

        yield return null;

        if (scneneName == "Village")
        {
            Collider[] colliders = Physics.OverlapSphere(GameObject.Find("Model").transform.position, 5);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.name == "OutStoreExit")
                {
                    mapName.text = "Desolate Emporium";
                    yield break;
                }
            }

            mapName.text = scneneName;
        }

        else if (scneneName == "Library")
            mapName.text = scneneName;

        else if (scneneName == "Desert")
            mapName.text = "Endless Barrens";

        else if (scneneName == "Forest")
            mapName.text = "Verdant Vale";

        else if (scneneName == "Final")
            mapName.text = "Infernal Abyss";
    }

    private IEnumerator StartDisplay()
    {
        float startTime = Time.time;

        for (int i = 0; i < fadeTarget.Count; i++)
            fadeTarget[i].color = toColor[i];

        while (Time.time - startTime <= fadeTime)
        {
            float persent = (Time.time - startTime) / fadeTime;

            for (int i = 0; i < fadeTarget.Count; i++)
            {
                fadeTarget[i].color = Color.Lerp(fadeTarget[i].color, defaultColor[i], persent);
                mapName.color = Color.Lerp(mapName.color, Color.white, persent);
            }

            yield return null;
        }

        yield return new WaitForSeconds(delayTime);

        Color toTextColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        startTime = Time.time;
        while (Time.time - startTime <= fadeTime)
        {
            float persent = (Time.time - startTime) / fadeTime;

            for (int i = 0; i < fadeTarget.Count; i++)
            {
                fadeTarget[i].color = Color.Lerp(fadeTarget[i].color, toColor[i], persent);
                mapName.color = Color.Lerp(mapName.color, toTextColor, persent);
            }

            yield return null;
        }

        sceneNameUI.SetActive(false);
    }
}
