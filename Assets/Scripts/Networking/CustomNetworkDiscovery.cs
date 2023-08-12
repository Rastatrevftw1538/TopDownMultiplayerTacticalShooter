using System;
using System.Net;
using UnityEngine;
using Mirror.Discovery;
using UnityEngine.Events;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    private CustomNetworkManager netManager;

    private void Awake() {
        netManager = this.GetComponent<CustomNetworkManager>();
    }
    protected override ServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
        {
            // In this case we don't do anything with the request
            // but other discovery implementations might want to use the data
            // in there,  This way the client can ask for
            // specific game mode or something

            try
            {
                // this is an example reply message,  return your own
                // to include whatever is relevant for your game
                return new ServerResponse
                {
                    serverId = ServerId,
                    uri = transport.ServerUri(),
                    joinCode = netManager.joinCode
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
        }
}
