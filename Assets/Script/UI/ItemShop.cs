using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class ItemShop : MonoBehaviour
{
    public PlayerState playerState;                 // 플레이어 상태

    public GameObject buyImg;                       // 상점 UI 이미지
    public GameObject details;                      // 아이템을 전시할 수 있는 위치
    public GameObject selectImg;                    // 선택했을 때 표시할 이미지

    public TextMeshProUGUI costText;                // 비용을 표시할 텍스트
    public TextMeshProUGUI remainCoinText;          // 남은 비용 텍스트

    public Button buyButton;                        // 구매 버튼
    public Ani2DRun signAnimation;                  // 사인 애니메이션 스크립트
    public AudioClip buySound;                      // 구매 소리

    public List<int> costs = new List<int>();       // 전시할 아이템의 비용 순서

    public ParticleSystem featherParticle;          // 깃털 이펙트

    private List<GameObject> itemIamges;            // 전시할 상점 아이템 이미지
    private List<GameObject> detailTexts;           // 아이템의 세부사항을 볼 수 있는 텍스트
    private int nowIndex = 0;                       // 선택한 현재 인덱스

    private AudioSource audioSource;                // 오디오 소스

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "Error (Null Reference) : 해당 객체에 오디오가 존재하지 않습니다.");

        itemIamges = new List<GameObject>();
        detailTexts = new List<GameObject>();

        // 아이템 이미지를 
        for (int i = 0; i < buyImg.transform.childCount - 1; i++)
            itemIamges.Add(buyImg.transform.GetChild(i).gameObject);

        // 아이템에 있는 모든 세부사항을 가져옴
        for (int i = 0; i < details.transform.childCount; i++)
            detailTexts.Add(details.transform.GetChild(i).gameObject);

        // 현재 있는 돈과 비용을 모두 출력함
        remainCoinText.text = FormatNumber(playerState.money);
        costText.text = FormatNumber(costs[0]);

        if (playerState.money < costs[0])
            remainCoinText.color = Color.red;

        else
            remainCoinText.color = Color.black;
    }

    public void Update()
    {

    }

    public void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnDisable()
    {
        Cursor.visible = false;                     // 커서를 감춤
        Cursor.lockState = CursorLockMode.Locked;   // 커서가 움직이지 않도록 설정
    }

    // 수치를 천 단위로 포맷팅하는 함수
    private string FormatNumber(int value)
    {
        return string.Format("{0:#,0}", value);
    }

    // 아이템을 선택했을때 처리하는 메소드
    public void ChangeSelected(int index)
    {
        // 선택 이미지를 현재 선택한 인텍스로 설정함
        selectImg.transform.position = itemIamges[index].transform.position;

        // 그의 가격도 설정함
        costText.text = FormatNumber(costs[index]);

        if (playerState.money < costs[index])
            remainCoinText.color = Color.red;

        else
            remainCoinText.color = Color.black;

        // 모든 세부사항을 모두 비활성화한 후 선택한 세부사항만 보이도록 설정
        foreach (GameObject item in detailTexts)
            item.SetActive(false);
        detailTexts[index].SetActive(true);

        // 현재 인덱스 설정
        nowIndex = index;
    }

    // 아이템을 구매하는 메소드
    public void BuyItem()
    {
        // 플레이어가 돈이 없거나 이미 구매 중인 경우 중지
        if (playerState.money < int.Parse(costText.text) || signAnimation.isSign)
            return;

        // 현재 돈을 감소시키고 화면상에 돈이 남은 여부를 동기화함
        playerState.money -= int.Parse(costText.text);
        remainCoinText.text = FormatNumber(playerState.money);

        ChangeSelected(nowIndex);

        // 0번째 아이템을 선택한 경우
        if (nowIndex == 0)
            InventroyPosition.CallAddItem("HPPotion", 1);

        else if (nowIndex == 1)
            InventroyPosition.CallAddItem("ManaPotion", 1);

        // 구매소리를 재생히고 깃털 및 사인 애니메이션을 재생함
        audioSource.PlayOneShot(buySound);
        featherParticle.Play();
        StartCoroutine(signAnimation.Play(false));
    }
}