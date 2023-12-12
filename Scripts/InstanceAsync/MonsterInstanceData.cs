using Unity.VisualScripting;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MonsterInstanceData : InstanceData
{
    BaseAI _baseAI;
    EnemyStatusController _status;

    [SerializeField] Transform[] _waypoints;
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _fovRange = 6f;
    [SerializeField] float _attackRange = 2f;
    [SerializeField] float _attackPerSecond = 1f;
    [SerializeField] float _recognitionTime = 0.5f;
    [SerializeField] float _coolDownTime_SA = 0.3f;
    [SerializeField] float _frequency_SA = 0.3f;
    // Deploy
    public override void Initialize()
    {
        base.Initialize();

        // TODO
        // 필요한 초기화 작업 수행
        // DeployedObj에 대해 접근해서 캐싱을 하는 것도 좋음.
        _baseAI = DeployedObj.GetComponent<BaseAI>();
        _baseAI.Initialize(_waypoints, _moveSpeed, _fovRange, _attackRange, _attackPerSecond, _recognitionTime, _coolDownTime_SA, _frequency_SA);

        _status = DeployedObj.GetComponent<EnemyStatusController>();
        _status.OnDead += SetState;
        _status.OnDead += () => { _status.OnDead -= SetState; };
    }

    private void SetState()
    {
        State = eDeployState.Dead;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        var mesh = AssetRef.editorAsset.GetComponentInChildren<SkinnedMeshRenderer>();
        Gizmos.DrawWireMesh(
            mesh.sharedMesh,
            transform.position + transform.rotation * mesh.transform.localPosition,
            Quaternion.Euler(transform.rotation.eulerAngles + mesh.transform.rotation.eulerAngles),
            new Vector3(transform.localScale.x * mesh.transform.localScale.x,
            transform.localScale.y * mesh.transform.localScale.y,
            transform.localScale.z * mesh.transform.localScale.z));
    }

    [CustomEditor(typeof(MonsterInstanceData))]
    public class MonsterInstanceDataEditor : Editor
    {
        MonsterInstanceData _instance;

        private void OnEnable()
        {
            if (AssetDatabase.Contains(target))
            {
                _instance = null;
            }
            else
            {
                _instance = target as MonsterInstanceData;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fix Height"))
            {
                FixHeight(_instance._waypoints);
            }
        }

        public void FixHeight(Transform[] waypoints)
        {
            var mainTerrain = Terrain.activeTerrain;
            foreach (var item in waypoints)
            {
                //float height = mainTerrain.terrainData.GetHeight(Mathf.FloorToInt(item.position.x), Mathf.FloorToInt(item.position.z));
                float height = mainTerrain.SampleHeight(item.position);
                item.position = new Vector3(item.position.x, height, item.position.z);
            }
            float actorHeight = mainTerrain.SampleHeight(_instance.transform.position);
            _instance.transform.position = new Vector3(_instance.transform.position.x, actorHeight, _instance.transform.position.z);
        }
    }
#endif
}