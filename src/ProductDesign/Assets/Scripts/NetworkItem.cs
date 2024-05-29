using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{       
    private XRGrabInteractable m_GrabInteractable;

    private Rigidbody m_RigidBody;
    private NetworkObject m_NetObject;

    private ulong m_ClientID;

    public override void OnNetworkSpawn()
    {
        m_ClientID = NetworkManager.Singleton.LocalClientId;  
        m_NetObject = gameObject.GetComponent<NetworkObject>();        
        m_RigidBody = gameObject.GetComponent<Rigidbody>();        
        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();

        if (m_GrabInteractable != null)
        {
            //Debug.Log("Grabbing");
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);            
            //m_GrabInteractable.selectExited.AddListener(OnLetGo);

        } else {
            Debug.Log("Cannot Grab");
        }   
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        //SendMoveToServerRpc(transform.position, transform.rotation, transform.localScale, m_RigidBody.velocity, m_ClientID); 
        RequestOwnershipServerRpc(m_ClientID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientID)
    {
        m_NetObject.ChangeOwnership(clientID); 
    }
}
