using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Discovery;
using TMPro;
using System;
using kcp2k;

public class StartMenu : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public GameObject networkObject;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;
    private TMP_Text loadingText;
    public TMP_Text debugText;
    private string uri;

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
        debugText.text = uri;
        //Debug.Log("Servers Found = " + discoveredServers.Count);
    }
    public void StartGame()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();

        StartCoroutine(WaitForServerAndStart());
        //networkManager.networkAddress = "localhost"; // set the IP address of the server
        print(discoveredServers.Count);
    }
    private IEnumerator WaitForServerAndStart()
    {
        int startRange = 1; // Starting range for the IP address (xxx)
        int endRange = 254; // Ending range for the IP address (xxx)
        int portStart = 7777; // Starting port range
        int portEnd = 7799; // Ending port range

        string loadingBar = ".";
        loadingText.gameObject.SetActive(true);

        // Get the subnet of the device
        string subnet = GetSubnet();

        for (int endPoint = startRange; endPoint <= endRange; endPoint++)
        {
            for (int port = portStart; port <= portEnd; port++)
            {
                string ipAddress = subnet + endPoint;
                string uriString = "kcp://" + ipAddress + ":" + port;
                loadingText.text = "Looking For Game\n" + loadingBar;

                // Try to convert the URI string to a URI object
                Uri uri = null;
                if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                {
                    // Try to connect to the server at the current IP address and port using KCP transport
                    NetworkManager.singleton.StartClient(uri);

                    // Wait for a short time to give the client a chance to connect
                    yield return new WaitForSeconds(1f);

                    // Check if the client successfully connected
                    if (NetworkClient.isConnected)
                    {
                        // Connected to the server, stop discovery
                        networkDiscovery.StopDiscovery();
                        loadingText.text = "Connected to Server";
                        yield break; // Exit the coroutine
                    }
                    else
                    {
                        // Connection attempt failed, disconnect and try the next port
                        networkManager.StopClient();
                    }
                }
            }
        }

        // No server found, start a host
        loadingText.text = "No server found, starting Server on Client";
        networkDiscovery.BroadcastAddress = CustomNetworkManager.singleton.networkAddress;
        networkDiscovery.StopDiscovery();
        CustomNetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    private string GetSubnet()
    {
        // Get all network interfaces of the device
        System.Net.NetworkInformation.NetworkInterface[] networkInterfaces =
            System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

        // Find the first network interface with an IPv4 address
        foreach (System.Net.NetworkInformation.NetworkInterface networkInterface in networkInterfaces)
        {
            // Consider only operational and non-loopback interfaces
            if (networkInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
            {
                // Get the IP properties of the network interface
                System.Net.NetworkInformation.IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                // Find the first IPv4 address assigned to the network interface
                foreach (System.Net.NetworkInformation.UnicastIPAddressInformation unicastAddress in ipProperties.UnicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        string ipAddress = unicastAddress.Address.ToString();
                        string[] parts = ipAddress.Split('.');
                        // Return the subnet (first three parts of the IP address)
                        return parts[0] + "." + parts[1] + "." + parts[2] + ".";
                    }
                }
            }
        }

        // If no suitable IP address is found, return a default value ("10.1.0.")
        return "127.0.0.";
    }

    public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
            //Debug.Log("Info Gathered"+info.uri);
        }
}
