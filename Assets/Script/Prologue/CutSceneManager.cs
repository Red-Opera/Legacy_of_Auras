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

    private uint lastCutNumber;         // ������ �ƾ��� ��ȣ
    private uint cutCount = 1;          // ���� �ƾ��� ��ȣ

    private uint lastScriptNumber;      // ������ �ó����� ��ȣ
    private uint scriptCount = 1;       // ���� �ó����� ��ȣ

    private bool isChangeCut = false;   // ���� ���� �ٲ�� �ִ��� Ȯ���ϴ� ����

    [SerializeField]
    private List<ScenarioArea> imgDelay;            // �̹����� �����ϴ� �Ⱓ
    private List<FadeInOutEffect> imgFadeTarget;    // �̹��� ���̵� Ÿ��
    private List<FadeInOutEffect> textFadeTarget;   // �ؽ�Ʈ�� ���̵� Ÿ�� 

    private Transform cutScene;                     // �ƾ� ����
    private Transform scenarioScript;               // �ó����� ���

    public void Start()
    {
        cutScene = transform.GetChild(0);           // �ƾ��� ������
        scenarioScript = transform.GetChild(1);     // �ó������� ������

        // �̹��� �����ϴ� �ó����� �ð� �Է�
        imgDelay = new List<ScenarioArea>() 
        { 
            new ScenarioArea(2, 4), 
            new ScenarioArea(5, 8), 
            new ScenarioArea(9, 12) 
        };
        imgFadeTarget = new List<FadeInOutEffect>();
        textFadeTarget = new List<FadeInOutEffect>();

        lastCutNumber = Convert.ToUInt32(cutScene.childCount);              // �� �̹����� ������ ������
        lastScriptNumber = Convert.ToUInt32(scenarioScript.childCount);     // �ó����� ������ ������

        // ���̵� �� �ƿ��� �ؽ�Ʈ �� �̹��� ��ü�� ������
        for (int i = 0; i < lastScriptNumber; i++)
        {
            textFadeTarget.Add(scenarioScript.GetChild(i).GetComponent<FadeInOutEffect>());
            textFadeTarget[i].gameObject.SetActive(false);
        }

        // ���̵� �� �ƿ��� �̹��� ��ü�� ������
        for (int i = 0; i < lastCutNumber; i++)
        {
            imgFadeTarget.Add(cutScene.GetChild(i).GetComponent<FadeInOutEffect>());
            imgFadeTarget[i].gameObject.SetActive(false);
        }

        isChangeCut = true;                                     // ���콺 Ŭ�� ����
        scenarioScript.GetChild(0).gameObject.SetActive(true);  // ù��° �ؽ�Ʈ ���
        StartCoroutine(textFadeTarget[0].FadeOut(delayTime));   // ���̵� �ƿ� (��� ��ü�� �������ϰ�)
        isChangeCut = false;
    }

    public void Update()
    {
        // ���� �ؽ�Ʈ �� �̹��� ��ȯ ���� ���
        if (isChangeCut)
            return;

        // ���콺�� Ŭ���ϰų� �����̽� �ٸ� ������ ���
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(NextCut());
    }

    // ���� ������ �Ѿ�� ó���ϴ� �Լ�
    public IEnumerator NextCut()
    {
        // ���콺 Ŭ�� �� Ű���� �Է� ����
        isChangeCut = true;

        // ���� �� ���̵� ��
        StartCoroutine(textFadeTarget[Convert.ToInt32(scriptCount) - 1].FadeIn(delayTime, true));

        // �̹��� �� ���̵� ��
        if (lastCutNumber >= cutCount && scriptCount == imgDelay[Convert.ToInt32(cutCount) - 1].end)
            StartCoroutine(imgFadeTarget[Convert.ToInt32(cutCount++) - 1].FadeIn(delayTime, true));

        yield return new WaitForSeconds(delayTime);
        scriptCount++;

        if (lastScriptNumber == scriptCount - 1)
            SceneManager.LoadScene("FirstMap");

        // ���� ���� ������ ���
        if (scriptCount - 1 != lastScriptNumber)
        {
            // ���� �� ���̵� �ƿ�
            StartCoroutine(textFadeTarget[Convert.ToInt32(scriptCount) - 1].FadeOut(delayTime));

            // �̹��� ���� �� ���̵� �ƿ�
            if (lastCutNumber >= cutCount && scriptCount == imgDelay[Convert.ToInt32(cutCount) - 1].start)
                StartCoroutine(imgFadeTarget[Convert.ToInt32(cutCount) - 1].FadeOut(delayTime));

            yield return new WaitForSeconds(delayTime);
        }
        
        isChangeCut = false;
    }
}