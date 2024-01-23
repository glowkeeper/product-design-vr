using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkConnect : MonoBehaviour
{
    // Start is called before the first frame update
    public void Create()
    {
        //Debug.Log("Network Connect create");
        NetworkManager.Singleton.StartHost();
    }

    // Update is called once per frame
    public void Join()
    {
        //Debug.Log("Network Connect join");
        NetworkManager.Singleton.StartClient();
    }
}
