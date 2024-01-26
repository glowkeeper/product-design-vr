using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.Vivox;


public class VivoxPlayer : MonoBehaviour
{
    [SerializeField] private string voiceChannel = "1";
    
    //private float _nextUpdate = 0;

    private GameObject xrCam; //position of our Main Camera

    // Start is called before the first frame update
    void Start()
    {        
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        xrCam =  GameObject.FindWithTag("MainCamera");
    }

    void OnUserLoggedIn()
    {
        Debug.Log("Joining Vivox voice channel: " + voiceChannel);
        //_vvm.JoinChannel(voiceChannel, ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.AudioOnly);
        //_vvm.JoinChannel(voiceChannel, ChannelType.Positional, VivoxVoiceManager.ChatCapability.AudioOnly);

        // Channel3DProperties props3D = new Channel3DProperties();
        // await VivoxService.Instance.JoinPositionalChannelAsync(voiceChannel, ChatCapability.AudioOnly, props3D);
        VivoxService.Instance.JoinGroupChannelAsync(voiceChannel, ChatCapability.AudioOnly);

        // var cid = new Channel(voiceChannel, ChannelType.Positional);
        // _chan = _vvm.LoginSession.GetChannelSession(cid);
    }

    void OnUserLoggedOut()
    {
        Debug.Log("Disconnecting from voice channel " + voiceChannel);
        VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Disconnecting from Vivox");
        VivoxService.Instance.LogoutAsync();  
    }

    // Update is called once per frame
    // void Update()
    // {      
    //     if (Time.time > _nextUpdate)
    //     {
    //         // _chan.Set3DPosition(xrCam.position, xrCam.position, xrCam.forward, xrCam.up);
    //         try {

    //             VivoxService.Instance.Set3DPosition(xrCam, voiceChannel);
    //             Debug.Log("Set position");

    //         } catch (Exception e) {
    //             Debug.Log(e);
    //         }
            
    //         _nextUpdate += 0.5f;
            
    //     }     
    // }
}