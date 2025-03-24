using System.Collections.Generic;

public class Config
{
    public string SelectedNetworkInterface { get; set; }
    public List<string> DnsServers { get; set; } = new List<string>();
} 