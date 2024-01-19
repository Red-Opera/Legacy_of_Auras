using UnityEngine;

public class StoreSignSound : MonoBehaviour
{
    public AudioClip firstSound;  // 애니메이션 0% 진행 시 재생될 사운드
    public AudioClip secondSound; // 애니메이션 50% 진행 시 재생될 사운드

    private Animator animator;      // 애니메이터 컴포넌트 참조
    private AudioSource audioSource;// 오디오 소스
    private bool firstSoundPlayed;  // 첫 번째 사운드 재생 여부를 나타내는 플래그
    private bool secondSoundPlayed; // 두 번째 사운드 재생 여부를 나타내는 플래그

    void Start()
    {
        // 객체의 Animator 컴포넌트를 저장
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0.5f;
    }

    void Update()
    {
        // 현재 애니메이션 클립의 정규화된 진행 상태를 얻음 (0에서 1 사이의 값)
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

        // 애니메이션 진행 상태를 확인하고 사운드 재생
        if (normalizedTime >= 0 && normalizedTime < 0.5f && !firstSoundPlayed)
        {
            // 애니메이션이 0%에 도달했을 때 첫 번째 사운드 재생
            audioSource.PlayOneShot(firstSound);
            firstSoundPlayed = true;
            secondSoundPlayed = false; // 두 번째 사운드 플래그 초기화
        }
        else if (normalizedTime >= 0.5f && !secondSoundPlayed)
        {
            // 애니메이션이 50%에 도달했을 때 두 번째 사운드 재생
            audioSource.PlayOneShot(secondSound);
            secondSoundPlayed = true;
            firstSoundPlayed = false; // 첫 번째 사운드 플래그 초기화
        }
    }
}
