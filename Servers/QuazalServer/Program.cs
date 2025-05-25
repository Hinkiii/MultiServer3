using CustomLogger;
using Microsoft.Extensions.Logging;
using NetworkLibrary;
using NetworkLibrary.Extension;
using NetworkLibrary.SNMP;
using Newtonsoft.Json.Linq;
using QuazalServer.ServerProcessors;
using System.Reflection;
using System.Runtime;

public static class QuazalServerConfiguration
{
    public static string ServerBindAddress { get; set; } = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
    public static string ServerPublicBindAddress { get; set; } = InternetProtocolUtils.GetPublicIPAddress();
    public static string EdNetBindAddressOverride { get; set; } = string.Empty;
    public static string QuazalStaticFolder { get; set; } = $"{Directory.GetCurrentDirectory()}/static/Quazal";
    public static bool UsePublicIP { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result;
    public static List<Tuple<int, string, string>>? BackendServersList { get; set; } = new List<Tuple<int, string, string>>
                    {
                        Tuple.Create(25101, "pbuT0dSs", "PS3UbisoftServices"), // SPARTACUSLEGENDS
                    };
    public static List<Tuple<int, int, string, string>>? RendezVousServersList { get; set; } = new List<Tuple<int, int, string, string>>
                    {
                        Tuple.Create(25100, 25101, "pbuT0dSs", "PS3UbisoftServices"), // SPARTACUSLEGENDS
                    };

    /// <summary>
    /// Tries to load the specified configuration file.
    /// Throws an exception if it fails to find the file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void RefreshVariables(string configPath)
    {
        // Make sure the file exists
        if (!File.Exists(configPath))
        {
            LoggerAccessor.LogWarn("Could not find the quazal.json file, writing and using server's default.");

            Directory.CreateDirectory(Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory() + "/static");

            // Write the JObject to a file
            File.WriteAllText(configPath, new JObject(
                new JProperty("config_version", (ushort)2),
                new JProperty("server_bind_address", ServerBindAddress),
                new JProperty("server_public_bind_address", ServerPublicBindAddress),
                new JProperty("ednet_bind_address_override", EdNetBindAddressOverride),
                new JProperty("quazal_static_folder", QuazalStaticFolder),
                new JProperty("server_public_ip", UsePublicIP),
                new JProperty("backend_servers_list", new JArray(
                    from item in BackendServersList
                    select new JObject(
                        new JProperty("item1", item.Item1),
                        new JProperty("item2", item.Item2),
                        new JProperty("item3", item.Item3)
                    )
                )),
                new JProperty("rendezvous_servers_list", new JArray(
                    from item in RendezVousServersList
                    select new JObject(
                        new JProperty("item1", item.Item1),
                        new JProperty("item2", item.Item2),
                        new JProperty("item3", item.Item3),
                        new JProperty("item4", item.Item4)
                    )
                ))
            ).ToString());

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
            if (config_version == 2)
            {
                ServerBindAddress = GetValueOrDefault(config, "server_bind_address", ServerBindAddress);
                ServerPublicBindAddress = GetValueOrDefault(config, "server_public_bind_address", ServerPublicBindAddress);
                EdNetBindAddressOverride = GetValueOrDefault(config, "ednet_bind_address_override", EdNetBindAddressOverride);
                QuazalStaticFolder = GetValueOrDefault(config, "quazal_static_folder", QuazalStaticFolder);
                UsePublicIP = GetValueOrDefault(config, "server_public_ip", UsePublicIP);
                // Deserialize BackendServersList if it exists
                try
                {
                    JArray BackendServersListArray = config.backend_servers_list;
                    if (BackendServersListArray != null)
                        BackendServersList = BackendServersListArray.ToObject<List<Tuple<int, string, string>>>();
                }
                catch
                {

                }
                // Deserialize RendezVousServersList if it exists
                try
                {
                    JArray RendezVousServersListArray = config.rendezvous_servers_list;
                    if (RendezVousServersListArray != null)
                        RendezVousServersList = RendezVousServersListArray.ToObject<List<Tuple<int, int, string, string>>>();
                }
                catch
                {

                }
            }
            else
                LoggerAccessor.LogWarn($"quazal.json file is outdated, using server's default.");
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogWarn($"quazal.json file is malformed (exception: {ex}), using server's default.");
        }
    }

    // Helper method to get a value or default value if not present
    private static T GetValueOrDefault<T>(dynamic obj, string propertyName, T defaultValue)
    {
        try
        {
            if (obj != null)
            {
                if (obj is JObject jObject)
                {
                    if (jObject.TryGetValue(propertyName, out JToken? value))
                    {
                        T? returnvalue = value.ToObject<T>();
                        if (returnvalue != null)
                            return returnvalue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError($"[Program] - GetValueOrDefault thrown an exception: {ex}");
        }

        return defaultValue;
    }
}

class Program
{
    public static string configDir = Directory.GetCurrentDirectory() + "/static/";
    private static string configPath = configDir + "quazal.json";
    private static string configNetworkLibraryPath = configDir + "NetworkLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static BackendServicesServer? BackendServer;
    private static RDVServer? RendezVousServer;

    private static void StartOrUpdateServer()
    {
        BackendServer?.Stop();
        RendezVousServer?.Stop();
        QuazalServer.RDVServices.ServiceFactoryRDV.ClearServices();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        BackendServer = new BackendServicesServer();
        RendezVousServer = new RDVServer();
        BackendServer.Start(QuazalServerConfiguration.BackendServersList
                    , 2, new CancellationTokenSource().Token);
        RendezVousServer.Start(QuazalServerConfiguration.RendezVousServersList
                    , 2, new CancellationTokenSource().Token);
    }

    static void Main()
    {
        if (!NetworkLibrary.Extension.Windows.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(Assembly.GetEntryAssembly()?.Location);

        LoggerAccessor.SetupLogger("QuazalServer", Directory.GetCurrentDirectory());

#if DEBUG
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            LoggerAccessor.LogError("[Program] - A FATAL ERROR OCCURED!");
            LoggerAccessor.LogError(args.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            LoggerAccessor.LogError("[Program] - A task has thrown a Unobserved Exception!");
            LoggerAccessor.LogError(args.Exception);
            args.SetObserved();
        };
#endif

        NetworkLibraryConfiguration.RefreshVariables(configNetworkLibraryPath);

        if (NetworkLibraryConfiguration.EnableSNMPReports)
        {
            trapSender = new SnmpTrapSender(NetworkLibraryConfiguration.SNMPHashAlgorithm.Name, NetworkLibraryConfiguration.SNMPTrapHost, NetworkLibraryConfiguration.SNMPUserName,
                    NetworkLibraryConfiguration.SNMPAuthPassword, NetworkLibraryConfiguration.SNMPPrivatePassword,
                    NetworkLibraryConfiguration.SNMPEnterpriseOid);

            if (trapSender.report != null)
            {
                LoggerAccessor.RegisterPostLogAction(LogLevel.Information, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Warning, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendWarn(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Error, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Critical, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });
#if DEBUG
                LoggerAccessor.RegisterPostLogAction(LogLevel.Debug, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });
#endif
            }
        }

        QuazalServerConfiguration.RefreshVariables(configPath);

        StartOrUpdateServer();

        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            while (true)
            {
                LoggerAccessor.LogInfo("Press any keys to access server actions...");

                Console.ReadLine();

                LoggerAccessor.LogInfo("Press one of the following keys to trigger an action: [R (Reboot),S (Shutdown)]");

                switch (char.ToLower(Console.ReadKey().KeyChar))
                {
                    case 's':
                        LoggerAccessor.LogWarn("Are you sure you want to shut down the server? [y/N]");

                        if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
                        {
                            LoggerAccessor.LogInfo("Shutting down. Goodbye!");
                            Environment.Exit(0);
                        }
                        break;
                    case 'r':
                        LoggerAccessor.LogWarn("Are you sure you want to reboot the server? [y/N]");

                        if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
                        {
                            LoggerAccessor.LogInfo("Rebooting!");

                            QuazalServerConfiguration.RefreshVariables(configPath);

                            StartOrUpdateServer();
                        }
                        break;
                }
            }
        }
        else
        {
            LoggerAccessor.LogWarn("\nConsole Inputs are locked while server is running. . .");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}