namespace Sentinel.Api.Domain.Entities;

public class DeviceSecurity
{
    public int Id { get; set; }
    public DateTime? LastScan { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? LastAntivirusUpdate { get; set; }
    public DateTime? LastAntispywareUpdate { get; set; }
    
    public bool IsVirtualMachine { get; set; }
    
    public bool AntivirusEnabled { get; set; }
    public bool RealTimeProtectionEnabled { get;set; }
    public bool NISEnabled { get; set; }   
    public bool TamperProtectionEnabled { get; set; }
    public bool AntispywareEnabled { get; set; }
    
    public bool DomainFirewallEnabled { get; set; }
    public bool PrivateFirewallEnabled { get; set; }
    public bool PublicFirewallEnabled { get; set; }
}
