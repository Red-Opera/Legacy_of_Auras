using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class EngNameToKorea
{
    public string eng;
    public string korea;
}

public class Loading : MonoBehaviour
{
    public static Loading instance;
    public static List<EngNameToKorea> engToKoreaStatic;

    public List<EngNameToKorea> engToKorea;

    [SerializeField] private GameObject loadingUI;
    [SerializeField] private TextMeshProUGUI nextMapName;
    [SerializeField] private TextMeshProUGUI persentText;
    [SerializeField] private Slider persentSlider;

    private void Start()
    {
        Debug.Assert(loadingUI != null, "Error (Null Reference) : �ε� UI�� �������� �ʽ��ϴ�.");
        Debug.Assert(nextMapName != null, "Error (Null Reference) : �̵��ϴ� �� �̸� UI�� �������� �ʽ��ϴ�.");
        Debug.Assert(persentText != null, "Error (Null Reference) : ���� ���� �ۼ�Ʈ ǥ���ϴ� UI�� �������� �ʽ��ϴ�.");
        Debug.Assert(persentSlider != null, "Error (Null Reference) : ���� ���� ǥ���ϴ� �����̴��� �������� �ʽ��ϴ�.");

        engToKoreaStatic = engToKorea;

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += CloseUI;
    }

    private void Update()
    {
        
    }

    private void CloseUI(Scene scene, LoadSceneMode mode)
    {
        loadingUI.SetActive(false);
    }

    public IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingUI.SetActive(true);

        for (int i = 0; i < engToKorea.Count; i++)
        {
            if (engToKorea[i].eng == MiniMap.currentMapName)
                nextMapName.text = engToKorea[i].korea;
        }

        while (!operation.isDone)
        {
            float currentPersent = Mathf.Clamp01(operation.progress / 0.9f);
            persentSlider.value = currentPersent;
            persentText.text = persentSlider.value.ToString("##0.0%");

            yield return null;
        }
    }
}
