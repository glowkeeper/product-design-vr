using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkItem : NetworkBehaviour
{        
    private XRGrabInteractable m_GrabInteractable;

    private bool m_HasGrabbed = false;
    private bool m_HasLetGo = false;
    private bool m_HasMoved = false;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private Vector3 m_Scale;

    public override void OnNetworkSpawn()
    {
        m_Position = transform.position;
        m_Rotation = transform.rotation;
        m_Scale = transform.localScale;

        m_GrabInteractable = gameObject.GetComponent<XRGrabInteractable>();

        if (m_GrabInteractable != null)
        {
            //Debug.Log("Grabbing");
            m_GrabInteractable.selectEntered.AddListener(OnGrabbed);            
            m_GrabInteractable.selectExited.AddListener(OnLetGo);

        } else {
            //Debug.Log("Cannot Grab");
        }   
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

                if ( m_HasMoved ) {
                                   
                    transform.position = m_Position;
                    transform.rotation = m_Rotation;
                    transform.localScale = m_Scale;
                }
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
        //Debug.Log("Receiving info from server" + position.ToString() + rotation.ToString() + scale.ToString());
        if( !m_HasGrabbed ) {   
            //Debug.Log("here?");
            m_Position = position; 
            m_Rotation = rotation;
            m_Scale = scale;
            m_HasMoved = true;
        }
   }
}
