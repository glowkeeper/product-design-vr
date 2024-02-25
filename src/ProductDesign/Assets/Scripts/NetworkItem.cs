using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{    
    private XRGrabInteractable m_GrabInteractable;
    // private NetworkObject m_SpawnedNetworkObject;

    private bool m_HasGrabbed = false;

    private ulong m_ClientID;

    public override void OnNetworkSpawn()
    {
        //m_SpawnedNetworkObject = gameObject.GetComponent<NetworkObject>();
        m_ClientID = NetworkManager.Singleton.LocalClientId;
        Debug.Log("Client ID: " + m_ClientID);

        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();

        if (m_GrabInteractable != null)
        {
            Debug.Log("Able to Grab");
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);            
            m_GrabInteractable.selectExited.AddListener(OnLetGo);

        } else {
            Debug.Log("Cannot Grab");
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Trying to do some Grabbing");
        m_HasGrabbed = true;
    }

    private void OnLetGo(SelectExitEventArgs args)
    {
        Debug.Log("Trying to Let Go");        
        m_HasGrabbed = false;
    }       

    // [ServerRpc(RequireOwnership = false)]
    // private void RequestOwnershipServerRpc()
    // {
    //     Debug.Log("Request ownership for Grabbing" + m_ClientID);
    //     m_SpawnedNetworkObject.ChangeOwnership(m_ClientID);
    // }

    private void FixedUpdate() 
   {
        if (m_HasGrabbed) 
        {
            Debug.Log("Calling Send RPC" + transform.position.ToString() + transform.rotation.ToString() + transform.localScale.ToString());
            SendInfoToServerRpc(transform.position, transform.rotation, transform.localScale);
        }
   }

   [ServerRpc(RequireOwnership = false)]
   private void SendInfoToServerRpc(Vector3 position, Quaternion rotation, Vector3 scale)
   {        
        var networkTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        Debug.Log("Sending info to clients" + position.ToString() + rotation.ToString() + scale.ToString() + networkTime.ToString());
        SendInfoToClientRpc(position, rotation, scale, networkTime);
   }

   [ClientRpc]
   private void SendInfoToClientRpc(Vector3 position, Quaternion rotation, Vector3 scale, float networkTime)
   {
        Debug.Log("Receiving info from server" + position.ToString() + rotation.ToString() + scale.ToString() + networkTime.ToString());
        if( !m_HasGrabbed ) {            
            DoMove(position, rotation, scale, networkTime);
        }
   }

   private void DoMove(Vector3 position, Quaternion rotation, Vector3 scale, float networkTime) 
   {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
   }
}
