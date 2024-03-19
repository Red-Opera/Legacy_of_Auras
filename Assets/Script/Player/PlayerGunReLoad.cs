using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGunReLoad : MonoBehaviour
{
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI remainText;
    public PlayerState state;
    public PlayerGunShotUI shotUI;

    private PlayerWeaponChanger weaponChanger;
    private Animator animator;

    public bool isReLoad { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        weaponChanger = GetComponent<PlayerWeaponChanger>();
        animator = GetComponent<Animator>();

        Debug.Assert(weaponChanger != null, "Error (Null Reference) : 플레이어에 WeaponChanger가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 플레이어에 애니메이터가 존재하지 않습니다.");
        Debug.Assert(currentText != null, "Error (Null Reference) : 현재 탄창의 총알 수가 존재하지 않습니다.");
        Debug.Assert(remainText != null, "Error (Null Reference) : 현재 전체 총알 수가 존재하지 않습니다.");
        Debug.Assert(state != null, "Error (Null Reference) : 플레이어 상태가 존재하지 않습니다.");
        Debug.Assert(shotUI != null, "Error (Null Reference) : 플레이어 총 UI가 존재하지 않습니다.");
    }

    void Update()
    {
        // 플레이어가 R키를 누르고 현제 총을 갖고 있는 경우
        if (Input.GetKeyDown(KeyCode.R) && weaponChanger.weaponType == WeaponType.GUN && !isReLoad)
            StartCoroutine(ReLoad());

    }

    // 탄창에 총알을 채우는 메소드
    private IEnumerator ReLoad()
    {
        int remain = Int32.Parse(remainText.text), current = Int32.Parse(currentText.text);
        int moveCount = state.bulletCurrentMax - current;

        if (remain <= 0 || current == state.bulletCurrentMax)
            yield break;

        isReLoad = true;

        // 총알 장전하는 애니메이션을 실행 
        animator.SetTrigger("Reload");
        while (true)
        {
            // 현재 애니메이션이 장전하는 모습하고 있을 경우 무한히 같은 모습하는 것을 방지
            if (animator.GetNextAnimatorStateInfo(1).IsName("ReloadGun"))
            {
                animator.SetBool("Reloading", true);
                break;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(3.3f);

        // 갖고 있는 총알의 수가 탄창 크기보다 작은 경우
        if (moveCount > remain)
        {
            // 모든 총알을 탄창으로 옮김
            currentText.SetText(remainText.text.ToString());
            remainText.SetText("0");
            shotUI.SetTextColor();
        }

        else
        {
            // 탄창에 총알을 가득 채움
            currentText.SetText(state.bulletCurrentMax.ToString());
            remainText.SetText((remain - moveCount).ToString());
            shotUI.SetTextColor();
        }

        // 다시 총을 잡고 있는 형태로 만듬
        animator.SetBool("Reloading", false);

        isReLoad = false;
    }
}
