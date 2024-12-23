using UnityEngine;

public class AC_Skeleton : AnimationController
{
    public enum State
    {
        Idle,
        Walk1,
        Walk2,
        Run,
        LookLeft,
        LookRight,
        Death,
        Attack1,
        Attack2,
        Hit1,
        Hit2
    }

    [Header("[ STATE ]")]
    [SerializeField] private State _prevState;
    [SerializeField] private State _state;

    public override void UpdateAnimation<State>(State nextState)
    {
        if (_state.Equals(nextState)) return;

        base.UpdateAnimation(nextState);
        _prevState = _state;
        object state = (object)nextState;
        _state = (AC_Skeleton.State)state;
    }
    // ????? ??????
    // ????????
    //???????????????????????
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateAnimation(State.Walk1);
        }
    }
}
