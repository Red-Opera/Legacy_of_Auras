using UnityEngine;
using UnityEngine.SceneManagement;

// �÷��̾ �ȴµ� �Ҹ��� ���� ���� ��ũ��Ʈ
// ���� �ʿ��� ��: �÷��̾� �ȱ� �Ҹ� Ŭ��
public class PlayerWalkSound : MonoBehaviour
{
    public AudioClip villageClip;           // �������� ����� �÷��̾� �ȱ� Ŭ��
    public AudioClip libClip;               // ���������� ����� �÷��̾� �ȱ� Ŭ��
    public AudioClip desertClip;            // �縷���� ����� �÷��̾� �ȱ� Ŭ��
    public AudioClip waterClip1;            // ������ �ȴ� �Ҹ� 1
    public AudioClip waterClip2;            // ������ �ȴ� �Ҹ� 2

    private Animator animator;              // �÷��̾��� �ִϸ�����
    private AudioSource audioSource;        // ������� ����ϱ� ���� ����

    private bool leftLegSound = false;      // 0.5 ���൵���� �Ҹ��� ����ߴ��� ����
    private bool rightLegSound = false;     // 0.95 ���൵���� �Ҹ��� ����ߴ��� ����

    private bool isWaterStep = false;        // ���� �� ���̸� �Ȱ� �ִ��� ����

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;                                  // ���� ���� �̸��� �����ɴϴ�.
        float animationPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;    // ���� �ִϸ��̼� ���൵�� ������

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

    // �Ҹ��� ����ϴ� �Լ�
    private void PlaySound(AudioClip clip)
    {
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
