﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alienseed.BaseNetworkServer.Network.Telnet.Prompts
{
    class MainMenu : BaseMenuPrompt
    {
        public override string MenuName
        {
            get { return "Main Menu"; }
        }

        public MainMenu(bool foradmin = false) : base(options, foradmin) { }

        private static MenuOption[] options = new MenuOption[] {
            new MenuOption() { Command = "USERS", Description = "List users online", Result=cmdUsers } ,
            new MenuOption() { Command = "LOGOUT", Description = "Log Out", Result=cmdLogOut } ,
            new MenuOption() { Command = "QUIT", Description = "Disconnect from server", Result=cmdQuit } ,
            new MenuOption(admin:true) { Command = "SHUTDOWN", Description = "Shut down the server", Result=cmdShutdown } 
       };

        private static BaseTextPrompt cmdShutdown(BaseTextPrompt sender, BaseTelNetState client, string args)
        {
            client.WriteLine("Not implemented.");
            return new MainMenu(client.User.IsAdmin);
        }

        private static BaseTextPrompt cmdUsers(BaseTextPrompt sender, BaseTelNetState client, string args)
        {
            client.Writer.WriteLine("Users Online:");
            var users = NetworkServer.AllUsers.Where(m => m != null).Select(m => m.Username);
            foreach (var user in users)
                client.Writer.WriteLine("  {0}", user);
            client.WriteLine();
            return sender;
        }

        private static BaseTextPrompt cmdLogOut(BaseTextPrompt sender, BaseTelNetState client, string args)
        {
            client.Logout();
            return new LoginPrompt();
        }

        private static BaseTextPrompt cmdQuit(BaseTextPrompt sender, BaseTelNetState client, string args)
        {
            client.Dispose();
            return null;
        }

    }
}