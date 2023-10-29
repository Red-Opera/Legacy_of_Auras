using UnityEngine;
using UnityEngine.SceneManagement;

// �÷��̾ �ȴµ� �Ҹ��� ���� ���� ��ũ��Ʈ
// ���� �ʿ��� ��: �÷��̾� �ȱ� �Ҹ� Ŭ��
public class PlayerWalkSound : MonoBehaviour
{
    public AudioClip villageClip;           // �������� ����� �÷��̾� �ȱ� Ŭ��
    public AudioClip libClip;               // ���������� ����� �÷��̾� �ȱ� Ŭ��

    private Animator animator;              // �÷��̾��� �ִϸ�����
    private AudioSource audioSource;        // ������� ����ϱ� ���� ����

    private bool leftLegSound = false;      // 0.5 ���൵���� �Ҹ��� ����ߴ��� ����
    private bool rightLegSound = false;     // 0.95 ���൵���� �Ҹ��� ����ߴ��� ����

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;                                  // ���� ���� �̸��� �����ɴϴ�.
        float animationPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;    // ���� �ִϸ��̼� ���൵�� ������

        // Animator�� ���°� "walk"�� ������ �Ҹ��� ���
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            return;

        if (animationPercent <= 0.1f)
        {
            leftLegSound = false;
            rightLegSound = false;
        }

        // 0.5 ���൵���� �� �� �Ҹ��� ���
        if (animationPercent >= 0.5f && !leftLegSound)
        {
            AudioClip selectedClip = sceneName == "Village" ? villageClip : libClip;
            PlaySound(selectedClip);
            leftLegSound = true;        // 0.5 ���൵���� �Ҹ��� ��������� ǥ��
        }

        // 0.95 ���൵���� �� �� �Ҹ��� ���
        if (animationPercent >= 0.95f && !rightLegSound)
        {
            AudioClip selectedClip = sceneName == "Village" ? villageClip : libClip;
            PlaySound(selectedClip);
            rightLegSound = true;       // 0.95 ���൵���� �Ҹ��� ��������� ǥ��
        }
    }

    // �Ҹ��� ����ϴ� �Լ�
    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
