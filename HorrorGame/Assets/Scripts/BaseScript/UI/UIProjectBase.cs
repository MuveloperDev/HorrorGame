using Cysharp.Threading.Tasks;
using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CanvasGroup))]
public class UIProjectBase : UIBase
{
    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private SerializeDictionary<EMainPanel, AudioClip> _audioClip = new();

    [Header("Buttons")]
    [SerializeField] private UIButtonBase _btnClose;
    protected override void Awake()
    {
        base.Awake();
        _cts = new();
        _cts.RegisterRaiseCancelOnDestroy(this);
        _animator = GetComponent<Animator>();
        IngameUIManager.Instance.mainPanel = this;
        gameObject.SetActive(false);
        _btnClose.onPointerUpEvent += (pointerEventData) => Hide();
    }
    protected override void AfterShow()
    {
        base.AfterShow();
        _animator.Play(EMainPanel.Show.ToString(), 0);
    }
    protected async override UniTask BeforeHide()
    {
        // Hide 애니메이션 이후로 처리
        await base.BeforeHide();
        _animator.Play(EMainPanel.Hide.ToString(), 0);

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(0).IsName(EMainPanel.Hide.ToString()) || _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    }

    protected override void AfterHide()
    {
        base.AfterHide();
        //Cursor.lockState = CursorLockMode.Locked;
        FristPersonController.instance.isUpdate = true;
    }


    // Todo 마우스 만들어서 확인 해야함.
}
