using UnityEngine;

public class SkeletonController : PhysicsController
{
    [Header("[ RESOURCES ]")]
    [SerializeField] private AC_Skeleton _ac;

    protected override void Awake()
    {
        base.Awake();
        _movementOptions.walkSpeed = 12f;
        _ac = GetComponentInChildren<AC_Skeleton>();
    }

    public void Move()
    {
        _ac.UpdateAnimation(AC_Skeleton.State.Walk1);
        _curState = State.Walk;
    }

    public void StopMove()
    {
        _ac.UpdateAnimation(AC_Skeleton.State.Idle);
        _curState = State.None;
    }
    public void Jumpp()
    {
        _ac.UpdateAnimation(AC_Skeleton.State.Idle);
        _curState = State.Jump;
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            Move();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jumpp();
        }
    }
}