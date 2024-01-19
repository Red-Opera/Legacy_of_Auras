using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class ItemShop : MonoBehaviour
{
    public PlayerState playerState;                 // �÷��̾� ����

    public GameObject buyImg;                       // ���� UI �̹���
    public GameObject details;                      // �������� ������ �� �ִ� ��ġ
    public GameObject selectImg;                    // �������� �� ǥ���� �̹���

    public TextMeshProUGUI costText;                // ����� ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI remainCoinText;          // ���� ��� �ؽ�Ʈ

    public Button buyButton;                        // ���� ��ư
    public Ani2DRun signAnimation;                  // ���� �ִϸ��̼� ��ũ��Ʈ
    public AudioClip buySound;                      // ���� �Ҹ�

    public List<int> costs = new List<int>();       // ������ �������� ��� ����

    public ParticleSystem featherParticle;          // ���� ����Ʈ

    private List<GameObject> itemIamges;            // ������ ���� ������ �̹���
    private List<GameObject> detailTexts;           // �������� ���λ����� �� �� �ִ� �ؽ�Ʈ
    private int nowIndex = 0;                       // ������ ���� �ε���

    private AudioSource audioSource;                // ����� �ҽ�

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "Error (Null Reference) : �ش� ��ü�� ������� �������� �ʽ��ϴ�.");

        itemIamges = new List<GameObject>();
        detailTexts = new List<GameObject>();

        // ������ �̹����� 
        for (int i = 0; i < buyImg.transform.childCount - 1; i++)
            itemIamges.Add(buyImg.transform.GetChild(i).gameObject);

        // �����ۿ� �ִ� ��� ���λ����� ������
        for (int i = 0; i < details.transform.childCount; i++)
            detailTexts.Add(details.transform.GetChild(i).gameObject);

        // ���� �ִ� ���� ����� ��� �����
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
        Cursor.visible = false;                     // Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked;   // Ŀ���� �������� �ʵ��� ����
    }

    // ��ġ�� õ ������ �������ϴ� �Լ�
    private string FormatNumber(int value)
    {
        return string.Format("{0:#,0}", value);
    }

    // �������� ���������� ó���ϴ� �޼ҵ�
    public void ChangeSelected(int index)
    {
        // ���� �̹����� ���� ������ ���ؽ��� ������
        selectImg.transform.position = itemIamges[index].transform.position;

        // ���� ���ݵ� ������
        costText.text = FormatNumber(costs[index]);

        if (playerState.money < costs[index])
            remainCoinText.color = Color.red;

        else
            remainCoinText.color = Color.black;

        // ��� ���λ����� ��� ��Ȱ��ȭ�� �� ������ ���λ��׸� ���̵��� ����
        foreach (GameObject item in detailTexts)
            item.SetActive(false);
        detailTexts[index].SetActive(true);

        // ���� �ε��� ����
        nowIndex = index;
    }

    // �������� �����ϴ� �޼ҵ�
    public void BuyItem()
    {
        // �÷��̾ ���� ���ų� �̹� ���� ���� ��� ����
        if (playerState.money < int.Parse(costText.text) || signAnimation.isSign)
            return;

        // ���� ���� ���ҽ�Ű�� ȭ��� ���� ���� ���θ� ����ȭ��
        playerState.money -= int.Parse(costText.text);
        remainCoinText.text = FormatNumber(playerState.money);

        ChangeSelected(nowIndex);

        // 0��° �������� ������ ���
        if (nowIndex == 0)
            InventroyPosition.CallAddItem("HPPotion", 1);

        else if (nowIndex == 1)
            InventroyPosition.CallAddItem("ManaPotion", 1);

        // ���żҸ��� ������� ���� �� ���� �ִϸ��̼��� �����
        audioSource.PlayOneShot(buySound);
        featherParticle.Play();
        StartCoroutine(signAnimation.Play(false));
    }
}