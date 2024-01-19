using UnityEngine;

public class StoreSignSound : MonoBehaviour
{
    public AudioClip firstSound;  // �ִϸ��̼� 0% ���� �� ����� ����
    public AudioClip secondSound; // �ִϸ��̼� 50% ���� �� ����� ����

    private Animator animator;      // �ִϸ����� ������Ʈ ����
    private AudioSource audioSource;// ����� �ҽ�
    private bool firstSoundPlayed;  // ù ��° ���� ��� ���θ� ��Ÿ���� �÷���
    private bool secondSoundPlayed; // �� ��° ���� ��� ���θ� ��Ÿ���� �÷���

    void Start()
    {
        // ��ü�� Animator ������Ʈ�� ����
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0.5f;
    }

    void Update()
    {
        // ���� �ִϸ��̼� Ŭ���� ����ȭ�� ���� ���¸� ���� (0���� 1 ������ ��)
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

        // �ִϸ��̼� ���� ���¸� Ȯ���ϰ� ���� ���
        if (normalizedTime >= 0 && normalizedTime < 0.5f && !firstSoundPlayed)
        {
            // �ִϸ��̼��� 0%�� �������� �� ù ��° ���� ���
            audioSource.PlayOneShot(firstSound);
            firstSoundPlayed = true;
            secondSoundPlayed = false; // �� ��° ���� �÷��� �ʱ�ȭ
        }
        else if (normalizedTime >= 0.5f && !secondSoundPlayed)
        {
            // �ִϸ��̼��� 50%�� �������� �� �� ��° ���� ���
            audioSource.PlayOneShot(secondSound);
            secondSoundPlayed = true;
            firstSoundPlayed = false; // ù ��° ���� �÷��� �ʱ�ȭ
        }
    }
}
