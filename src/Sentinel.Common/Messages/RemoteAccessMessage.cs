namespace Sentinel.Common.Messages;

public sealed record RemoteAccessMessage;
public sealed record RemoteAccessResponseMessage(string ConnectionId);