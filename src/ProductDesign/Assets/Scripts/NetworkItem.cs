using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{       
    private XRGrabInteractable m_GrabInteractable;

    private NetworkObject m_NetObject;

    public override void OnNetworkSpawn()
    {
        m_NetObject = gameObject.GetComponent<NetworkObject>();    
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
        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientID)
    {
        m_NetObject.ChangeOwnership(clientID); 
    }
}
