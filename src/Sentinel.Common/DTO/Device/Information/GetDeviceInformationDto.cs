﻿namespace Sentinel.Common.DTO.Device.Information;

public class GetDeviceInformationDto
{
    // General info
    public string? DeviceName { get; set; }
    public string? OsName { get; set; }
    public string? OsVersion{ get; set; }
    public string? Version { get; set; }

    // Device specs
    public string? ProductName { get; set; }
    public string? Processor { get; set; }
    public string? InstalledRam { get; set; }
    public string? GraphicsCard { get; set; }
    public string? Manufacturer { get; set; }
}