using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkSlideShow : NetworkBehaviour
{
    [SerializeField] private GameObject projectorScreen;
    [SerializeField] private string[] slides;

    [SerializeField] private string slideDeckName;

    private const string slidesPath = "SlideDecks";
    private int slideIndex = 0;
    private Material screenMaterial;

    void Start()
    {        
        Renderer screenRenderer = projectorScreen.GetComponent<Renderer>();
        screenMaterial = screenRenderer.material;
    }
    
    public void Forward()
    {
        if(slideIndex < slides.Length)
        { 
            slideIndex++;
            Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);            
            Debug.Log("getting back slide" + slides[slideIndex]); 
            screenMaterial.SetTexture("_BaseMap", slide);
            SendSlideToServerRpc(slideIndex);
        }
    }

    public void Back()
    {
        if(slideIndex > 0)
        {
            slideIndex--;
            Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);  
            Debug.Log("getting back slide" + slides[slideIndex]);  
            screenMaterial.SetTexture("_BaseMap", slide);
            SendSlideToServerRpc(slideIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendSlideToServerRpc(int slideIndex)
    {        
            SendSlideToClientRpc(slideIndex);
    }

    [ClientRpc]
    private void SendSlideToClientRpc(int slideIndex)
    {       
        Texture2D slide = Resources.Load<Texture2D>(slidesPath + "/" + slideDeckName + "/" + slides[slideIndex]);  
        screenMaterial.SetTexture("_BaseMap", slide);
    }
        
}
