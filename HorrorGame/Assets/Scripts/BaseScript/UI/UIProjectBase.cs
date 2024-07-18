using Cysharp.Threading.Tasks;
using Enum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class UIProjectBase : UIBase
{
    [Header("Animation")]
    [SerializeField] private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _cts = new();
        _cts.RegisterRaiseCancelOnDestroy(this);
        _animator = GetComponent<Animator>();
        UIIngameContentMgr.Instance.inventory = this;
        gameObject.SetActive(false);
    }
    protected override void AfterShow()
    {
        base.AfterShow();
        _animator.Play(EMainPanel.Show.ToString(), 0);
    }
    protected async override UniTask BeforeHide()
    {
        await base.BeforeHide();
        _animator.Play(EMainPanel.Hide.ToString(), 0);

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName(EMainPanel.Hide.ToString()) || _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    }

    protected override void AfterHide()
    {
        base.AfterHide();
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }
}
