using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LassBossNameEffect : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset info;    // ��Ʈ�� ������ ��� ����
    [SerializeField] private float maxNextDelay;    // ���� ���� ȿ������ ��� �ð�
    [SerializeField] private float maxEffectTime;   // �ִ� ���� ȿ�� �ð�

    private static List<TMP_Character> characterInfoArray;  // �ش� ��Ʈ�� ���� �� �ִ� ��� ����

    private TextMeshPro text;               // ����� ����
    private bool currentEffect = false;     // ���� ���� ���ڸ� �����ϰ� �ִ��� ����
    private string defaultText;             // ������ �ؽ�Ʈ

    public void Start()
    {
        Debug.Assert(info != null, "Error (Null Reference) : �ؽ�Ʈ ��Ʈ ������ �������� �ʽ��ϴ�.");

        // ��Ʈ �������� ����� �� �ִ� ��� ���ڸ� ������
        characterInfoArray = new List<TMP_Character>();
        characterInfoArray = info.characterTable;

        text = GetComponent<TextMeshPro>();
        Debug.Assert(text != null, "Error (Null Reference) : �ؽ�Ʈ�� ����� ������Ʈ�� �������� �ʽ��ϴ�.");

        // ������ �ؽ�Ʈ ����
        defaultText = text.text;
    }

    public void Update()
    {
        // ���� ȿ���� ���� �ʰ� �ִٸ� ȿ�� ȿ�� ����
        if (!currentEffect)
            StartCoroutine(SettingNext());
    }

    private IEnumerator SettingNext()
    {
        currentEffect = true;

        // ������ ����� ���� ���ڿ� �� ���� ���ڸ� ��±��� ���ð��� ����
        int nextCharacter = UnityEngine.Random.Range(0, characterInfoArray.Count);
        float nextRemain = UnityEngine.Random.Range(0, maxNextDelay);

        yield return new WaitForSeconds(nextRemain);

        // �ش� ���ڸ� ���� ���ڷ� ����
        text.text = Convert.ToChar((int)characterInfoArray[nextCharacter].unicode).ToString();

        StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        // ���� ���� ���� �ð��� ����
        float effectTime = UnityEngine.Random.Range(0, maxEffectTime);

        yield return new WaitForSeconds(effectTime);

        // ���ڸ� ������� ����
        text.text = defaultText;
        currentEffect = false;
    }
}
