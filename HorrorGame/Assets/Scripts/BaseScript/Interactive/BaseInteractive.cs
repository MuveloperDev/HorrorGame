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
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected EAudioPlayType _audioPlayType;
    [SerializeField] protected SerializeDictionary<ESoundsType, AudioClip> _sounds = new();

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

    #region [ Audio ]
    protected async virtual void PlaySound(ESoundsType type, Action onComplete = null)
    {
        if (null == _audioSource)
            return;

        if (!_sounds.ContainsKey(type))
        {
            Debug.LogError("Invalid key");
            return;
        }

        var clip = _sounds[type];
        _audioSource.clip = clip;
        switch (_audioPlayType)
        {
            case EAudioPlayType.OneShot:
                _audioSource.loop = false;

                break;
            case EAudioPlayType.Loop:
                _audioSource.loop = true;
                break;
        }
        _audioSource.Play();

        if (EAudioPlayType.Loop == _audioPlayType)
            return;
        if (null == onComplete)
            return;

        await WaitForAudioToEnd(clip.length);
    }
    protected async virtual void PlaySound(ESoundsType soundsType, EAudioPlayType audioPlayType, Action onComplete = null)
    {
        if (null == _audioSource)
            return;

        if (!_sounds.ContainsKey(soundsType))
        {
            Debug.LogError("Invalid key");
            return;
        }

        var clip = _sounds[soundsType];
        _audioSource.clip = clip;
        switch (audioPlayType)
        {
            case EAudioPlayType.OneShot:
                _audioSource.loop = false;
                break;
            case EAudioPlayType.Loop:
                _audioSource.loop = true;
                break;
        }
        _audioSource.Play();

        if (EAudioPlayType.Loop == audioPlayType)
            return;
        if (null == onComplete)
            return;

        await WaitForAudioToEnd(clip.length);
    }
    protected async virtual void PlaySoundOneshot(ESoundsType type, Action onComplete = null)
    {
        if (null == _audioSource)
            return;

        if (!_sounds.ContainsKey(type))
        {
            Debug.LogError("Invalid key");
            return;
        }

        var clip = _sounds[type];
        _audioSource.clip = clip;
        _audioSource.PlayOneShot(clip);

        if (null == onComplete)
            return;

         await WaitForAudioToEnd(clip.length);
    }

    protected async virtual UniTask WaitForAudioToEnd(float ClipLength, Action onComplete = null)
    {
        if (null == onComplete)
            return;

        await UniTask.WaitForSeconds(ClipLength, false, PlayerLoopTiming.Update, _cts.Token, true);

        onComplete?.Invoke();
    }
    #endregion

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
