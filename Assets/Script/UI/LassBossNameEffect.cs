using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LassBossNameEffect : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset info;    // 폰트의 정보를 담는 변수
    [SerializeField] private float maxNextDelay;    // 다음 오류 효과까지 대기 시간
    [SerializeField] private float maxEffectTime;   // 최대 오류 효과 시간

    private static List<TMP_Character> characterInfoArray;  // 해당 폰트가 가질 수 있는 모든 글자

    private TextMeshPro text;               // 출력할 글자
    private bool currentEffect = false;     // 현재 오류 글자를 실행하고 있는지 여부
    private string defaultText;             // 기존의 텍스트

    public void Start()
    {
        Debug.Assert(info != null, "Error (Null Reference) : 텍스트 폰트 정보가 존재하지 않습니다.");

        // 폰트 정보에서 출력할 수 있는 모든 글자를 가져옴
        characterInfoArray = new List<TMP_Character>();
        characterInfoArray = info.characterTable;

        text = GetComponent<TextMeshPro>();
        Debug.Assert(text != null, "Error (Null Reference) : 텍스트를 출력할 컴포넌트가 존재하지 않습니다.");

        // 기존의 텍스트 저장
        defaultText = text.text;
    }

    public void Update()
    {
        // 오류 효과를 내지 않고 있다면 효류 효과 실행
        if (!currentEffect)
            StartCoroutine(SettingNext());
    }

    private IEnumerator SettingNext()
    {
        currentEffect = true;

        // 다음에 출력할 오류 문자와 그 오류 문자를 출력까지 대기시간을 구함
        int nextCharacter = UnityEngine.Random.Range(0, characterInfoArray.Count);
        float nextRemain = UnityEngine.Random.Range(0, maxNextDelay);

        yield return new WaitForSeconds(nextRemain);

        // 해당 문자를 오류 문자로 변경
        text.text = Convert.ToChar((int)characterInfoArray[nextCharacter].unicode).ToString();

        StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        // 오류 문자 진행 시간을 구함
        float effectTime = UnityEngine.Random.Range(0, maxEffectTime);

        yield return new WaitForSeconds(effectTime);

        // 글자를 원래대로 돌림
        text.text = defaultText;
        currentEffect = false;
    }
}
