using Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonBase : UIEventHandler
{
    [Header("[ RESOURCES ]")]
    [SerializeField] private AudioSourceController _audioSourceController;
    [SerializeField] private Animator _animator;

    [Header("[ INFO ]")]
    [SerializeField] private ESoundTypes _soundType = ESoundTypes.Button;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _soundType = ESoundTypes.Button;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _animator.Play("Highlighted", 0);
        Debug.Log("Enter");
        _audioSourceController.PlayOneShot(_soundType, (int)ESoundsType_Button.Hover);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _animator.Play("Normal", 0);
        Debug.Log("Exit");
        base.OnPointerExit(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _animator.Play("Normal", 0);
        Debug.Log("Click");
        _audioSourceController.PlayOneShot(_soundType, (int)ESoundsType_Button.Click);
        base.OnPointerDown(eventData);
    }
}
