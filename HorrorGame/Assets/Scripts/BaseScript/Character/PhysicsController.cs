using UnityEngine;

public class PhysicsController : StateMachine
{
    public enum State
    {
        None,
        Walk,
        Run,
        Jump,
    }

    [Header("[ RESOURCES ]")]
    [SerializeField] protected Rigidbody _rb;

    [Header("[ MOVEMENT SETTINGS ]")]
    [SerializeField] protected MovementOptions _movementOptions;

    [Header("[ STATE ]")]
    [SerializeField] protected State _curState;

    [Header("[ DIRECTION INFO ]")]
    [SerializeField] protected bool _isMove = false;
    [SerializeField] protected Vector3 _dir;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _movementOptions = new MovementOptions();
        _curState = State.None;
        _dir = Vector3.zero;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _movementOptions = null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_curState == State.None) return;
        Move(_dir);
    }

    protected virtual void Move(Vector3 direction)
    {
        switch (_curState)
        {
            case State.Walk:
                ApplyMovement(direction, _movementOptions.walkSpeed);
                RotateTowards(direction);
                break;

            case State.Run:
                ApplyMovement(direction, _movementOptions.runSpeed);
                RotateTowards(direction);
                break;

            case State.Jump:
                ApplyMovement(direction, _movementOptions.runSpeed);
                RotateTowards(direction);
                if (IsGrounded())
                {
                    Jump();
                }
                break;

            case State.None:
            default:
                break;
        }
        if (transform.position == _dir)
        {
            _isMove = false;
            _dir = Vector3.zero;
            _curState = State.None;
        }
    }

    protected virtual void ApplyMovement(Vector3 direction, float speed)
    {
        if (!_isMove) _isMove = true;

        Vector3 velocity = direction.normalized * speed;
        _rb.MovePosition(_rb.position + velocity * Time.deltaTime);
    }

    protected virtual void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _movementOptions.rotSpeed * Time.deltaTime);
    }

    protected virtual void Jump()
    {
        Vector3 jumpVelocity = new Vector3(0, _movementOptions.jumpForce, 0);
        _rb.AddForce(jumpVelocity, ForceMode.Impulse);
    }

    protected virtual bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    protected virtual void SetState(State newState)
    {
        _curState = newState;
    }

    [System.Serializable]
    public class MovementOptions
    {
        public float walkSpeed = 2f;
        public float runSpeed = 5f;
        public float jumpForce = 5f;
        public float rotSpeed = 20f;
    }
}