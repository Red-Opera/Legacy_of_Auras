using System.Collections;
using TMPro;
using UnityEngine;

public class MonsterHPChangeUI : MonoBehaviour
{
    public GameObject resultText;
    public MonsterState state;

    private GameObject player;
    private Animator animator;

    private void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): 플레이어가 존재하지 않습니다.");

        Debug.Assert(resultText != null, "Error (Null Reference): 데미지를 출력할 텍스트가 존재하지 않습니다.");
    }

    private void Update()
    {
        transform.LookAt(player.transform.position);
    }

    public IEnumerator CreateChangeText(bool isDamage, int addHP)
    {
        if (addHP < 0)
            addHP = 0;

        GameObject text = Instantiate(resultText, transform);
        TextMeshProUGUI mesh = text.GetComponentInChildren<TextMeshProUGUI>();

        animator = text.GetComponent<Animator>();

        // 데미지를 받은 경우 -로 출력함
        if (isDamage)
            mesh.text = "-" + addHP;

        // 체력을 얻은 경우 +로 출력함
        else
            mesh.text = "+" + addHP;

        // 텍스트 색깔을 바꿈
        ChangeTextColor(mesh, isDamage, addHP);

        while (true)
        {
            // 애니메이션이 끝난 경우 중지
            if (text != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98)
            {
                Destroy(text);
                yield break;
            }

            yield return null;
        }
    }

    // 텍스트 색깔을 바꾸는 메소드
    private void ChangeTextColor(TextMeshProUGUI mesh, bool isDamage, int damage)
    {
        // 체력 변화에 얼마나 받았는지 확인하는 변수
        float damagePercentage;

        // 데미지인지 아닌지 판단 후 출력할 색깔를 정규화 함
        if (isDamage)
            damagePercentage = 50 - ((float)damage * 100 / state.HP)  / 2f;

        else
            damagePercentage = 50 + ((float)damage * 100 / state.HP) / 2f;

        // 정규화한 데이터를 기준으로 색깔을 지정함
        Color startColor = Color.red;
        Color endColor = Color.green;
        Color textColor = Color.Lerp(startColor, endColor, damagePercentage / 100);

        mesh.color = textColor;
    }
}