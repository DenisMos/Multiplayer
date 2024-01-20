internal interface INetworkAdapter
{
    void StartServer(string ip, int port);

    bool StartClient(string ip, int port, string ipDistance, int portDistance);
}