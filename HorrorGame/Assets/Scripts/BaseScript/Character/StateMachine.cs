using UnityEngine;

public class StateMachine : MonoBehaviour
{

    protected virtual void Awake(){}
    protected virtual void Start(){}
    protected virtual void Update(){}
    protected virtual void OnDestroy(){}
    protected virtual void OnEnable(){}
    protected virtual void OnDisable(){}
    protected virtual void FixedUpdate(){}
    protected virtual void LateUpdate(){}

    protected virtual void UpdateState<T>(T nextState) where T : System.Enum { }
}