using Cysharp.Threading.Tasks;
using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 오디오 소스와 같이 관리할 컨트롤러
[SerializeField]
[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour
{
    [Header("[ RESOURCES ]")]
    [SerializeField] AudioSource _audioSource;

    [Header("[ OPTIONS ]")]
    [SerializeField] protected EAudioPlayType _audioPlayType;

    private CancellationTokenSource _cts;
    private ResourcesManager _resources;
    private void Awake()
    {
        _cts = new();
        _cts.RegisterRaiseCancelOnDestroy(this);
        _audioSource = GetComponent<AudioSource>();
        _resources = ResourcesManager.Instance;
    }

    #region [ Audio ]
    public async void PlaySound(ESoundTypes st, int st_category, Action onComplete = null)
    {
        if (null == _audioSource)
            return;

        var clip = _resources.GetAudioClip(st, st_category);
        if (clip == null) 
            return;

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

    public async void PlaySound(ESoundTypes st, int st_category, EAudioPlayType audioPlayType, Action onComplete = null)
    {
        if (null == _audioSource)
            return;

        var clip = _resources.GetAudioClip(st, st_category);
        if (clip == null)
            return;

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

    public async void PlayOneShot(ESoundTypes st, int st_category, Action onComplete = null)
    {
        if (null == _audioSource)
            return;


        var clip = _resources.GetAudioClip(st, st_category);
        if (clip == null)
            return;

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


}
