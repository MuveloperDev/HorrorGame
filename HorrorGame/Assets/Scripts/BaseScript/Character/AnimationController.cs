using System;
using UnityEngine;

public class AnimationController : StateMachine
{
    [Header("[ ANIMATOR ]")]
    [SerializeField] private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        CahceAnimator();

        void CahceAnimator()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_animator != null) return;

            Debug.LogError($"Animator is null in [{gameObject.name}]");
        }
    }

    protected override void UpdateState<T>(T nextState)
    {
        base.UpdateState(nextState);
        if (_animator == null) return;

        float transitionDuration = 0.2f; // ??? ?©£??? ???? (??γα ???? ???? ????)
        _animator.CrossFade(nextState.ToString(), transitionDuration, 0);
    }

    public virtual void UpdateAnimation<T>(T nextState) where T : System.Enum
    {
        UpdateState(nextState);
    }

}