using System.Globalization;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Text.Json;

namespace Todd;

public static class Program
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    // ReSharper disable once MemberCanBePrivate.Global
    internal static DiscordRpcClient client;
    // ReSharper disable once MemberCanBePrivate.Global
    internal static ToddLogger logger;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private const string TODD_FOLDER = ".todd";
    
    private const string TODD_ID_FILE = ".id";

    private const string TODD_LOG_FOLDER = "logs";
    
    private const string TODD_CONFIG_FILE = "config.json";

    public static int Main()
    {
        int ec = Program.NotMain();

        if (ec is 0 or 2)
            return ec;
        
        Console.Error.WriteLine("Program exited with a non-zero exit code, meaning an error has occured");
        Console.Error.WriteLine($"Please check '{Program.logger.File}'");
        return ec;
    }
    
    private static int NotMain()
    {
        string toddFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Program.TODD_FOLDER);

        if (!Directory.Exists(toddFolder))
        {
            Exception? err = null;
            
            try
            {
                Directory.CreateDirectory(toddFolder);
            }
            catch (IOException e)
            {
                err = e;
            }
            catch (UnauthorizedAccessException e)
            {
                err = e;
            }

            if (err is not null)
            {
                Console.Error.WriteLine($"could not create todd folder: {err.Message}");
                return 2;
            }
        }
        
        FileInfo idFile = new(Path.Join(toddFolder, Program.TODD_ID_FILE));

        FileInfo configFile = new(Path.Join(toddFolder, Program.TODD_CONFIG_FILE));

        DirectoryInfo logFolder = new(Path.Join(toddFolder, Program.TODD_LOG_FOLDER));

        if (!logFolder.Exists)
        {
            Console.WriteLine("logs folder not found, creating...");
            
            Exception? err = null;
            
            try
            {
                Directory.CreateDirectory(logFolder.FullName);
            }
            catch (IOException e)
            {
                err = e;
            }
            catch (UnauthorizedAccessException e)
            {
                err = e;
            }

            if (err is not null)
            {
                Console.Error.WriteLine($"could not create logs folder: {err.Message}");
                return 2;
            }
            
            Console.WriteLine("logs folder created");
        }
        
        Program.logger = new ToddLogger(Path.Join(logFolder.FullName, $"{Directory.GetFiles(logFolder.FullName).Length}_{DateTime.Now.ToString(CultureInfo.InvariantCulture).Split(' ')[0].Replace('/', '-')}.log"));
        
        if (!idFile.Exists)
        {
            Program.logger.LogBoth("application ID has not been saved yet");
            
            string? id;
            
            while (true)
            {
                try
                {
                    Console.Write("Please enter your application ID: ");
                    id = Console.ReadLine();
                    if (id is not null && id != "")
                        break;
                    Console.WriteLine("Please enter a non-empty value");
                }
                catch (IOException e)
                {
                    Program.logger.Error($"could not read input: {e.Message}");
                    return 1;
                }
            }
            
            Program.logger.Info($"application ID declared as: {id}");

            try
            {
                File.WriteAllText(idFile.FullName, id);
            }
            catch (IOException e)
            {
                Program.logger.Error($"could not write to '{idFile.Name}': {e.Message}");
                return 1;
            }
            
            Program.logger.LogBoth("application ID saved");
            Program.logger.Info($"created ID file at path '{idFile.FullName}'");
        }

        ToddConfig config = ToddConfig.Default;

        if (!configFile.Exists)
        {
            Program.logger.Info("config file not found, creating...");
            
            JsonSerializerOptions jsonOptions = new()
            {
                WriteIndented = true
            };

            try
            {
                FileStream stream = configFile.Create();
                JsonSerializer.Serialize(stream, config, jsonOptions);
                stream.Close();
            }
            catch (IOException e)
            {
                Program.logger.Error($"could not write config file: {e.Message}");
                return 1;
            }
            catch (NotSupportedException e)
            {
                Program.logger.Error($"could not serialize default config: {e.Message}");
                return 1;
            }
            
            Program.logger.Info($"created config file at path '{configFile.FullName}'");
        }

        try
        {
            config = JsonSerializer.Deserialize<ToddConfig>(File.ReadAllText(configFile.FullName))!;
        }
        catch (IOException e)
        {
            Program.logger.Error($"could not read config file: {e.Message}");
            return 1;
        }
        catch (JsonException e)
        {
            Program.logger.Error($"could not parse config file: {e.Message}");
            return 1;
        }
        catch (NotSupportedException e)
        {
            Program.logger.Error($"could not parse config file: {e.Message}");
            return 1;
        }

        try
        {
            Program.client = new DiscordRpcClient(File.ReadAllText(idFile.FullName));
            Program.logger.Info("Connected to Discord");
        }
        catch (IOException e)
        {
            Program.logger.Error($"could not read '{idFile.Name}': {e.Message}");
            return 1;
        }

        Program.client.Logger = new ConsoleLogger
        {
            Level = LogLevel.Warning
        };

        Program.client.OnReady += (sender, e) =>
        {
            Program.logger.Info($"Received Ready from user {e.User.Username}");
        };

        Program.client.OnPresenceUpdate += (sender, e) =>
        {
            Program.logger.Info($"Received Update! {e.Presence}");
        };

        if (!Program.client.IsInitialized)
            Program.client.Initialize();

        RichPresence presence = new()
        {
            Details = config.Details,
            State = config.State,
            Assets = new Assets
            {
                LargeImageKey = config.Assets.LargeImage,
                LargeImageText = config.Assets.LargeImageText,
                SmallImageKey = config.Assets.SmallImage,
            },
            Buttons = config.Buttons.Select(button => new Button { Label = button.Label, Url = button.Url }).ToArray(),
            Timestamps = Timestamps.Now,
        };

        Program.client.SetPresence(presence);
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();

        if (!Program.client.IsDisposed)
            Program.client.Dispose();
        
        return 0;
    }
}