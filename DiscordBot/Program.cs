using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace DiscordBot
{
    class Program
    {
        private DiscordSocketClient _client;

        System.IO.StreamReader tokenInput;
        string tokenLoop;
        string token;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            try
            {
                tokenInput = new System.IO.StreamReader(@"token.txt");
            } catch (System.IO.FileNotFoundException Ex)
            {
                Console.WriteLine("We could not find your token.txt file. This is needed to communicate with your bot. Please place it in the /bin/netcoreapp3.0/ directory.");
                Environment.Exit(1);
            }

            while ((tokenLoop = tokenInput.ReadLine()) != null)
            {
                token = tokenLoop;
            }

            tokenInput.Close();

            Console.WriteLine("Welcome! Token is: " + token);
            _client = new DiscordSocketClient();

            _client.Log += Log;

            // Remember to keep token private or to read it from an 
            // external source! In this case, we are reading the token 
            // from an environment variable. If you do not know how to set-up
            // environment variables, you may find more information on the 
            // Internet or by using other methods such as reading from 
            // a configuration.
            await _client.LoginAsync(TokenType.Bot,
                token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceived;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        //static void Main(string[] args)
        //{
        //TODO Create init menu to input tokens and store the output locally
        //     to prevent token leaking
        //Console.WriteLine("Hello World!");
        //}
    }
}
