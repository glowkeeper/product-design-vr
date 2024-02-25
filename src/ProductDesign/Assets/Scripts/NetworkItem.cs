using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{    
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private bool DestroyWithSpawner = true;
    
    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;
    private XRGrabInteractable m_GrabInteractable;
    // private NetworkObject m_SpawnedNetworkObject;

    private bool m_HasGrabbed = false;
    private bool m_HasLetGo = false;
    private bool m_DidGrab = false;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private Vector3 m_Scale;

    void Start() {

        m_Position = transform.position;
        m_Rotation = transform.rotation;
        m_Scale = transform.localScale;

        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();

        if (m_GrabInteractable != null)
        {
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);            
            m_GrabInteractable.selectExited.AddListener(OnLetGo);

        } else {
            Debug.Log("Cannot Grab");
        }
    }

    public override void OnNetworkSpawn()
    {
        // Only the server spawns
        if (IsServer && prefabToSpawn != null)
        {
            var clientID = NetworkManager.Singleton.LocalClientId;
            m_PrefabInstance = Instantiate(prefabToSpawn);

            // Optional, this example applies the spawner's position and rotation to the new instance
            m_PrefabInstance.transform.position = transform.position;
            m_PrefabInstance.transform.rotation = transform.rotation;

            // Get the instance's NetworkObject and Spawn
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.SpawnWithOwnership(clientID);
        }      
    }

    public override void OnNetworkDespawn()
        {
            if (IsServer && DestroyWithSpawner && m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
            {
                m_SpawnedNetworkObject.Despawn();
            }
            base.OnNetworkDespawn();
        }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        //Debug.Log("Trying to do some Grabbing");
        m_HasGrabbed = true;
    }

    private void OnLetGo(SelectExitEventArgs args)
    {
        //Debug.Log("Trying to Let Go");        
        m_HasLetGo = true;
    }  

    private void FixedUpdate() 
   {
        if (m_HasGrabbed) 
        { 
            SendInfoToServerRpc(transform.position, transform.rotation, transform.localScale);            

        } else {

            if ( m_HasLetGo ) {
                
                if( transform.hasChanged ) {

                    SendInfoToServerRpc(transform.position, transform.rotation, transform.localScale);

                } else {
                    
                    m_HasLetGo = false;
                    
                }
            } else {                
                
                transform.position = m_Position;
                transform.rotation = m_Rotation;
                transform.localScale = m_Scale;

            }
        }
   }

   [ServerRpc(RequireOwnership = false)]
   private void SendInfoToServerRpc(Vector3 position, Quaternion rotation, Vector3 scale)
   {        
        // var networkTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        // Debug.Log("Sending info to clients" + position.ToString() + rotation.ToString() + scale.ToString());
        SendInfoToClientRpc(position, rotation, scale);
   }

   [ClientRpc]
   private void SendInfoToClientRpc(Vector3 position, Quaternion rotation, Vector3 scale)
   {
        // Debug.Log("Receiving info from server" + position.ToString() + rotation.ToString() + scale.ToString());
        if( !m_HasGrabbed ) {   

            m_Position = position; 
            m_Rotation = rotation;
            m_Scale = scale;
        }
   }
}
