using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{    
    private XRGrabInteractable m_GrabInteractable;
    private NetworkObject m_SpawnedNetworkObject;

    private ulong m_ClientID;

    public override void OnNetworkSpawn()
    {
        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        m_SpawnedNetworkObject = gameObject.GetComponent<NetworkObject>();
        m_ClientID = NetworkManager.Singleton.LocalClientId;
        Debug.Log("Client ID: " + m_ClientID);

        if (m_GrabInteractable != null)
        {
            Debug.Log("Grabbing ");
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);

        } else {
            Debug.Log("NOT Grabbing");
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Trying to do some Grabbing");
        RequestOwnershipServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc()
    {
        Debug.Log("Request ownership for Grabbing" + m_ClientID);
        m_SpawnedNetworkObject.ChangeOwnership(m_ClientID);
    }
}
