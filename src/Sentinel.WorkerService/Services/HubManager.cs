using Microsoft.AspNetCore.SignalR.Client;

namespace Sentinel.WorkerService.Services;

public static class HubManager
{
    public static HubConnection Initialize(string hubName, HostBuilderContext hostContext)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl($"{hostContext.Configuration.GetConnectionString("Api")}/{hubName}", opts =>
            {
                opts.AccessTokenProvider = () => Task.FromResult(hostContext.Configuration["AccessToken"]);
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        clientHandler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
                    return message;
                };
            })
            .Build();
        return hubConnection;
    }

    public static async Task Connect(HubConnection hubConnection)
    {
        while (hubConnection.State != HubConnectionState.Connected)
        {
            string? connectionId = null;
            try
            {
                await hubConnection.StartAsync();
                connectionId = hubConnection.ConnectionId;
                break;
            }
            catch (Exception ex)
            {
                await Task.Delay(10000);
            }

            var x = connectionId;
            // TODO: put connectionId to device endpoint
        }
    }
}