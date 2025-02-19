namespace Sentinel.Common.DTO.DeviceInformation;

public class SecurityInformation
{
    public required LastSecurityScan LastSecurityScan { get; set; }
    public bool AntivirusEnabled { get; set; }
    public DateTime? LastAntivirusUpdate { get; set; }
    public DateTime? LastAntispywareUpdate { get; set; }
    public bool RealTimeProtectionEnabled { get;set; }
    public bool NISEnabled { get; set; }   
    public bool TamperProtectionEnabled { get; set; }
    public bool AntispywareEnabled { get; set; }
    public bool IsVirtualMachine { get; set; }
    
    public FirewallSettings FirewallSettings { get; set; } = null!;
}

public class LastSecurityScan
{
    public DateTime? LastScan { get; set; }
    public TimeSpan? Duration { get; set; }
}

public class FirewallSettings
{
    public bool DomainFirewallEnabled { get; set; }
    public bool PrivateFirewallEnabled { get; set; }
    public bool PublicFirewallEnabled { get; set; }
}