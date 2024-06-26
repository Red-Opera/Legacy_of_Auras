using System.Collections;
using UnityEngine;

public class LastBossReady : MonoBehaviour
{
    [SerializeField] private Animator wingAnimator;
    [SerializeField] private Transform rigTransform;
    [SerializeField] private GameObject showInfo;
    
    private Animator animator;
    private bool isJumpStart = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메니션이 존재하지 않습니다.");
        Debug.Assert(wingAnimator != null, "Error (Null Reference) : 날개 애니메이션이 존재하지 않습니다.");
        Debug.Assert(showInfo != null, "Error (Null Reference) : 정보 오브젝트가 존재하지 않습니다.");
    }

    public void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Wait") && !isJumpStart)
            StartCoroutine(StartReady());

        if (state.IsName("Ready"))
            transform.position += new Vector3(0, Time.deltaTime * 2, 0);

        if (state.IsName("Ready") && state.normalizedTime >= 0.6f)
            StartCoroutine(StartGame());
    }

    private IEnumerator StartReady()
    {
        isJumpStart = true;

        float currentTime = 0;
        showInfo.SetActive(true);

        Transform showInfoBack = showInfo.transform.GetChild(0);
        float defaultSize = showInfoBack.localScale.y;

        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;
            showInfoBack.localScale = new Vector3(showInfoBack.localScale.x, Mathf.Lerp(0, defaultSize, currentTime * 2), showInfoBack.localScale.z);

            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        currentTime = 0;
        while (currentTime < 0.25f)
        {
            currentTime += Time.deltaTime;
            showInfoBack.localScale = new Vector3(showInfoBack.localScale.x, Mathf.Lerp(defaultSize, 0, currentTime * 4), showInfoBack.localScale.z);

            yield return null;
        }
        showInfo.SetActive(false);

        wingAnimator.SetTrigger("StartJump");
        animator.SetTrigger("Ready");

        animator.applyRootMotion = false;
    }

    private IEnumerator StartGame()
    {
        while (true)
        {
            if (wingAnimator.GetCurrentAnimatorStateInfo(0).IsName("LassBossReady"))
                break;

            yield return null;
        }

        wingAnimator.SetTrigger("StartGame");
    }
}
