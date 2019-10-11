//using list
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

//File written (mostly) by James Wright. See the repo here: https://github.com/jamesw-64/DiscordBot

namespace DiscordBot
{
    class Program
    {
        //With some help from https://discord.foxbot.me/docs/guides/getting_started/first-bot.html

        //create global variables 
        private DiscordSocketClient _client;

        StreamReader tokenInput;
        string tokenLoop;
        string token;
        static string startTime = DateTime.Now.ToString("HH-mm-ss");

        //call main but redirect to MainAsync
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            //Prepare crash handler to save crash log
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);

            //Print logo (remove if you want)
            await Log("\n _              _       \n| \\o _ _ _ .__||_) __|_ \n|_/|_>(_(_)|(_||_)(_)|_ \n");
            
            try //to read in token
            {
                tokenInput = new StreamReader(@"token.txt");
            } catch (FileNotFoundException) //If we cannot find the token.txt file...
            {
                //Tell the user that they need to input a token
                Log("We could not find your token.txt file. This is needed to communicate with your bot. Would you like to enter one now? (Y/N) ", false);

                //read input
                String input = Console.ReadLine();

                //standarise input (make it capital)
                input = input.ToUpper();

                //switch case to check input
                switch (input){
                    case "Y":
                        //get the token input
                        Log("Please input your token and press enter: ", false);
                        token = Console.ReadLine();

                        //save the input
                        await Log("Saving input...");
                        File.WriteAllText(@"token.txt", token);
                        await Log("token.txt saved to " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\token.txt");

                        //restart program
                        await Log("Thank you. The program will now restart... (STOPCODE 3)");
                        Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DiscordBot.exe");
                        Environment.Exit(3);
                        break;
                    case "N":
                        //end the program as we cannot contiune without token
                        await Log("Program cannont operate without bot token. Exitiing... (STOPCODE 1)");
                        Environment.Exit(1);
                        break;
                    default:
                        //what was that input? we cannout continue without token
                        await Log("Did not understand input. Exiting... (STOPCODE 2)");
                        Environment.Exit(2);
                        break;
                } 
            }

            //clean up token as some OS's add a newline to the bottom of a file,
            //which can crash the program.
            while ((tokenLoop = tokenInput.ReadLine()) != null)
            {
                token = tokenLoop;
            }

            //close the file containing the token
            tokenInput.Close();

            /*
             *
             * IF YOU DO NOT WANT TO USE A FILE TO READ IN THE TOKEN, DO THE FOLLOWING:
             *
             *  - REMOVE StreamReader tokenInput; on line 18
             *  - REMOVE string tokenLoop; on line 19
             *  - REMOVE lines 36 - 73
             *
             *  - ADD token = "<insert token here>";
             *
             *  However, if you decide to hardcode your token into the program  please
             *  remember to not push this to a public repo as doing so will expose
             *  your bot token, allowing anyone to control your bot. Consider making
             *  methods that are async Tasks so that log running commands don't freeze
             *  up the bot.
             *  
             */

            //print token to screen but don't log it as doing so may expose token
            Console.WriteLine("Welcome! Token is: " + token + " (this line is not logged, so don't worry about submitting the log file if there is a crash)");

            //create a new socketclient
            _client = new DiscordSocketClient();

            //what to do when the _client needs to output
            _client.Log += Log;

            //Log in using token
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //what to to when a message is recieved (watches all channel in the server it's in)
            _client.MessageReceived += MessageReceived;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            //output to console the message that was received
            //IDEA FOR IMPORVEMENT: LOG MESSAGES TO A DATABASE AND NOT A FILE
            await Log("Message Received! Contents: '" + message.Content + "' from " + message.Source + ": " + message.Author + " in channel: #" + message.Channel);

            /*
             *
             * This is where you program your main functionality of your program.
             * You can use if/else statments or switch/case stamtements to program
             * in commands.
             *
             */

            switch (message.Content)
            {
                //an example of a command
                case "!ping":
                    await message.Channel.SendMessageAsync("Pong!");
                    break;
            }
        }

        //Logging for _client
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg.ToString() + Environment.NewLine);
            return Task.CompletedTask;
        }

        //general logging
        private Task Log(string msg)
        {
            msg = DateTime.Now.ToString("HH:mm:ss") + ": " + msg;
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg + Environment.NewLine);
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        //Logging, but with the option to use a WriteLine or Write statement
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

        //crash handler that will save Stack trace on unhandled execption
        static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string msg = DateTime.Now.ToString("HH:mm:ss") + ": Oops, the program has crash without being caught. Crash handler caught: " + e + "\nRuntime terminating? " + args.IsTerminating;
            Console.WriteLine();
            File.AppendAllText(@"DiscordBot_log_" + startTime + ".log", msg + Environment.NewLine);
            Environment.Exit(4);
        }
    } // end class
} //end namespace
