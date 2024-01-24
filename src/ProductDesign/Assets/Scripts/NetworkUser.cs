using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUser : NetworkBehaviour
{

    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;

    [SerializeField] private Renderer[] meshToDisable;

    public override void OnNetworkSpawn() {
        
        base.OnNetworkSpawn();
        if (IsOwner) {
            foreach (var mesh in meshToDisable)
            {
                mesh.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (IsOwner) {
            root.position = VRRigReferences.Instance.Root.position;
            root.rotation = VRRigReferences.Instance.Root.rotation;
            
            head.position = VRRigReferences.Instance.Head.position;
            head.rotation = VRRigReferences.Instance.Head.rotation;

            left.position = VRRigReferences.Instance.Left.position;
            left.rotation = VRRigReferences.Instance.Left.rotation;

            right.position = VRRigReferences.Instance.Right.position;
            right.rotation = VRRigReferences.Instance.Right.rotation;
        }
    }
}
