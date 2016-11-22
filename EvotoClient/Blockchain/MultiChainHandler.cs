using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MultiChainLib;

namespace EvotoClient.Blockchain
{
    public class MultiChainHandler : IMultiChainHandler
    {
        public event EventHandler<EventArgs> OnConnect;

        private const string ChainHost = "michaelfiford.com";
        private const int ChainPort = 7211;
        private const string ChainName = "chain2";
        private const string RpcUser = "evoto";
        private const int RpcPort = 24533;

        private static readonly Random Random = new Random();
        private readonly string _password = RandomString(10);
        private MultiChainClient _client;
        private bool _connected;
        private Process _process;

        public bool Connected
        {
            get { return _connected; }
            private set
            {
                if (value)
                    OnConnect?.Invoke(this, null);
                _connected = value;
            }
        }

        public async Task Connect()
        {
            while (true)
            {
                var success = RunDaemon();

                if (success)
                {
                    // Give multichain a chance to start up
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    try
                    {

                        await ConnectRpc();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    if (!Connected)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        continue;
                    }
                }
                break;
            }
        }

        public void Disconnect()
        {

            StopDaemon();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private async Task ConnectRpc()
        {
            _client = new MultiChainClient("127.0.0.1", RpcPort, false, RpcUser, _password, ChainName);

            Console.WriteLine("Attempting to connect to MultiChain using RPC");

            try
            {
                var info = await _client.GetInfoAsync();

                Console.WriteLine($"Connected to {info.Result.ChainName}!");

                Connected = true;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Could not connect to MultiChain via RPC");
                Console.WriteLine(e.Message);
                Connected = false;
            }
        }

        private bool RunDaemon()
        {
            if (_process != null)
            {
                if (_process.HasExited)
                    Console.WriteLine("Restarting Multichain!!");
                else 
                    return true;
            }

            // TODO: Nicer way of running this?
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "multichaind.exe");

            var data = Environment.GetEnvironmentVariable("APPDATA") + Path.DirectorySeparatorChar + "Evoto";

            Console.WriteLine("Starting MultiChain");
            try
            {
                _process = new Process
                {
                    StartInfo =
                    {
                        // Stop the process from opening a new window
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,

                        // Setup executable and parameters
                        FileName = path,
                        Arguments =
                            $"{ChainName}@{ChainHost}:{ChainPort} -daemon -datadir={data} -server -rpcuser={RpcUser} -rpcpassword={_password} -rpcport={RpcPort}"
                    }
                };

                //Console.WriteLine(_process.StartInfo.Arguments);

                _process.ErrorDataReceived +=
                    (sender, args) => { Console.WriteLine($"Multichaind Error: {args.Data}"); };
                _process.OutputDataReceived += (sender, args) => { Console.WriteLine($"Multichaind: {args.Data}"); };

                // Go
                var success = _process.Start();

                if (!success)
                    throw new SystemException();

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start MultiChain. " + e.Message);
                return false;
            }
        }

        private void StopDaemon()
        {
            if (_process == null || _process.HasExited)
                return;

            _process.Close();
        }
    }
}