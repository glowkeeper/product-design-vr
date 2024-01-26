using System.Threading.Tasks;

using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine.Android;
using Unity.Services.Authentication;

public class VivoxVoiceManager : MonoBehaviour
{  
    static object m_Lock = new object();
    static VivoxVoiceManager m_Instance;

    public static VivoxVoiceManager Instance
    {
        get
        {
            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (VivoxVoiceManager)FindObjectOfType(typeof(VivoxVoiceManager));

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<VivoxVoiceManager>();
                        singletonObject.name = typeof(VivoxVoiceManager).ToString() + " (Singleton)";
                    }
                }
                // Make instance persistent even if its already in the scene
                DontDestroyOnLoad(m_Instance.gameObject);
                return m_Instance;
            }
        }
    }

    async void Awake()
    {
        await UnityServices.InitializeAsync(); 
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await VivoxService.Instance.InitializeAsync();
        Debug.Log("Signed in with id: " + AuthenticationService.Instance.PlayerId);
    }
    
    public async void SignInToVivox()
    {
        LoginOptions options = new LoginOptions();
        options.DisplayName = AuthenticationService.Instance.PlayerId;
        options.EnableTTS = true;    
        await VivoxService.Instance.LoginAsync(options);

        Debug.Log("Signed into Vivox with Display Name: " + options.DisplayName);
    }    
}