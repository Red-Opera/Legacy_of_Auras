using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

// �÷��̾ �ȴµ� �Ҹ��� ���� ���� ��ũ��Ʈ
// ���� �ʿ��� ��: �÷��̾� �ȱ� �Ҹ� Ŭ��
public class PlayerWalkSound : MonoBehaviour
{
    [SerializeField] private AudioClip villageClip1;    // �������� ����� �÷��̾� �ȴ� �Ҹ� 1
    [SerializeField] private AudioClip villageClip2;    // �������� ����� �÷��̾� �ȴ� �Ҹ� 2
    [SerializeField] private AudioClip storeClip;       // �������� ����� �÷��̾� �ȴ� �Ҹ�
    [SerializeField] private AudioClip libClip;         // ���������� ����� �÷��̾� �ȱ� Ŭ��
    [SerializeField] private AudioClip desertClip;      // �縷���� ����� �÷��̾� �ȱ� Ŭ��
    [SerializeField] private AudioClip waterClip1;      // ������ �ȴ� �Ҹ� 1
    [SerializeField] private AudioClip waterClip2;      // ������ �ȴ� �Ҹ� 2
    [SerializeField] private AudioClip bossClip1;       // ���������� �ȴ� �Ҹ� 1
    [SerializeField] private AudioClip bossClip2;       // ���������� �ȴ� �Ҹ� 2

    [SerializeField] private List<WalkSoundTagNameToAudioClip> nameToClipInspector;  // �ش� �̸��� ������Ʈ�� ���� �Ҹ�

    private Dictionary<string, AudioClip> nameToClip;
    private Animator animator;              // �÷��̾��� �ִϸ�����
    private AudioSource audioSource;        // ������� ����ϱ� ���� ����

    private bool leftLegSound = false;      // 0.5 ���൵���� �Ҹ��� ����ߴ��� ����
    private bool rightLegSound = false;     // 0.95 ���൵���� �Ҹ��� ����ߴ��� ����

    private bool isWaterStep = false;        // ���� �� ���̸� �Ȱ� �ִ��� ����

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        nameToClip = new Dictionary<string, AudioClip>();

        for (int i = 0; i < nameToClipInspector.Count; i++)
            nameToClip.Add(nameToClipInspector[i].tagName, nameToClipInspector[i].clip);
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;                                  // ���� ���� �̸��� ������
        float animationPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;    // ���� �ִϸ��̼� ���൵�� ������

        AudioClip selectedClip = villageClip1;

        if (isWaterStep)
        {
            if (leftLegSound)
                selectedClip = waterClip1;

            else
                selectedClip = waterClip2;
        }

        else if (sceneName == "Village")
        {
            if (PlayerTeleport.isInStore)
                selectedClip = storeClip;

            else
            {
                if (leftLegSound)
                    selectedClip = villageClip1;

                else
                    selectedClip = villageClip2;
            }
        }

        else if (sceneName == "Library")
            selectedClip = libClip;

        else if (sceneName == "Desert")
            selectedClip = desertClip;

        else if (sceneName == "Forest")
        {
            if (leftLegSound)
                selectedClip = villageClip1;

            else
                selectedClip = villageClip2;
        }

        else if (sceneName == "Final")
            selectedClip = storeClip;

        // Animator�� ���°� "walk"�� ������ �Ҹ��� ���
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            return;

        if (animationPercent <= 0.1f)
        {
            leftLegSound = false;
            rightLegSound = false;
        }

        AudioClip changeSound = SoundChangeFromObject();

        if (changeSound != null)
            selectedClip = changeSound;

        // 0.5 ���൵���� �� �� �Ҹ��� ���
        if (animationPercent >= 0.5f && !leftLegSound)
        {
            PlaySound(selectedClip);
            leftLegSound = true;        // 0.5 ���൵���� �Ҹ��� ��������� ǥ��
        }

        // 0.95 ���൵���� �� �� �Ҹ��� ���
        if (animationPercent >= 0.95f && !rightLegSound)
        {
            PlaySound(selectedClip);
            rightLegSound = true;       // 0.95 ���൵���� �Ҹ��� ��������� ǥ��
        }
    }

    // ���� ���� �� �Ҹ��� �ٲ�� ������Ʈ�� ���
    private AudioClip SoundChangeFromObject()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            string soundName = hit.transform.gameObject.tag;

            if (soundName.Contains("Sound"))
            {
                if (leftLegSound)
                    return nameToClip[soundName];

                return nameToClip[soundName + "2"];
            }
        }

        return null;
    }

    // �Ҹ��� ����ϴ� �Լ�
    private void PlaySound(AudioClip clip)
    {
        audioSource.volume = 0.5f * GameManager.info.soundVolume; 
        audioSource.PlayOneShot(clip);
    }

    public void OnTriggerEnter(Collider other)
    {
        // �� ���̸� �Ȱ� �ִ� ���
        if (other.name == "water")
            isWaterStep = true;
    }

    public void OnTriggerExit(Collider other)
    {
        // �� ������ �� ���
        if (other.name == "water")
            isWaterStep = false;
    }
}

[Serializable]
public class WalkSoundTagNameToAudioClip
{
    public string tagName;
    public AudioClip clip;
}