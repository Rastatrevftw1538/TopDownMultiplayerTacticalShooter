using System;
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
    private TMP_InputField ipInput;

    private void Start() {
        loadingText = GameObject.Find("Searching For Game").GetComponent<TMP_Text>();
        loadingText.gameObject.SetActive(false);
        ipInput = GameObject.Find("IPInput").GetComponent<TMP_InputField>();
        networkManager = networkObject.GetComponent<CustomNetworkManager>();
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
                long serverKey = discoveredServers.Keys.First<long>();
                string joinCode = discoveredServers[serverKey].joinCode;
                Debug.Log(serverKey);
                Debug.Log("<color=green> Join Code: "+joinCode+"</color>");
                /*if(joinCode!=""){
                    try{
                    // Join the Relay host
                    await RelayService.Instance.JoinAsync(new JoinRequest { JoinCode = joinCode });
                    */
                   // Debug.Log("Joined the Relay host successfully!");
                    Debug.Log(discoveredServers[serverKey].uri);
                    networkManager.StartClient(discoveredServers[serverKey].uri);
                    /*}
                    catch (Exception e)
                    {
                        Debug.LogError("Error joining the Relay host: " + e.Message);
                    }*/
                
                yield break;
            }
            else if (ipInput.text != ""){
                int port = 7777;
                Uri uri = new Uri($"kcp://{ipInput}:{port}");
                print(uri);
                networkManager.StartClient(uri);
            }
            yield return null; // Wait for the next frame
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
