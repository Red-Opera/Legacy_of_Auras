using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어가 걷는데 소리를 내기 위한 스크립트
// 사용시 필요한 것: 플레이어 걷기 소리 클립
public class PlayerWalkSound : MonoBehaviour
{
    [SerializeField] private AudioClip villageClip1;    // 마을에서 재생할 플레이어 걷는 소리 1
    [SerializeField] private AudioClip villageClip2;    // 마을에서 재생할 플레이어 걷는 소리 2
    [SerializeField] private AudioClip storeClip;       // 상점에서 재생할 플레이어 걷는 소리
    [SerializeField] private AudioClip libClip;         // 도서관에서 재생할 플레이어 걷기 클립
    [SerializeField] private AudioClip desertClip;      // 사막에서 재생할 플레이어 걷기 클립
    [SerializeField] private AudioClip waterClip1;      // 물에서 걷는 소리 1
    [SerializeField] private AudioClip waterClip2;      // 물에서 걷는 소리 2
    [SerializeField] private AudioClip bossClip1;       // 보스전에서 걷는 소리 1
    [SerializeField] private AudioClip bossClip2;       // 보스전에서 걷는 소리 2

    [SerializeField] private List<WalkSoundTagNameToAudioClip> nameToClipInspector;  // 해당 이름의 오브젝트가 내는 소리

    private Dictionary<string, AudioClip> nameToClip;
    private Animator animator;              // 플레이어의 애니메이터
    private AudioSource audioSource;        // 오디오를 재생하기 위한 변수

    private bool leftLegSound = false;      // 0.5 진행도에서 소리를 재생했는지 여부
    private bool rightLegSound = false;     // 0.95 진행도에서 소리를 재생했는지 여부

    private bool isWaterStep = false;        // 현재 물 사이를 걷고 있는지 여부

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
        string sceneName = SceneManager.GetActiveScene().name;                                  // 현재 씬의 이름을 가져옴
        float animationPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;    // 현재 애니메이션 진행도를 가져옴

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

        // Animator의 상태가 "walk"일 때에만 소리를 재생
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

        // 0.5 진행도에서 한 번 소리를 재생
        if (animationPercent >= 0.5f && !leftLegSound)
        {
            PlaySound(selectedClip);
            leftLegSound = true;        // 0.5 진행도에서 소리를 재생했음을 표시
        }

        // 0.95 진행도에서 한 번 소리를 재생
        if (animationPercent >= 0.95f && !rightLegSound)
        {
            PlaySound(selectedClip);
            rightLegSound = true;       // 0.95 진행도에서 소리를 재생했음을 표시
        }
    }

    // 만약 걸을 때 소리가 바뀌는 오브젝트인 경우
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

    // 소리를 재생하는 함수
    private void PlaySound(AudioClip clip)
    {
        audioSource.volume = 0.5f * GameManager.info.soundVolume; 
        audioSource.PlayOneShot(clip);
    }

    public void OnTriggerEnter(Collider other)
    {
        // 물 사이를 걷고 있는 경우
        if (other.name == "water")
            isWaterStep = true;
    }

    public void OnTriggerExit(Collider other)
    {
        // 물 밖으로 들어간 경우
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