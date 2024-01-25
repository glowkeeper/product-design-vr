using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;

public class VivoxPlayer : MonoBehaviour
{
    [SerializeField] private string voiceChannelName = "Product Design";
    
    private float _nextUpdate = 0;

    private GameObject xrCam; //position of our Main Camera

    // Start is called before the first frame update
    void Start()
    {
        //_vvm = VivoxVoiceManager.Instance;
        // _vvm.OnUserLoggedInEvent += OnUserLoggedIn;
        // _vvm.OnUserLoggedOutEvent += OnUserLoggedOut;

        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;

        xrCam =  GameObject.FindWithTag("MainCamera");
    }

    async void OnUserLoggedIn()
    {
        
        Debug.Log("Successfully connected to Vivox");
        Debug.Log("Joining voice channel: " + voiceChannelName);
        //_vvm.JoinChannel(voiceChannelName, ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.AudioOnly);
        //_vvm.JoinChannel(voiceChannelName, ChannelType.Positional, VivoxVoiceManager.ChatCapability.AudioOnly);

        Channel3DProperties props3D = new Channel3DProperties();
        await VivoxService.Instance.JoinPositionalChannelAsync(voiceChannelName, ChatCapability.AudioOnly, props3D);

        // var cid = new Channel(voiceChannelName, ChannelType.Positional);
        // _chan = _vvm.LoginSession.GetChannelSession(cid);
    }

    async void OnUserLoggedOut()
    {
        Debug.Log("Disconnecting from voice channel " + voiceChannelName);
        await VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Disconnecting from Vivox");
        await VivoxService.Instance.LogoutAsync();  
    }

    // Update is called once per frame
    void Update()
    {      
        if (Time.time > _nextUpdate)
        {
            //_chan.Set3DPosition(xrCam.position, xrCam.position, xrCam.forward, xrCam.up);
            VivoxService.Instance.Set3DPosition(xrCam, voiceChannelName);
            _nextUpdate += 0.5f;
        }     
    }
}