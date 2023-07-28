using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Discovery;
using TMPro;
public class StartMenu : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public GameObject networkObject;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private TMP_Text loadingText;

    private void Start() {
        loadingText = GameObject.Find("Searching For Game").GetComponent<TMP_Text>();
        loadingText.gameObject.SetActive(false);
        networkManager = networkObject.GetComponent<NetworkManager>();
        networkDiscovery = networkObject.GetComponent<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        networkDiscovery.StartDiscovery();
        //NetworkManager.singleton.StartClient();

        //UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
    }
    private void Update() {
        //Debug.Log("Servers Found = " + discoveredServers.Count);
    }
    public void StartGame()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();

        StartCoroutine(WaitForServerAndStart());
        //networkManager.networkAddress = "localhost"; // set the IP address of the server
        Debug.Log("<color=purple>Discovered Servers: " + (discoveredServers.Count) + "</color>");
    }
    private IEnumerator WaitForServerAndStart()
    {
        float startTime = Time.time;
        string loadingBar = ".";
        while (Time.time - startTime < 5f)
        {
            loadingText.gameObject.SetActive(true);
            if(loadingBar.Length>3){
                loadingBar = ".";
            }
            else{
                loadingBar+=loadingBar;
            }
            loadingBar+=loadingBar;
            loadingText.text = "Looking For Game\n"+(loadingBar);
            if (discoveredServers.Count >= 1)
            {
                // Found a server, stop discovery and connect to it
                networkDiscovery.StopDiscovery();
                loadingBar="Starting Game";
                foreach (long serverKey in discoveredServers.Keys)
                {
                    Debug.Log(serverKey);
                    Debug.Log(discoveredServers[serverKey].uri);
                    networkManager.StartClient(discoveredServers[serverKey].uri);
                    break;
                }
                yield break; // Exit the coroutine early if a server is found
            }
            yield return null; // Wait for the next frame
        }

        // No server found, start a host
        loadingBar="Starting Game";
        Debug.Log("No server found, launching Server on Client");
        networkDiscovery.BroadcastAddress = CustomNetworkManager.singleton.networkAddress;
        networkDiscovery.StopDiscovery();
        CustomNetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }
    public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
            //Debug.Log("Info Gathered"+info.uri);
        }
}
