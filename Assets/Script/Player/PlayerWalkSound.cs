using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어가 걷는데 소리를 내기 위한 스크립트
// 사용시 필요한 것: 플레이어 걷기 소리 클립
public class PlayerWalkSound : MonoBehaviour
{
    public AudioClip villageClip;           // 마을에서 재생할 플레이어 걷기 클립
    public AudioClip libClip;               // 도서관에서 재생할 플레이어 걷기 클립
    public AudioClip desertClip;            // 사막에서 재생할 플레이어 걷기 클립
    public AudioClip waterClip1;            // 물에서 걷는 소리 1
    public AudioClip waterClip2;            // 물에서 걷는 소리 2

    private Animator animator;              // 플레이어의 애니메이터
    private AudioSource audioSource;        // 오디오를 재생하기 위한 변수

    private bool leftLegSound = false;      // 0.5 진행도에서 소리를 재생했는지 여부
    private bool rightLegSound = false;     // 0.95 진행도에서 소리를 재생했는지 여부

    private bool isWaterStep = false;        // 현재 물 사이를 걷고 있는지 여부

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;                                  // 현재 씬의 이름을 가져옵니다.
        float animationPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;    // 현재 애니메이션 진행도를 가져옴

        AudioClip selectedClip = villageClip;
        audioSource.volume = 1.0f;

        if (isWaterStep)
        {
            if (leftLegSound)
                selectedClip = waterClip1;

            else
                selectedClip = waterClip2;
        }

        else if (sceneName == "Village")
            selectedClip = villageClip;

        else if (sceneName == "Library")
            selectedClip = libClip;

        else if (sceneName == "Desert")
        {
            audioSource.volume = 0.3f;
            selectedClip = desertClip;
        }

        // Animator의 상태가 "walk"일 때에만 소리를 재생
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            return;

        if (animationPercent <= 0.1f)
        {
            leftLegSound = false;
            rightLegSound = false;
        }

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

    // 소리를 재생하는 함수
    private void PlaySound(AudioClip clip)
    {
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
