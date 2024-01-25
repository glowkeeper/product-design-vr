using System.Threading.Tasks;

using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine.Android;
#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif

public class VivoxVoiceManager : MonoBehaviour
{  
    private int PermissionAskedCount;

    // Check to see if we're about to be destroyed.
    static object m_Lock = new object();
    static VivoxVoiceManager m_Instance;

    //These variables should be set to the projects Vivox credentials if the authentication package is not being used
    //Credentials are available on the Vivox Developer Portal (developer.vivox.com) or the Unity Dashboard (dashboard.unity3d.com), depending on where the organization and project were made
    [SerializeField]
    string _key;
    [SerializeField]
    string _issuer;
    [SerializeField]
    string _domain;
    [SerializeField]
    string _server;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
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
        if (m_Instance != this && m_Instance != null)
        {
            Debug.LogWarning("Multiple VivoxVoiceManager detected in the scene. Only one VivoxVoiceManager can exist at a time. The duplicate VivoxVoiceManager will be destroyed.");
            Destroy(this);
            return;
        }

        var options = new InitializationOptions();
        if (CheckManualCredentials())
        {
            options.SetVivoxCredentials(_server, _domain, _issuer, _key);
        }

        await UnityServices.InitializeAsync(options);
#if AUTH_PACKAGE_PRESENT
        if (!CheckManualCredentials())
        {
            AuthenticationService.Instance.ClearSessionToken();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
#endif

        await VivoxService.Instance.InitializeAsync();

        SignInToVivox();
    }

    bool CheckManualCredentials()
    {
        return !(string.IsNullOrEmpty(_issuer) && string.IsNullOrEmpty(_domain) && string.IsNullOrEmpty(_server));
    }

    
    async void SignInToVivox()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }
        return "";
    }
#endif

        bool IsMicPermissionGranted()
        {
            bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (IsAndroid12AndUp())
        {
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
            isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
        }
#endif
            return isGranted;
        }

        void AskForPermissions()
        {
            string permissionCode = Permission.Microphone;

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (PermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
            PermissionAskedCount++;
            Permission.RequestUserPermission(permissionCode);
        }

        bool IsPermissionsDenied()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return PermissionAskedCount == 2;
        }
#endif
            return PermissionAskedCount == 1;
        }

        LoginOptions options = new LoginOptions();
        options.DisplayName = "Student";

        //Actual code runs from here
        if (IsMicPermissionGranted())
        {            
            await VivoxService.Instance.LoginAsync(options);
        }
        else
        {
            if (IsPermissionsDenied())
            {
                PermissionAskedCount = 0;
                await VivoxService.Instance.LoginAsync(options);
            }
            else
            {
                AskForPermissions();
                await VivoxService.Instance.LoginAsync(options);
            }
        }
    }

    
}