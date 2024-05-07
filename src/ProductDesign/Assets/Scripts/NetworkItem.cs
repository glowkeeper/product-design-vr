using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{       
    private XRGrabInteractable m_GrabInteractable;

    private Rigidbody m_RigidBody;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private Vector3 m_Scale;

    private ulong m_ClientID;
    private ulong m_MoveClientID;

    private const double m_IdleTime = 1000; //milliseconds
    private double m_TimeMove = 0;

    public override void OnNetworkSpawn()
    {
        m_ClientID = NetworkManager.Singleton.LocalClientId;
        m_MoveClientID = m_ClientID;
        
        m_RigidBody = gameObject.GetComponent<Rigidbody>();
        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        if (m_GrabInteractable != null)
        {
            //Debug.Log("Grabbing");
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);            
            m_GrabInteractable.selectExited.AddListener(OnLetGo);

        } else {
            Debug.Log("Cannot Grab");
        }   
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        //Debug.Log("Sending info from OnGrabbed");
        SendMoveToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID);  
        transform.hasChanged = true; 
    }

    private void OnLetGo(SelectExitEventArgs args)
    {
        //Debug.Log("Sending info from OnLetGo");
        SendMoveToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID); 
        transform.hasChanged = true; 
    }  

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Sending info from OnCollisionEnter");
        SendMoveToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID);
        transform.hasChanged = true; 
    }

    private void DoMove() 
    {
        if ( m_MoveClientID != m_ClientID && 
                    (System.DateTime.Now.TimeOfDay.TotalMilliseconds - m_TimeMove) < m_IdleTime ) 
        { 
            m_RigidBody.Move(m_Position, m_Rotation);
            // transform.localScale = m_Scale;
        }
    }

    private void GenerateMove()
    {        
        if( m_MoveClientID == m_ClientID && transform.hasChanged ) 
        {
            //Debug.Log("Sending info from let go");
            SendMoveToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID);
            transform.hasChanged = false; 
        }        
    }

    private void FixedUpdate() 
    {
        GenerateMove();
    }

   [ServerRpc(RequireOwnership = false)]
   private void SendMoveToServerRpc(Vector3 position, Quaternion rotation, Vector3 scale, ulong clientID)
   {        
        // var networkTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        //Debug.Log("Sending info to clients" + position.ToString() + rotation.ToString() + scale.ToString());
        SendMoveToClientRpc(position, rotation, scale, clientID);
   }

   [ClientRpc]
   private void SendMoveToClientRpc(Vector3 position, Quaternion rotation, Vector3 scale, ulong clientID)
   {                  
        m_Position = position;
        m_Rotation = rotation;
        m_Scale = scale;
        m_MoveClientID = clientID;
        m_TimeMove = System.DateTime.Now.TimeOfDay.TotalMilliseconds;        
        DoMove();
        //Debug.Log("Me: " + m_ClientID + ", Mover: " + m_MoveClientID);
   }
}
