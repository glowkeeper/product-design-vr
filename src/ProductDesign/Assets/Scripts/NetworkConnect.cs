
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

public class NetworkConnect : MonoBehaviour
{
    [SerializeField] private int maxConnections = 4;
    [SerializeField] private TMP_InputField joinCodeInput;
    private string connectionType = "dtls";

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in with id: " + AuthenticationService.Instance.PlayerId);
    }

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

            } catch (RelayServiceException e) {
                Debug.Log(e);
            }  
        }      
    }
}
