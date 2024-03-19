using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LastBossGroundPound : MonoBehaviour
{
    [HideInInspector] public bool isGroundPound = false;

    [SerializeField] private DecalProjector attackRange;        // ���� ������ ��Ÿ���� ������Ʈ
    [SerializeField] private AnimationCurve upSpeed;            // �ö󰡴� �ӵ�
    [SerializeField] private Animator wing;                     // ������ �����ϴ� �ִϸ�����
    [SerializeField] private GameObject explosion;              // ���� ȿ�� ������Ʈ
    [SerializeField] private float attackRangeSize = 35.0f;     // ���� ���� �� �ִ� ������
    [SerializeField] private float rotateTime = 2.0f;           // �Ʒ��� ���������� ȸ�� �ӵ� (��)

    private Animator animator;
    private LastBossAction lastBossAction;  // ���� ������ �ൿ�� ó���ϴ� ������Ʈ

    public void Start()
    {
        Debug.Assert(wing != null, "Error (Null Reference) : ������ �����ϴ� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        lastBossAction = GetComponent<LastBossAction>();
        Debug.Assert(animator != null, "Error (Null Reference) : ���� ���� �׼� �� ������Ʈ�� �������� �ʽ��ϴ�.");
    }

    public IEnumerator StartAction()
    {
        isGroundPound = true;
        wing.enabled = false;
        lastBossAction.isAction = true;

        // ���� ���� ��� �Ϻ��ϰ� ���������� ���
        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f)
            {
                animator.enabled = false;
                break;
            }
            
            yield return null;
        }

        // õõ�� ���� �ö�
        float elapsedTime = 0f, maxSpeed = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (transform.position.y >= 50)
                break;

            maxSpeed = Mathf.Max(upSpeed.Evaluate(elapsedTime), maxSpeed);
            transform.position += Vector3.up * maxSpeed;


            yield return null;
        }

        float startTime = Time.time;
        Vector3 defaultRotation = transform.rotation.eulerAngles;

        // õõ�� �Ʒ� �������� ȸ��
        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(new Vector3(0, defaultRotation.y, defaultRotation.z)),
                    Quaternion.Euler(new Vector3(180, defaultRotation.y, defaultRotation.z)),
                    upSpeed.Evaluate(elapsedTime));
            }

            else
                break;

            yield return null;
        }

        startTime = Time.time;
        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                // ���� ���� ���� Ŀ������ ����
                float size = Mathf.Lerp(0, attackRangeSize, elapsedTime);

                attackRange.size = new Vector3(size, size, attackRange.size.z);
            }

            else
                break;

            yield return null;
        }

        // ������ ������
        elapsedTime = 0f; maxSpeed = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (transform.position.y < -1.0f)
                break;

            maxSpeed = Mathf.Max(upSpeed.Evaluate(elapsedTime), maxSpeed);
            transform.position += Vector3.down * maxSpeed;

            yield return null;
        }

        // �ٽ� ���� ���·� ���ư�
        startTime = Time.time;
        attackRange.size = new Vector3(0, 0, attackRange.size.z);

        GameObject newExplosion = Instantiate(explosion, transform);
        newExplosion.transform.parent = null;
        Destroy(newExplosion, 3f);

        while (true)
        {
            elapsedTime = (Time.time - startTime) / rotateTime;

            if (elapsedTime < rotateTime)
            {
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(new Vector3(180, defaultRotation.y, defaultRotation.z)),
                    Quaternion.Euler(new Vector3(0, defaultRotation.y, defaultRotation.z)),
                    upSpeed.Evaluate(elapsedTime) / 7.5f);

                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 10, upSpeed.Evaluate(elapsedTime) / 7.5f), transform.position.z);
            }

            else
                break;

            yield return null;
        }

        animator.enabled = true;
        wing.enabled = true;
        isGroundPound = false;
    }
}