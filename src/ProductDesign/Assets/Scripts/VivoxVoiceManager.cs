using System.Threading.Tasks;

using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine.Android;
using Unity.Services.Authentication;

public class VivoxVoiceManager : MonoBehaviour
{  
    [SerializeField] private string voiceChannel = "1";

    static object m_Lock = new object();
    static VivoxVoiceManager m_Instance;

    private GameObject xrCam; //position of our Main Camera

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

    void Start()
    {        
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        xrCam =  GameObject.FindWithTag("MainCamera");
    }
    
    public async void SignInToVivox()
    {
        LoginOptions options = new LoginOptions();
        options.DisplayName = AuthenticationService.Instance.PlayerId;
        options.EnableTTS = true;    
        await VivoxService.Instance.LoginAsync(options);
        await VivoxService.Instance.JoinGroupChannelAsync(voiceChannel, ChatCapability.AudioOnly);
        await VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, voiceChannel);

        Debug.Log("Signed into Vivox with Display Name: " + options.DisplayName);
    }   

    void OnUserLoggedIn()
    {
        Debug.Log("Joining Vivox voice channel: " + voiceChannel);
        //_vvm.JoinChannel(voiceChannel, ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.AudioOnly);
        //_vvm.JoinChannel(voiceChannel, ChannelType.Positional, VivoxVoiceManager.ChatCapability.AudioOnly);

        // Channel3DProperties props3D = new Channel3DProperties();
        // await VivoxService.Instance.JoinPositionalChannelAsync(voiceChannel, ChatCapability.AudioOnly, props3D);
        // VivoxService.Instance.JoinGroupChannelAsync(voiceChannel, ChatCapability.AudioOnly);
        // VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, voiceChannel);

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
}