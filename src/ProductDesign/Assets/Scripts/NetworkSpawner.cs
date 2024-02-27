using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;

public class NetworkSpawner : NetworkBehaviour
    {
        
        [SerializeField] private GameObject prefabToSpawn;
        [SerializeField] private bool DestroyWithSpawner = true;
        //private XRGrabInteractable m_GrabInteractable;
        private GameObject m_PrefabInstance;
        private NetworkObject m_SpawnedNetworkObject;

        public override void OnNetworkSpawn()
        {
            //Debug.Log("NetworkSpawn");
            // Only the server spawns, clients will disable this component on their side
            enabled = IsServer;

            if (!enabled || prefabToSpawn == null)
            {
                return;
            }

            //Debug.Log("Deep into NetworkSpawn");

            var clientID = NetworkManager.Singleton.LocalClientId;
            m_PrefabInstance = Instantiate(prefabToSpawn);

            // Optional, this example applies the spawner's position and rotation to the new instance
            m_PrefabInstance.transform.position = transform.position;
            m_PrefabInstance.transform.rotation = transform.rotation;

            // Get the instance's NetworkObject and Spawn
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.SpawnWithOwnership(clientID);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && DestroyWithSpawner && m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
            {
                m_SpawnedNetworkObject.Despawn();
            }
            base.OnNetworkDespawn();
        }
    }
