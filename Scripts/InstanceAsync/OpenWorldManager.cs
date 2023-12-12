using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum eAssetSyncState
{
    Unload,
    TryLoad,
    LoadFail,
    LoadSuccess,
    Clear,
}
public enum eDeployState
{
    Undeploy,
    TryDeploy,
    DeployFail,
    DeploySuccess,
    Unactive,
    Dead,
    Destroy,
}

public class OpenWorldManager : MonoBehaviour
{
    [field: Header("시작 위치")]
    [field: SerializeField] public Vector3 MinPosition { get; private set; }
    [field: Header("최대 위치\n(무조건 시작 위치보다 큰 값을 넣어야 합니다.)")]
    [field: SerializeField] public Vector3 MaxPosition { get; private set; }
    [field: Header("단위 영역 크기")]
    [field: SerializeField] public Vector3 DivideUnit { get; private set; }
    [field: Header("정적 내부 오브젝트")]
    [field: SerializeField] public List<InstanceData> StaticInstData { get; private set; }
    [field: Header("동적 내부 오브젝트(NPC, 몬스터)")]
    [field: SerializeField] public List<InstanceData> DynamicInstData { get; private set; }
    [field: Header("표현할 청크의 거리")]
    [field: SerializeField] public int VisibleStaticChunkDistance { get; private set; }
    [field: SerializeField] public int VisibleDynamicChunkDistance { get; private set; }


    private RootChunk _root;
    [Header("테스트용 플래이어")]
    [SerializeField] private Transform _player;
    public Coroutine UpdateWorldCoroutine { get; private set; }

    AsyncOperationHandle<GameObject> instHandleLocation;

    private Dictionary<string, Queue<GameObject>> _objPool = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        _root = new RootChunk(MinPosition, MaxPosition, StaticInstData, DynamicInstData, DivideUnit, this);
    }

    private void Start()
    {
        if (_player != null)
            SetPlayer(_player);
    }

    public IEnumerator UpdatePlayerPosition(Transform tr)
    {
        _root.InitPlayerPosition(_player.position);
        while (tr != null)
        {
            _root.UpdatePlayerPosition(tr.position);

            yield return null;
        }
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
        if (UpdateWorldCoroutine != null)
            StopCoroutine(UpdateWorldCoroutine);
        UpdateWorldCoroutine = StartCoroutine(UpdatePlayerPosition(_player));
    }

    // InstanceData 에셋을 로드하고 배치하는 함수들 //
    public void DeactiveInstData(BaseChunk chunk)
    {
        foreach (var data in chunk.StaticInstDatas)
        {
            if (data.State == eDeployState.DeploySuccess)
            {
                data.DeployedObj.SetActive(false);
                data.DeployedObj = null;
                data.State = eDeployState.Unactive;
            }
        }
    }
    public void LoadInstData(List<InstanceData> datas)
    {
        foreach (var data in datas)
        {
            if (data.State == eDeployState.Undeploy || data.State == eDeployState.Unactive)
            {
                LoadAndDeployAsset(data);
            }
        }
    }
    private void LoadAndDeployAsset(InstanceData data)
    {
        if (!data.AssetRef.RuntimeKeyIsValid())
        {
            instHandleLocation = Addressables.LoadAssetAsync<GameObject>(data.AssetRef);
            instHandleLocation.Completed += (OperationLoadAsset) =>
            {
                OnLoadSuccess(OperationLoadAsset, data);
            };
        }
        else
        {
            if (_objPool.ContainsKey(data.AssetRef.AssetGUID))
            {
                GameObject obj = _objPool[data.AssetRef.AssetGUID].Dequeue();
                _objPool[data.AssetRef.AssetGUID].Enqueue(obj);
                for (int i = 0; i < _objPool[data.AssetRef.AssetGUID].Count; i++)
                {
                    if (!obj.activeInHierarchy)
                        break;
                    obj = _objPool[data.AssetRef.AssetGUID].Dequeue();
                    _objPool[data.AssetRef.AssetGUID].Enqueue(obj);
                }

                if (obj.activeInHierarchy)
                {
                    DeployAsset(data);
                }
                else
                {
                    obj.transform.localScale = data.transform.localScale;
                    obj.transform.SetPositionAndRotation(data.transform.position, data.transform.rotation);
                    obj.SetActive(true);
                    data.DeployedObj = obj;
                    data.State = eDeployState.DeploySuccess;
                    data.Initialize();
                }
            }
            else
            {
                DeployAsset(data);
            }
        }
    }
    private void DeployAsset(InstanceData data)
    {
        var syncOpHandle = Addressables.InstantiateAsync(data.AssetRef, data.transform.position, data.transform.rotation);
        syncOpHandle.Completed += (op) =>
        {
            OnInstantiateSuccess(op, data);
        };
    }
    private void OnLoadSuccess(AsyncOperationHandle<GameObject> operationLoadAsset, InstanceData data)
    {
        if (operationLoadAsset.Status == AsyncOperationStatus.Succeeded)
        {
            //var loadedObj = operationLoadAsset.Result;
            DeployAsset(data);
        }
        else if (operationLoadAsset.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogWarning($"Load Asset Fail");
            data.State = eDeployState.DeployFail;
        }
    }
    private void OnInstantiateSuccess(AsyncOperationHandle<GameObject> operationLoadAsset, InstanceData data)
    {
        if (operationLoadAsset.Status == AsyncOperationStatus.Succeeded)
        {
            if (!_objPool.ContainsKey(data.AssetRef.AssetGUID))
            {
                _objPool.Add(data.AssetRef.AssetGUID, new Queue<GameObject>());
            }
            _objPool[data.AssetRef.AssetGUID].Enqueue(operationLoadAsset.Result);
            data.State = eDeployState.DeploySuccess;
            operationLoadAsset.Result.transform.localScale = data.transform.localScale;
            operationLoadAsset.Result.transform.SetPositionAndRotation(data.transform.position, data.transform.localRotation);
            data.DeployedObj = operationLoadAsset.Result;
            data.Initialize();
        }
        else if (operationLoadAsset.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogWarning($"instantiate Asset Fail");
            data.State = eDeployState.DeployFail;
        }
    }
    public void ReleaseLoadedAsset(AssetReference assetref)
    {
        while (_objPool[assetref.AssetGUID].Count > 0)
        {
            var obj = _objPool[assetref.AssetGUID].Dequeue();
            if (!Addressables.ReleaseInstance(obj))
            {
                _objPool[assetref.AssetGUID].Enqueue(obj);
                Debug.LogWarning($"Can't Release Instance! => GUID:{assetref.AssetGUID}\tName:{assetref.Asset.name}");
                break;
            }
        }
        if (_objPool[assetref.AssetGUID].Count == 0)
        {
            _objPool[assetref.AssetGUID].Clear();
            _objPool.Remove(assetref.AssetGUID);

            Addressables.Release(assetref);
        }
    }
    /**********************************************/

    private void OnDrawGizmosSelected()
    {
        if (MinPosition != MaxPosition)
        {
            var length = MaxPosition - MinPosition;
            Vector3[] vertices = new Vector3[] {
                MinPosition + new Vector3(0, length.y, 0),
                MinPosition + new Vector3(length.x, length.y, 0),
                MinPosition + new Vector3(length.x, 0, 0),
                MinPosition,
                MinPosition + new Vector3(0, length.y, length.z),
                MaxPosition,
                MinPosition + new Vector3(length.x, 0, length.z),
                MinPosition + new Vector3(0, 0, length.z)
                };
            // 시계방향으로 A,B,C 점을 지정하면 BC X AC 인 외적값의 방향이 노말이 된다.
            int[] triangles = new int[]
            {
                0,1,2,
                0,2,3,
                1,5,6,
                1,6,2,
                5,4,7,
                5,7,6,
                4,0,3,
                4,3,7,
                4,5,1,
                4,1,0,
                3,2,6,
                3,6,7
            };
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireMesh(mesh);
        }
    }
}

public class BaseChunk
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }
    public float X_Min { get => Min.x; }
    public float X_Max { get => Max.x; }
    public float Y_Min { get => Min.y; }
    public float Y_Max { get => Max.y; }
    public float Z_Min { get => Min.z; }
    public float Z_Max { get => Max.z; }

    public List<InstanceData> StaticInstDatas { get; protected set; }
    public List<InstanceData> DynamicInstDatas { get; protected set; }

    public BaseChunk(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public virtual bool IsInsideChunk(Vector3 position)
    {
        if (position.x > Min.x && position.x < Max.x &&
            position.y > Min.y && position.y < Max.y &&
            position.z > Min.z && position.z < Max.z)
        {
            return true;
        }
        else
            return false;
    }
}

public class RootChunk : BaseChunk
{
    private Vector3 _divideUnit;
    public ObjectChunk[,,] Chunks { get; private set; }
    public int3 CurrentChunkIndex { get; private set; }
    private OpenWorldManager _manager;
    public RootChunk(Vector3 min, Vector3 max, List<InstanceData> staticDatas, List<InstanceData> dynamicDatas, Vector3 divideUnit, OpenWorldManager manager) : base(min, max)
    {
        _divideUnit = divideUnit;
        StaticInstDatas = staticDatas;
        DynamicInstDatas = dynamicDatas;
        _manager = manager;

        // Make ObjectChunks
        var length = max - min;
        int x = Mathf.CeilToInt(length.x / _divideUnit.x);
        int y = Mathf.CeilToInt(length.y / _divideUnit.y);
        int z = Mathf.CeilToInt(length.z / _divideUnit.z);
        Chunks = new ObjectChunk[x, y, z];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    Chunks[i, j, k] = new ObjectChunk(
                        this,
                        new Vector3(
                            min.x + i * _divideUnit.x,
                            min.y + j * _divideUnit.y,
                            min.z + k * _divideUnit.z),
                        new Vector3(
                            i + 1 >= x ? max.x : (min.x + (i + 1) * _divideUnit.x),
                            j + 1 >= y ? max.y : (min.y + (j + 1) * _divideUnit.y),
                            k + 1 >= z ? max.z : (min.z + (k + 1) * _divideUnit.z)));
                }
            }
        }

        foreach (InstanceData data in StaticInstDatas)
        {
            var index = GetIndex(data.transform.position);
            Chunks[index[0], index[1], index[2]].StaticInstDatas.Add(data);
        }
        foreach (InstanceData data in DynamicInstDatas)
        {
            var index = GetIndex(data.transform.position);
            Chunks[index[0], index[1], index[2]].DynamicInstDatas.Add(data);
        }
        foreach (var chunk in Chunks)
        {
            chunk.InitSubChunk();
        }
    }

    public void InitPlayerPosition(Vector3 position)
    {
        var index = GetIndex(position);
        CurrentChunkIndex = index;

        HashSet<ObjectChunk> loadChunks = new HashSet<ObjectChunk>();

        // Static Inst Data
        for (int i = -_manager.VisibleStaticChunkDistance; i < _manager.VisibleStaticChunkDistance; i++)
        {
            for (int j = -_manager.VisibleStaticChunkDistance; j < _manager.VisibleStaticChunkDistance; j++)
            {
                for (int k = -_manager.VisibleStaticChunkDistance; k < _manager.VisibleStaticChunkDistance; k++)
                {
                    // Make Sphere Area
                    if (i * i + j * j + k * k >= _manager.VisibleStaticChunkDistance * _manager.VisibleStaticChunkDistance)
                        continue;

                    // 새로운 청크
                    if (CurrentChunkIndex.x + i < 0 || CurrentChunkIndex.y + j < 0 || CurrentChunkIndex.z + k < 0)
                    { }
                    else if (CurrentChunkIndex.x + i > Chunks.GetUpperBound(0) || CurrentChunkIndex.y + j > Chunks.GetUpperBound(1) || CurrentChunkIndex.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        loadChunks.Add(Chunks[index.x + i, index.y + j, index.z + k]);
                }
            }
        }
        // 업데이트
        foreach (var chunk in loadChunks)
        {
            LoadChunkStaticInstData(chunk);
        }

        // Dynamic Inst Data
        loadChunks.Clear();
        for (int i = -_manager.VisibleDynamicChunkDistance; i < _manager.VisibleDynamicChunkDistance; i++)
        {
            for (int j = -_manager.VisibleDynamicChunkDistance; j < _manager.VisibleDynamicChunkDistance; j++)
            {
                for (int k = -_manager.VisibleDynamicChunkDistance; k < _manager.VisibleDynamicChunkDistance; k++)
                {
                    // Make Sphere Area
                    if (i * i + j * j + k * k >= _manager.VisibleDynamicChunkDistance * _manager.VisibleDynamicChunkDistance)
                        continue;

                    // 새로운 청크
                    if (CurrentChunkIndex.x + i < 0 || CurrentChunkIndex.y + j < 0 || CurrentChunkIndex.z + k < 0)
                    { }
                    else if (CurrentChunkIndex.x + i > Chunks.GetUpperBound(0) || CurrentChunkIndex.y + j > Chunks.GetUpperBound(1) || CurrentChunkIndex.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        loadChunks.Add(Chunks[index.x + i, index.y + j, index.z + k]);
                }
            }
        }
        // 업데이트
        foreach (var chunk in loadChunks)
        {
            LoadChunkDynamicInstData(chunk);
        }
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        var index = GetIndex(position);
        var newChunk = Chunks[index.x, index.y, index.z];

        if (CurrentChunkIndex.x == index.x && CurrentChunkIndex.y == index.y && CurrentChunkIndex.z == index.z)
            return;

        // TODO
        // 주변 청크들도 업데이트 하기
        HashSet<ObjectChunk> unloadChunks = new HashSet<ObjectChunk>();
        HashSet<ObjectChunk> loadChunks = new HashSet<ObjectChunk>();

        // Static Inst Data
        for (int i = -_manager.VisibleStaticChunkDistance; i < _manager.VisibleStaticChunkDistance; i++)
        {
            for (int j = -_manager.VisibleStaticChunkDistance; j < _manager.VisibleStaticChunkDistance; j++)
            {
                for (int k = -_manager.VisibleStaticChunkDistance; k < _manager.VisibleStaticChunkDistance; k++)
                {
                    // Make Sphere Area
                    if (i * i + j * j + k * k >= _manager.VisibleStaticChunkDistance * _manager.VisibleStaticChunkDistance)
                        continue;

                    // 이전 청크
                    if (CurrentChunkIndex.x + i < 0 || CurrentChunkIndex.y + j < 0 || CurrentChunkIndex.z + k < 0)
                    { }
                    else if (CurrentChunkIndex.x + i > Chunks.GetUpperBound(0) || CurrentChunkIndex.y + j > Chunks.GetUpperBound(1) || CurrentChunkIndex.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        unloadChunks.Add(Chunks[CurrentChunkIndex.x + i, CurrentChunkIndex.y + j, CurrentChunkIndex.z + k]);

                    // 새로운 청크
                    if (index.x + i < 0 || index.y + j < 0 || index.z + k < 0)
                    { }
                    else if (index.x + i > Chunks.GetUpperBound(0) || index.y + j > Chunks.GetUpperBound(1) || index.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        loadChunks.Add(Chunks[index.x + i, index.y + j, index.z + k]);
                }
            }
        }
        // 중복된 청크 찾기
        HashSet<ObjectChunk> duplicateChunk = new HashSet<ObjectChunk>();
        foreach (var chunk in loadChunks)
        {
            if (unloadChunks.Contains(chunk))
                duplicateChunk.Add(chunk);
        }
        loadChunks.RemoveWhere(chunk => duplicateChunk.Contains(chunk));
        unloadChunks.RemoveWhere(chunk => duplicateChunk.Contains(chunk));
        // 업데이트
        foreach (var chunk in loadChunks)
        {
            LoadChunkStaticInstData(chunk);
        }
        foreach (var chunk in unloadChunks)
        {
            UnloadChunk(chunk);
        }

        // Dynamic Inst Data
        // 주변 청크들도 업데이트 하기
        loadChunks.Clear();
        unloadChunks.Clear();
        for (int i = -_manager.VisibleDynamicChunkDistance; i < _manager.VisibleDynamicChunkDistance; i++)
        {
            for (int j = -_manager.VisibleDynamicChunkDistance; j < _manager.VisibleDynamicChunkDistance; j++)
            {
                for (int k = -_manager.VisibleDynamicChunkDistance; k < _manager.VisibleDynamicChunkDistance; k++)
                {
                    // Make Sphere Area
                    if (i * i + j * j + k * k >= _manager.VisibleDynamicChunkDistance * _manager.VisibleDynamicChunkDistance)
                        continue;

                    // 이전 청크 기준
                    if (CurrentChunkIndex.x + i < 0 || CurrentChunkIndex.y + j < 0 || CurrentChunkIndex.z + k < 0)
                    { }
                    else if (CurrentChunkIndex.x + i > Chunks.GetUpperBound(0) || CurrentChunkIndex.y + j > Chunks.GetUpperBound(1) || CurrentChunkIndex.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        unloadChunks.Add(Chunks[CurrentChunkIndex.x + i, CurrentChunkIndex.y + j, CurrentChunkIndex.z + k]);

                    // 새로운 청크 기준
                    if (index.x + i < 0 || index.y + j < 0 || index.z + k < 0)
                    { }
                    else if (index.x + i > Chunks.GetUpperBound(0) || index.y + j > Chunks.GetUpperBound(1) || index.z + k > Chunks.GetUpperBound(2))
                    { }
                    else
                        loadChunks.Add(Chunks[index.x + i, index.y + j, index.z + k]);
                }
            }
        }
        // 중복된 청크 찾기
        duplicateChunk.Clear();
        foreach (var chunk in loadChunks)
        {
            if (unloadChunks.Contains(chunk))
                duplicateChunk.Add(chunk);
        }
        loadChunks.RemoveWhere(chunk => duplicateChunk.Contains(chunk));
        // 업데이트
        foreach (var chunk in loadChunks)
        {
            LoadChunkDynamicInstData(chunk);
        }

        CurrentChunkIndex = index;
    }

    /// <param name="position">World position</param>
    /// <returns>index of x, y, z</returns>
    public int3 GetIndex(Vector3 position)
    {
        Vector3 relativePos = (position - Min);
        // Using Mathf.Clamp to avoid "out of range exception"
        int xIndex = Mathf.Clamp(Mathf.CeilToInt(relativePos.x / _divideUnit.x), 0, Chunks.GetUpperBound(0));
        int yIndex = Mathf.Clamp(Mathf.CeilToInt(relativePos.y / _divideUnit.y), 0, Chunks.GetUpperBound(1));
        int zIndex = Mathf.Clamp(Mathf.CeilToInt(relativePos.z / _divideUnit.z), 0, Chunks.GetUpperBound(2));
        return new(xIndex, yIndex, zIndex);
    }
    public void UnloadChunk(ObjectChunk chunk)
    {
        _manager.DeactiveInstData(chunk);
    }
    public void LoadChunkStaticInstData(ObjectChunk chunk)
    {
        _manager.LoadInstData(chunk.StaticInstDatas);
    }
    public void LoadChunkDynamicInstData(ObjectChunk chunk)
    {
        _manager.LoadInstData(chunk.DynamicInstDatas);
    }
}

public class ObjectChunk : BaseChunk
{
    public RootChunk Root { get; private set; }
    public BaseChunk Parent { get => Root; }
    public SubChunk[,] Chunks { get; private set; }
    public int2 CurrentSubChunkIndex { get; private set; }
    public ObjectChunk(RootChunk parent, Vector3 min, Vector3 max) : base(min, max)
    {
        StaticInstDatas = new List<InstanceData>();
        DynamicInstDatas = new List<InstanceData>();
        Root = parent;

        // Make 3x3 SubChunk
        Chunks = new SubChunk[3, 3];
        Vector2 length = new Vector2(Max.x - Min.x, Max.z - Min.z);
        length /= 3;
        for (int i = 0; i < Chunks.GetLength(0); i++)
        {
            for (int j = 0; j < Chunks.GetLength(1); j++)
            {
                Chunks[i, j] = new SubChunk(this,
                    new Vector3(
                        Min.x + i * length.x,
                        Min.y,
                        Min.z + j * length.y),
                    new Vector3(
                        i + 1 >= Chunks.GetUpperBound(0) ? Max.x : (Min.x + (i + 1) * length.x),
                        Max.y,
                        j + 1 >= Chunks.GetUpperBound(1) ? Max.z : (Min.z + (j + 1) * length.y)));
            }
        }
    }

    public void InitSubChunk()
    {
        foreach (var data in StaticInstDatas)
        {
            var index = GetIndex(data.transform.position);
            Chunks[index.x, index.y].StaticInstDatas.Add(data);
        }
        foreach (var data in DynamicInstDatas)
        {
            var index = GetIndex(data.transform.position);
            Chunks[index.x, index.y].DynamicInstDatas.Add(data);
        }
    }

    /// <param name="position">World position</param>
    /// <returns>index of x, y, z</returns>
    public int2 GetIndex(Vector3 position)
    {
        Vector3 relativePos = (position - Min);
        Vector3 length = relativePos / 3;
        // Using Mathf.Clamp to avoid "out of range exception"
        int xIndex = Mathf.Clamp(Mathf.CeilToInt(relativePos.x / length.x), 0, Chunks.GetUpperBound(0));
        int zIndex = Mathf.Clamp(Mathf.CeilToInt(relativePos.z / length.z), 0, Chunks.GetUpperBound(1));
        return new(xIndex, zIndex);
    }
}

public class SubChunk : BaseChunk
{
    public ObjectChunk Parent { get; private set; }
    public RootChunk Root { get => Parent.Root; }
    public SubChunk(ObjectChunk parent, Vector3 min, Vector3 max) : base(min, max)
    {
        Parent = parent;
        StaticInstDatas = new List<InstanceData>();
        DynamicInstDatas = new List<InstanceData>();
    }
}