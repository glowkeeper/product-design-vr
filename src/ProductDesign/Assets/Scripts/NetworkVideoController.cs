using UnityEngine;
using Unity.Netcode;
using UnityEngine.Video;

public class NetworkVideoController : NetworkBehaviour
{
    [SerializeField] private VideoPlayer mVideoPlayer; 

    [SerializeField] private string m_CameraTag = "MainCamera";

    private bool m_Play = false;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning("VIDEOCONTROLLER: in trigger enter");
        if (other.tag == m_CameraTag) {  
            mVideoPlayer.Play();
            SendVideoPlayToServerRpc(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.LogWarning("VIDEOCONTROLLER: in trigger exit");
        if (other.tag == m_CameraTag) {               
            mVideoPlayer.Stop();
            SendVideoPlayToServerRpc(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendVideoPlayToServerRpc(bool doPlay)
    {        
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
