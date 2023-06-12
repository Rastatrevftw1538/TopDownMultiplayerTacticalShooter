using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Discovery;
public class StartMenu : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public GameObject networkObject;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    private void Start() {
        networkManager = networkObject.GetComponent<NetworkManager>();
        networkDiscovery = networkObject.GetComponent<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        networkDiscovery.StartDiscovery();
        //NetworkManager.singleton.StartClient();

        //UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
    }
    public void StartGame()
    {

        //networkManager.networkAddress = "localhost"; // set the IP address of the server
        Debug.Log(discoveredServers.Count);
        if (discoveredServers.Count >= 1) {
            foreach (long serverKey in discoveredServers.Keys) {
                Debug.Log(serverKey);
                Debug.Log(discoveredServers[serverKey].uri);
                    networkDiscovery.StopDiscovery();
                    NetworkManager.singleton.StartClient(discoveredServers[serverKey].uri); // start the client and connect to the server
                    //SceneManager.LoadScene(1);
                    break;
            }
        }
        else {
            Debug.Log("No servers are running at the moment");
        }
    }
    public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
            //Debug.Log("Info Gathered"+info.uri);
        }
}
