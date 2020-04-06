using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Testing_Bot_1.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
        [Command("gachihyper")]
        public async Task Gachihyper()
        {
            await ReplyAsync("https://cdn.betterttv.net/emote/59143b496996b360ff9b807c/3x");
        }
    }
}
