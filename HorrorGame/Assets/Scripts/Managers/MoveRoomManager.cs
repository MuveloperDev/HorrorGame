using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveRoomManager : MonoBehaviour
{
    public enum MoveVibrateType
    {
        Moving = 2,
        Arrive = 3,
        Start =4,
    }

    [Header("[ ROOM LIST ]")]
    [SerializeField] private List<Room> _rooms = new();
    [SerializeField] private Dictionary<int, Room> _roomDic= new();

    [SerializeField] private int _nextRoomIdxTest;
    [Header("[ ROOM INFO ]")]
    [SerializeField] private int _nextRoomIdx;
    [SerializeField] private int _curRoomIdx;
    [SerializeField] private Room _curRoom;
    [SerializeField] private Room _nextRoom;

    [Header("[ OFFSET POS ]")]
    [SerializeField] private OffsetPosition offsetPos = new();

    private CancellationTokenSource _cts;
    private void Awake()
    {
        _cts = new CancellationTokenSource();
        _cts.RegisterRaiseCancelOnDestroy(this);

        var load = new JsonLoader();
        load.Load();

        var childs = transform.GetComponentsInChildren<Room>(true);
        _rooms = childs.ToList();
        for (int i = 0; i < _rooms.Count; i++)
        {
            var room = _rooms[i];
            _roomDic[i] = room;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetRoom(_nextRoomIdxTest);
            MoveToRoom();
        }
    }

    public void SetRoom(int nextRoomIdx)
    {
        if (_curRoomIdx == nextRoomIdx)
        {
            Debug.LogError("Equal next room id and cur room id.");
            nextRoomIdx++;
        }
        var room = _roomDic[nextRoomIdx];
        if (null == room)
        {
            Debug.LogError("Room is null");
            return;
        }
        _nextRoomIdx = nextRoomIdx;
        _nextRoom = room;
    }

    [SerializeField] private bool isMoveRoom = false;
    protected async UniTask MoveToRoom()
    {
        if (isMoveRoom) return;
        isMoveRoom = true;
        float timePassed = 0f;

        _nextRoom.gameObject.SetActive(true);
        var next = _nextRoom.transform;
        var cur = _curRoom.transform;

        bool isDirR = _nextRoomIdx > _curRoomIdx;
        _nextRoom.transform.position = isDirR ? offsetPos.nextRoomPos : offsetPos.prevRoomPos;
        Vector3 moveDirVecCurRoom = isDirR ? offsetPos.prevRoomPos : offsetPos.nextRoomPos; 

        CameraShakeManager.Instance.PlayCameraShake((int)MoveVibrateType.Start);
        var data = GetData((int)MoveVibrateType.Start);
        if (data == null)
        {
            Debug.LogError($"Camera shake data is null.");
            return;
        }

        await UniTask.WaitForSeconds(data.TotalDuration);
        CameraShakeManager.Instance.PlayCameraShake((int)MoveVibrateType.Moving);
        while (timePassed < 5)
        {
            await UniTask.Yield(_cts.Token);

            // 현재 프레임 카운트를 가져오고, 지난 프레임 카운트와의 차이를 계산

            timePassed += Time.deltaTime;

            float remainingTime = Mathf.Max(5 - timePassed, 0.01f);
            float remainingDistance = Vector3.Distance(_nextRoom.transform.position, offsetPos.curRoomPos);
            float speed = remainingDistance / remainingTime;

            _nextRoom.transform.position = Vector3.MoveTowards(_nextRoom.transform.position, offsetPos.curRoomPos, speed * Time.deltaTime);
            _curRoom.transform.position = Vector3.MoveTowards(_curRoom.transform.position, moveDirVecCurRoom, speed  * Time.deltaTime);
        }
        CameraShakeManager.Instance.PlayCameraShake((int)MoveVibrateType.Arrive);
        _nextRoom.transform.position = offsetPos.curRoomPos;
        _curRoom.gameObject.SetActive(false);
        _curRoom = _nextRoom;
        _curRoomIdx = _nextRoomIdx;
        isMoveRoom =false;
    }

    private CameraShakeData GetData(int id)
    {
        var data = CameraShakeData.table[id];
        if (data == null) return null;
        return data;
    }

    [System.Serializable]
    public class OffsetPosition
    {
        public Vector3 curRoomPos = Vector3.zero;
        public Vector3 nextRoomPos = new Vector3(30, 0, 0);
        public Vector3 prevRoomPos = new Vector3(-30, 0, 0);
    }
}
