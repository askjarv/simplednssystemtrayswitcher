using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

public class MainForm : Form
{
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private SettingsForm settingsForm;

    public MainForm()
    {
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Settings", null, OnSettings);
        trayMenu.Items.Add("Exit", null, OnExit);

        trayIcon = new NotifyIcon();
        trayIcon.Text = "DNS Switcher";
        trayIcon.Icon = new Icon("appicon.ico");
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.Visible = true;

        settingsForm = new SettingsForm();
        UpdateTrayMenu();
    }

    protected override void OnLoad(EventArgs e)
    {
        Visible = false;
        ShowInTaskbar = false;
        base.OnLoad(e);
    }

    private void OnSettings(object sender, EventArgs e)
    {
        settingsForm.ShowDialog();
        UpdateTrayMenu();
    }

    private void UpdateTrayMenu()
    {
        // Clear existing DNS items
        for (int i = trayMenu.Items.Count - 1; i >= 2; i--)
        {
            trayMenu.Items.RemoveAt(i);
        }

        // Add DNS server items
        foreach (var dns in settingsForm.DnsServers)
        {
            trayMenu.Items.Add(dns, null, (s, e) => SetDns(dns));
        }
    }

    private void SetDns(string dns)
    {
        string selectedInterface = settingsForm.SelectedNetworkInterface;
        if (!string.IsNullOrEmpty(selectedInterface))
        {
            settingsForm.SetDns(selectedInterface, dns);
        }
    }

    private void OnExit(object sender, EventArgs e)
    {
        Application.Exit();
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            trayIcon.Dispose();
        }
        base.Dispose(isDisposing);
    }
} 