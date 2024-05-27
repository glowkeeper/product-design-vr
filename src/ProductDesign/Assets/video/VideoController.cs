using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer mVideoPlayer; 

    [SerializeField] private string m_CameraTag = "MainCamera";

    private void OnTriggerEnter(Collider other) {

        //Debug.LogWarning("VIDEOCONTROLLER: in trigger enter");
        if (other.tag == m_CameraTag) {  
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player enter"); 
            mVideoPlayer.Play();
        }
    }

    private void OnTriggerExit(Collider other) {

        //Debug.LogWarning("VIDEOCONTROLLER: in trigger exit");
        if (other.tag == m_CameraTag) {    
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player exit");        
            mVideoPlayer.Stop();
        }
    }
}
