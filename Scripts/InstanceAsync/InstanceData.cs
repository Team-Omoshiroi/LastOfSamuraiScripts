using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class InstanceData : MonoBehaviour
{
    public AssetReference AssetRef;
    public GameObject DeployedObj { get; set; }
    protected eDeployState _state = eDeployState.Undeploy;
    public eDeployState State
    {
        get => _state;
        set
        {
            //Debug.Log($"{Name} : {_state} => {value} at {DateTime.UtcNow} {DateTime.UtcNow.Millisecond}ms");
            _state = value;
        }
    }

    public virtual void Initialize()
    {
        
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        var mesh = (AssetRef.editorAsset as GameObject).GetComponents<MeshFilter>();
        foreach (var m in mesh)
        {
            Gizmos.DrawWireMesh(
                m.sharedMesh,
                transform.position + m.transform.localPosition,
                transform.rotation,
                new Vector3(transform.localScale.x * m.transform.localScale.x,
                transform.localScale.y * m.transform.localScale.y,
                transform.localScale.z * m.transform.localScale.z));
        }

        mesh = (AssetRef.editorAsset as GameObject).GetComponentsInChildren<MeshFilter>();
        Gizmos.color = Color.blue;
        foreach (var m in mesh)
        {
            Gizmos.DrawWireMesh(
                m.sharedMesh,
                transform.position + transform.rotation * m.transform.localPosition,
                Quaternion.Euler(transform.rotation.eulerAngles + m.transform.rotation.eulerAngles),
                new Vector3(transform.localScale.x * m.transform.localScale.x,
                transform.localScale.y * m.transform.localScale.y,
                transform.localScale.z * m.transform.localScale.z));
        }
    }
#endif
}
