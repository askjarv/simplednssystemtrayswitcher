using System;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

public class SettingsForm : Form
{
    private ComboBox networkInterfacesComboBox;
    private TextBox dnsTextBox;
    private Button saveButton;
    private Button addDnsButton;
    private ListBox dnsListBox;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> DnsServers { get; private set; } = new List<string>();

    public string SelectedNetworkInterface
    {
        get => networkInterfacesComboBox.SelectedItem?.ToString();
    }

    public SettingsForm()
    {
        Text = "DNS Settings";
        Width = 300;
        Height = 300;
        Icon = new Icon("appicon.ico");
        // Add a label to the form  
        Label label = new Label() { Text = "Select a network interface:", Left = 10, Top = 10, Width = 260 };
        Controls.Add(label);
        networkInterfacesComboBox = new ComboBox() { Left = 10, Top = 30, Width = 260 };
        // Add a label to the form  
        Label label2 = new Label() { Text = "Enter a DNS server:", Left = 10, Top = 50, Width = 260 };
        Controls.Add(label2);
        dnsTextBox = new TextBox() { Left = 10, Top = 70, Width = 260 };
        addDnsButton = new Button() { Text = "Add DNS", Left = 10, Top = 90, Width = 260 };
        dnsListBox = new ListBox() { Left = 10, Top = 130, Width = 260, Height = 100 };
        saveButton = new Button() { Text = "Save", Left = 10, Top = 240, Width = 260 };

        addDnsButton.Click += AddDnsButton_Click;
        saveButton.Click += SaveButton_Click;

        Controls.Add(networkInterfacesComboBox);
        Controls.Add(dnsTextBox);
        Controls.Add(addDnsButton);
        Controls.Add(dnsListBox);
        Controls.Add(saveButton);

        LoadNetworkInterfaces();
        LoadConfig();
    }

    private void LoadNetworkInterfaces()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            networkInterfacesComboBox.Items.Add(ni.Name);
        }
    }

    private void AddDnsButton_Click(object sender, EventArgs e)
    {
        string dns = dnsTextBox.Text;
        if (!string.IsNullOrWhiteSpace(dns) && !DnsServers.Contains(dns))
        {
            DnsServers.Add(dns);
            dnsListBox.Items.Add(dns);
            dnsTextBox.Clear();
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        SaveConfig();
        this.Close();
    }

    public void SetDns(string networkInterface, string dns)
    {
        ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set dns \"{networkInterface}\" static {dns}");
        psi.Verb = "runas"; // Run as administrator
        Process.Start(psi);
    }

    private void LoadConfig()
    {
        if (File.Exists("config.json"))
        {
            string json = File.ReadAllText("config.json");
            Config config = JsonSerializer.Deserialize<Config>(json);
            if (config != null)
            {
                DnsServers = config.DnsServers;
                dnsListBox.Items.AddRange(DnsServers.ToArray());

                if (!string.IsNullOrEmpty(config.SelectedNetworkInterface))
                {
                    networkInterfacesComboBox.SelectedItem = config.SelectedNetworkInterface;
                }
            }
        }
    }

    private void SaveConfig()
    {
        Config config = new Config
        {
            SelectedNetworkInterface = SelectedNetworkInterface,
            DnsServers = DnsServers
        };

        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("config.json", json);
    }
} 