namespace Sentinel.Common.DTO.Device;

public class SecurityInformationDto
{
    public required LastSecurityScanDto LastSecurityScanDto { get; set; }
    public bool AntivirusEnabled { get; set; }
    public DateTime? LastAntivirusUpdate { get; set; }
    public DateTime? LastAntispywareUpdate { get; set; }
    public bool RealTimeProtectionEnabled { get;set; }
    public bool NisEnabled { get; set; }   
    public bool TamperProtectionEnabled { get; set; }
    public bool AntispywareEnabled { get; set; }
    public bool IsVirtualMachine { get; set; }
    
    public FirewallSettingsDto FirewallSettingsDto { get; set; } = null!;
}

public class LastSecurityScanDto
{
    public DateTime? LastScan { get; set; }
    public TimeSpan? Duration { get; set; }
}

public class FirewallSettingsDto
{
    public bool DomainFirewallEnabled { get; set; }
    public bool PrivateFirewallEnabled { get; set; }
    public bool PublicFirewallEnabled { get; set; }
}