using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Messaging;
using UnityEngine;
using Unity.Notifications.iOS;
public class PushNotification : MonoBehaviour
{
    private bool firebaseInitialized = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private async void Start()
    {
        await InitializeFirebase();

        if (firebaseInitialized)
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

    }

    // Initialize Firebase dependencies
    private async Task InitializeFirebase()
    {
        Debug.Log("Checking Firebase dependencies...");
        var dependencyStatus = await FirebaseApp.CheckDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            firebaseInitialized = true;
            Debug.Log("Firebase is ready.");
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            firebaseInitialized = false;
        }
    }

    private IEnumerator RequestNotifications()
    {

        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            Debug.Log("Requesting Notification for IOS");
            yield return null;
        }

    }

    // Called when Firebase receives a new token
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    // Called when Firebase receives a new message
    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

    // Unsubscribe from Firebase events when object is destroyed
    private void OnDestroy()
    {
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }
}
