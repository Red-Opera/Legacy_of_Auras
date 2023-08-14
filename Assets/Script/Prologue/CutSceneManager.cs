using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct ScenarioArea 
{ 
    public uint start, end; 

    public ScenarioArea(uint start, uint end) { this.start = start; this.end = end; }
}

public class CutSceneManager : MonoBehaviour
{
    public float delayTime = 1.0f;

    private uint lastCutNumber;         // 마지막 컷씬의 번호
    private uint cutCount = 1;          // 현재 컷씬의 번호

    private uint lastScriptNumber;      // 마지막 시나리오 번호
    private uint scriptCount = 1;       // 현재 시나리오 번호

    private bool isChangeCut = false;   // 현재 컷이 바뀌고 있는지 확인하는 변수

    [SerializeField]
    private List<ScenarioArea> imgDelay;            // 이미지가 유지하는 기간
    private List<FadeInOutEffect> imgFadeTarget;    // 이미지 페이드 타겟
    private List<FadeInOutEffect> textFadeTarget;   // 텍스트의 페이드 타겟 

    private Transform cutScene;                     // 컷씬 모음
    private Transform scenarioScript;               // 시나리오 모듬

    public void Start()
    {
        cutScene = transform.GetChild(0);           // 컷씬을 가져옴
        scenarioScript = transform.GetChild(1);     // 시나리오를 가져옴

        // 이미지 유지하는 시나리오 시간 입력
        imgDelay = new List<ScenarioArea>() 
        { 
            new ScenarioArea(2, 4), 
            new ScenarioArea(5, 8), 
            new ScenarioArea(9, 12) 
        };
        imgFadeTarget = new List<FadeInOutEffect>();
        textFadeTarget = new List<FadeInOutEffect>();

        lastCutNumber = Convert.ToUInt32(cutScene.childCount);              // 컷 이미지의 개수를 가져옴
        lastScriptNumber = Convert.ToUInt32(scenarioScript.childCount);     // 시나리오 개수를 가져옴

        // 페이드 인 아웃할 텍스트 및 이미지 객체를 가져옴
        for (int i = 0; i < lastScriptNumber; i++)
        {
            textFadeTarget.Add(scenarioScript.GetChild(i).GetComponent<FadeInOutEffect>());
            textFadeTarget[i].gameObject.SetActive(false);
        }

        // 페이드 잇 아웃할 이미지 객체를 가져옴
        for (int i = 0; i < lastCutNumber; i++)
        {
            imgFadeTarget.Add(cutScene.GetChild(i).GetComponent<FadeInOutEffect>());
            imgFadeTarget[i].gameObject.SetActive(false);
        }

        isChangeCut = true;                                     // 마우스 클릭 금지
        scenarioScript.GetChild(0).gameObject.SetActive(true);  // 첫번째 텍스트 출력
        StartCoroutine(textFadeTarget[0].FadeOut(delayTime));   // 페이드 아웃 (블록 자체를 불투명하게)
        isChangeCut = false;
    }

    public void Update()
    {
        // 현재 텍스트 및 이미지 변환 중일 경우
        if (isChangeCut)
            return;

        // 마우스를 클릭하거나 스페이스 바를 눌렸을 경우
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(NextCut());
    }

    // 다음 컷으로 넘어갈때 처리하는 함수
    public IEnumerator NextCut()
    {
        // 마우스 클릭 및 키보드 입력 금지
        isChangeCut = true;

        // 현재 컷 페이드 인
        StartCoroutine(textFadeTarget[Convert.ToInt32(scriptCount) - 1].FadeIn(delayTime, true));

        // 이미지 컷 페이드 인
        if (lastCutNumber >= cutCount && scriptCount == imgDelay[Convert.ToInt32(cutCount) - 1].end)
            StartCoroutine(imgFadeTarget[Convert.ToInt32(cutCount++) - 1].FadeIn(delayTime, true));

        yield return new WaitForSeconds(delayTime);
        scriptCount++;

        if (lastScriptNumber == scriptCount - 1)
            SceneManager.LoadScene("FirstMap");

        // 다음 컷이 존재할 경우
        if (scriptCount - 1 != lastScriptNumber)
        {
            // 다음 컷 페이드 아웃
            StartCoroutine(textFadeTarget[Convert.ToInt32(scriptCount) - 1].FadeOut(delayTime));

            // 이미지 다음 컷 페이드 아웃
            if (lastCutNumber >= cutCount && scriptCount == imgDelay[Convert.ToInt32(cutCount) - 1].start)
                StartCoroutine(imgFadeTarget[Convert.ToInt32(cutCount) - 1].FadeOut(delayTime));

            yield return new WaitForSeconds(delayTime);
        }
        
        isChangeCut = false;
    }
}