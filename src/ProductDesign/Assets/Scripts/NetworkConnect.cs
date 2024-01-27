
//using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Networking.Transport.Relay;

using Unity.Services.Core;
using Unity.Services.Authentication;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Services.Vivox;

public class NetworkConnect : MonoBehaviour
{
    [SerializeField] private int maxConnections = 4;
    [SerializeField] private TMP_InputField joinCodeInput;
    
    //private float _nextUpdate = 0;

    private string connectionType = "dtls";

    public async void Create()
    {
        try {

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + joinCode);
            joinCodeInput.text = joinCode;

            RelayServerData serverData = new RelayServerData(allocation, connectionType);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();
            VivoxVoiceManager.Instance.SignInToVivox();

        } catch (RelayServiceException e) {
            Debug.Log(e);
        }       
    }

    // Update is called once per frame
    public async void Join()
    {
        string joinCode = joinCodeInput.text;
        if (!string.IsNullOrEmpty(joinCode)) {
            
            try {                

                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData serverData = new RelayServerData(joinAllocation, connectionType);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

                NetworkManager.Singleton.StartClient();                  
                VivoxVoiceManager.Instance.SignInToVivox();              

            } catch (RelayServiceException e) {
                Debug.Log(e);
            }  
        }      
    }
}
