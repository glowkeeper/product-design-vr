using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{       
    private bool m_HasGrabbed = false;
    private bool m_HasLetGo = false;
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
        //Debug.Log("Trying to do some Grabbing");
        m_HasGrabbed = true;
        m_HasLetGo = false;
    }

    private void OnLetGo(SelectExitEventArgs args)
    {
        //Debug.Log("Trying to Let Go");    
        m_HasGrabbed = false;    
        m_HasLetGo = true;
    }  

    private void FixedUpdate() 
    {
        if (m_HasGrabbed) 
        { 
            //Debug.Log("Sending info from Grabbed");
            SendInfoToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID);            

        } else if ( m_HasLetGo ) {
                
            if( transform.hasChanged ) 
            {
                //Debug.Log("Sending info from let go");
                SendInfoToServerRpc(transform.position, transform.rotation, transform.localScale, m_ClientID);
                transform.hasChanged = false;

            } else {
                
                //Debug.Log("Do I ever get here?");
                m_HasLetGo = false;
                
            }

        } else if ( m_MoveClientID != m_ClientID && 
                    (System.DateTime.Now.TimeOfDay.TotalMilliseconds - m_TimeMove) < m_IdleTime ) 
        {
            // Debug.Log("In here at time " + now);
            // Debug.Log("In here at position " + m_Position.ToString());
            m_RigidBody.Move(m_Position, m_Rotation);
            // transform.localScale = m_Scale;
        }
   }

   [ServerRpc(RequireOwnership = false)]
   private void SendInfoToServerRpc(Vector3 position, Quaternion rotation, Vector3 scale, ulong clientID)
   {        
        // var networkTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        //Debug.Log("Sending info to clients" + position.ToString() + rotation.ToString() + scale.ToString());
        SendInfoToClientRpc(position, rotation, scale, clientID);
   }

   [ClientRpc]
   private void SendInfoToClientRpc(Vector3 position, Quaternion rotation, Vector3 scale, ulong clientID)
   {                  
        m_Position = position;
        m_Rotation = rotation;
        m_Scale = scale;
        m_MoveClientID = clientID;
        m_TimeMove = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
        //Debug.Log("Me: " + m_ClientID + ", Mover: " + m_MoveClientID);
   }
}
