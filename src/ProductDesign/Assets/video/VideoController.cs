using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer mVideoPlayer;    
    // void Start() {
        //Debug.LogWarning("VIDEOCONTROLLER: in start");
    //}

    private void OnTriggerEnter(Collider other) {

        //Debug.LogWarning("VIDEOCONTROLLER: in trigger enter");
        if (other.tag == "MainCamera") {  
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player enter"); 
            mVideoPlayer.Play();
        }
    }

    private void OnTriggerExit(Collider other) {

        //Debug.LogWarning("VIDEOCONTROLLER: in trigger exit");
        if (other.tag == "MainCamera") {    
            //Debug.LogWarning("VIDEOCONTROLLER: in trigger player exit");        
            mVideoPlayer.Stop();
        }
    }
}
