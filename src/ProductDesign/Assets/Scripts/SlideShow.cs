using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SlideShow : NetworkBehaviour
{
    [SerializeField] private GameObject projectorScreen;
    [SerializeField] private string[] slides;

    [SerializeField] private string slideDeckName;

    private const string slidesPath = "SlideDecks";

    private int slideIndex = 0;
    private Material screenMaterial;

    private bool m_IsServer = false;

    public override void OnNetworkSpawn()
    {
        m_IsServer = IsServer; 
        Renderer screenRenderer = projectorScreen.GetComponent<Renderer>();
        screenMaterial = screenRenderer.material;
    }

    // void Start()
    // {        
    //     Renderer screenRenderer = projectorScreen.GetComponent<Renderer>();
    //     screenMaterial = screenRenderer.material;
    // }
    
    public void Forward()
    {
        if(slideIndex < slides.Length && m_IsServer)
        { 
            slideIndex++;
            Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);            
            //Debug.Log("getting back slide" + slides[slideIndex]); 
            screenMaterial.SetTexture("_BaseMap", slide);
            SendPresentationShowToServerRpc(slideIndex);
        }
    }

    public void Back()
    {
        if(slideIndex > 0 && m_IsServer)
        {
            slideIndex--;
            Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);  
            //Debug.Log("getting back slide" + slides[slideIndex]);  
            screenMaterial.SetTexture("_BaseMap", slide);
            SendPresentationShowToServerRpc(slideIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPresentationShowToServerRpc(int slideIndex)
    {        
            SendPresentationShowToClientRpc(slideIndex);
    }

    [ClientRpc]
    private void SendPresentationShowToClientRpc(int slideIndex)
    {       
        Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);  
        screenMaterial.SetTexture("_BaseMap", slide);
    }
        
}
