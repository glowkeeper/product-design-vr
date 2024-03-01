using UnityEngine;
using Unity.Netcode;
using UnityEngine.Video;

public class NetworkVideoController : NetworkBehaviour
{
    [SerializeField] private VideoPlayer mVideoPlayer; 

    [SerializeField] private string m_CameraTag = "MainCamera";

    private bool m_IsServer = false;

    public override void OnNetworkSpawn()
    {
        m_IsServer = IsServer; 
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning("VIDEOCONTROLLER: in trigger enter");
        if (other.tag == m_CameraTag && m_IsServer) {  
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player enter"); 
            var doPlay = true;
            SendVideoPlayToServerRpc(doPlay);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        //Debug.LogWarning("VIDEOCONTROLLER: in trigger exit");
        if (other.tag == m_CameraTag && m_IsServer) {    
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player exit");        
            var doPlay = false;
            SendVideoPlayToServerRpc(doPlay);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendVideoPlayToServerRpc(bool doPlay)
    {        
            // var networkTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
            //Debug.Log("Sending info to clients" + position.ToString() + rotation.ToString() + scale.ToString());
            SendVideoPlayToClientRpc(doPlay);
    }

    [ClientRpc]
    private void SendVideoPlayToClientRpc(bool doPlay)
    {       
            if ( doPlay ) {
                mVideoPlayer.Play();
            } else {
                mVideoPlayer.Stop();
            }
    }
}
