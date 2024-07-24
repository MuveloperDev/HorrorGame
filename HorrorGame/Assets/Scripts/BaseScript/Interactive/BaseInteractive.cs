using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enum;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

[RequireComponent(typeof(OutlineEffect))]
public class BaseInteractive : MonoBehaviour, IInteractive
{
    [Header("[ Animation ]")]
    [SerializeField] protected Animator _anim;

    [Header("[ OutlineEffect ]")]
    [SerializeField] protected OutlineEffect _outline;

    [Header("[ Sounds ]")]
    [SerializeField] protected SerializeDictionary<ESoundsType_In, AudioClip> _sounds = new();

    protected CancellationTokenSource _cts;
    protected virtual void Awake() 
    { 
        Initialize();
    }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    protected virtual void LateUpdate() { }
    protected virtual void OnDestroy() { }

    protected virtual void Initialize() 
    {
        _cts = new();
        _cts.RegisterRaiseCancelOnDestroy(this);

        _outline = GetComponent<OutlineEffect>();
    }

    #region [ Animation ]
    protected virtual void PlayAnimation(EInteractiveAnimationType type)
    {
        _anim.Play(type.ToString());
    }

    #endregion

    public virtual void Interaction()
    {
        Debug.Log("Interaction ! ");
    }

    public BaseInteractive GetBaseInteractive() => this;

    public virtual void OnPointerEnter()
    {
        _outline.ApplyOutline();
    }

    public virtual void OnPointerExit()
    {
        _outline.RemoveOutline();
    }
}
