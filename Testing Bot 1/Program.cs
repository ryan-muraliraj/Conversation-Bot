using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AIMLbot;

namespace Testing_Bot_1
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private static ulong channelanchorid = INSERT_CHANNEL_ID_HERE;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static Bot AI = new Bot();
        public User myuser = new User("Username Here", AI);
        public Boolean enableprefixneccessity = true;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = "INSERT_BOT_TOKEN_HERE";

            AI.loadSettings();
            AI.loadAIMLFromFiles();
            AI.isAcceptingUserInput = true;
            

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            while (true)
            {

                string input = Console.ReadLine();
                handleConsoleInput(input);
               

                
            }
            

            
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;
            Console.WriteLine("User: " + message);



            int argPos = 0;

            if (message.HasStringPrefix("/", ref argPos))
            {
                string stringmessage = message.ToString();
                string[] splitstring = stringmessage.Split('/');
                string sendtoconsole = splitstring[1];

                var chnl = _client.GetChannel(channelanchorid) as IMessageChannel;
                await chnl.SendMessageAsync(handleConsoleInput(sendtoconsole));
            }
            else if (!enableprefixneccessity)
            {
                string stringmessage = message.ToString();
                string sendtoaiml = stringmessage;

                Request r = new Request(sendtoaiml, myuser, AI);
                Result res = AI.Chat(r);
                Console.WriteLine("Bot: " + res.Output);
                var chnl = _client.GetChannel(channelanchorid) as IMessageChannel;
                await chnl.SendMessageAsync(res.Output);
            }
            else
            {


                if (message.HasStringPrefix("?", ref argPos))
                {

                    string stringmessage = message.ToString();
                    string[] splitstring = stringmessage.Split('?');
                    string sendtoaiml = splitstring[1];

                    Request r = new Request(sendtoaiml, myuser, AI);
                    Result res = AI.Chat(r);
                    Console.WriteLine(res.Output);
                    var chnl = _client.GetChannel(channelanchorid) as IMessageChannel;
                    await chnl.SendMessageAsync(res.Output);

                    /*
                    # var result = await _commands.ExecuteAsync(context, argPos, _services);
                    #if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                    */
                }

            }
        
            /*else
            {
                
            }
            */
        } 
        public string handleConsoleInput(String input)
        {
            if(input == "enable -p")
            {
                enableprefixneccessity = true;
                Console.WriteLine("Console: Prefix necessity has been set to true");
                return "Prefix necessity has been set to true";
            } else if (input == "disable -p")
            {
                enableprefixneccessity = false;
                Console.WriteLine("Console: Prefix necessity has been set to false");
                return "Prefix necessity has been set to false";
            } else if (input == "check -p")
            {
                Console.WriteLine("Console: Returning prefix boolean");
                return "Prefix necessity is currently set to " + enableprefixneccessity;
            } else if (input == "stop")
            {
                _client.StopAsync();
                Console.WriteLine("Console: Shutting down discord connection");
                return "Shutting down bot, console must be manually closed";
            } else if (input == "reload")
            {
                Console.WriteLine("Console: Reloading AI assets and files");
                AI.isAcceptingUserInput = false;
                AI.loadSettings();
                AI.loadAIMLFromFiles();
                AI.isAcceptingUserInput = true;
                return "Reloading AI engine assets";
            } else if (input == "restart")
            {
                _client.StopAsync();
                Console.WriteLine("Console: Shutting down discord connection");
                _client.StartAsync();
                return "Restarting bot, data could get corrupted";
            }

            return input;
        }



    }
}
