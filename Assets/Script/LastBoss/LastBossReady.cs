using System.Collections;
using UnityEngine;

public class LastBossReady : MonoBehaviour
{
    [SerializeField] private Animator wingAnimator;
    [SerializeField] private Transform rigTransform;
    
    private Animator animator;
    private bool isJumpStart = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메니션이 존재하지 않습니다.");
        Debug.Assert(wingAnimator != null, "Error (Null Reference) : 날개 애니메이션이 존재하지 않습니다.");
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

        yield return new WaitForSeconds(3.0f);

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
