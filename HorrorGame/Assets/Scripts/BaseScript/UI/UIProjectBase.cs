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


    public Material dissolveMaterial;
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
        _=StartDissolveEffect();
    }
    protected async override UniTask BeforeHide()
    {
        // Hide �ִϸ��̼� ���ķ� ó��
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
    private float dissolveThreshold = 0f; // �ʱⰪ
    private bool isIncreasing = true; // �� ���� ���θ� �Ǵ��ϴ� �÷���

    // �ִϸ��̼� �ӵ�
    public float animationDuration = 1f;
    private async UniTaskVoid StartDissolveEffect()
    {
        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / animationDuration;

                if (isIncreasing)
                {
                    dissolveThreshold = Mathf.Lerp(0f, 1f, t);
                }
                else
                {
                    dissolveThreshold = Mathf.Lerp(1f, 0f, t);
                }

                // ��Ƽ������ DissolveThreshold �� ������Ʈ
                dissolveMaterial.SetFloat("_DissolveThreshold", dissolveThreshold);
                Debug.Log($"_DissolveThreshold: {dissolveThreshold}");
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            isIncreasing = !isIncreasing;
        }
    }

    // Todo ���콺 ���� Ȯ�� �ؾ���.
}
