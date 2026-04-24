namespace Mycelium.Common.SignalR;

public sealed record RemoteAccessMessage;
public sealed record RemoteAccessResponseMessage(string ConnectionId);