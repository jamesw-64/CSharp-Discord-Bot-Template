using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using DeepAI;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    class Program
    {
        //create global variables 
        private DiscordSocketClient _client;

        System.IO.StreamReader tokenInput;
        string tokenLoop;
        string token;
        static string startTime = DateTime.Now.ToString("HH-mm-ss");

        DeepAI_API api = new DeepAI_API(apiKey: "nope");

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);

            Log("\n _              _       \n| \\o _ _ _ .__||_) __|_ \n|_/|_>(_(_)|(_||_)(_)|_ \n");

            try
            {
                tokenInput = new System.IO.StreamReader(@"token.txt");
            } catch (System.IO.FileNotFoundException)
            {
                Log("We could not find your token.txt file. This is needed to communicate with your bot. Would you like to enter one now? (Y/N) ", false);
                String input = Console.ReadLine();
                input = input.ToUpper();
                switch (input){
                    case "Y":
                        Log("Please input your token and press enter: ", false);
                        token = Console.ReadLine();

                        Log("Saving input...");
                        System.IO.File.WriteAllText(@"token.txt", token);
                        Log("token.txt saved to " + System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\token.txt");

                        Log("Thank you. The program will now restart... (STOPCODE 3)");
                        Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DiscordBot.exe");
                        Environment.Exit(3);
                        break;
                    case "N":
                        Log("Program cannont operate without bot token. Exitiing... (STOPCODE 1)");
                        Environment.Exit(1);
                        break;
                    default:
                        Log("Did not understand input. Exiting... (STOPCODE 2)");
                        Environment.Exit(2);
                        break;
                } 
            }

            while ((tokenLoop = tokenInput.ReadLine()) != null)
            {
                token = tokenLoop;
            }

            tokenInput.Close();

            Console.WriteLine("Welcome! Token is: " + token + " (this line is not logged, so don't worry about submitting the log file if there is a crash)");
            _client = new DiscordSocketClient();

            _client.Log += LogMsg;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceived;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            Log("Message Received! Contents: '" + message.Content + "' from " + message.Source + ": " + message.Author + " in channel: #" + message.Channel);
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
            if (message.Content.StartsWith("!generate"))
            {
                string msg = (message.Content).Replace("!generate ", "");
                Log("Sending: '" + msg + "' to DeepAI");

                await message.Channel.SendMessageAsync("Sending text to DeepAI and generating text, this may take a while and the bot will be unresponsive (maybe?) until task is completed. Please remember that the result of the generation does not reflect the views of any real people...");

                await SendToAI(msg, message);
            }
            if (message.Content == "!end" && (message.Author).ToString == "GhostCode#6938")
            {
                await message.Channel.SendMessageAsync("Good night!");
                Environment.Exit(0);
            }
        }

        private Task LogMsg(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg.ToString() + Environment.NewLine);
            return Task.CompletedTask;
        }

        private Task SendToAI(string msg, SocketMessage message)
        {
            StandardApiResponse resp = api.callStandardApi("text-generator", new
            {
                text = msg,
            });

            var data = (JObject)JsonConvert.DeserializeObject(api.objectAsJsonString(resp));

            System.IO.File.WriteAllText(@"deepai_output.txt", data["output"].Value<string>());

            message.Channel.SendFileAsync(@"deepai_output.txt");

            File.Delete(@"deepai_output.txt");

            return Task.CompletedTask;
        }

        static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string msg = DateTime.Now.ToString("HH:mm:ss") + ": Oops, the program has crash without being caught. Crash handler caught: " + e + "\nRuntime terminating? " + args.IsTerminating;
            Console.WriteLine();
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg + Environment.NewLine);
            Environment.Exit(4);
        }

        private void Log(string msg)
        {
            msg = DateTime.Now.ToString("HH:mm:ss") + ": " + msg;
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg + Environment.NewLine);
            Console.WriteLine(msg);
        }

        private void Log(string msg, Boolean WriteLine)
        {
            msg = DateTime.Now.ToString("HH:mm:ss") + ": " + msg;
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg + Environment.NewLine);
            switch (WriteLine)
            {
                case true:
                    Console.WriteLine(msg);
                    break;
                case false:
                    Console.Write(msg);
                    break;
            }
        }
    } // end class
} //end namespace
