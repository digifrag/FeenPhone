﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alienseed.BaseNetworkServer.Network.Telnet.Prompts
{
    class LoginPrompt : TextPrompt
    {
        static string[] prompt = new string[] { "Login> " };
        
        protected override string[] QuestionText
        {
            get { return prompt; }
        }

        public override BaseTextPrompt OnResponse(TelNetState client, string username, bool cancel)
        {
            if (!cancel && !string.IsNullOrWhiteSpace(username))
            return new LoginPrompt2(username.Trim());
            return this;
        }
    }

    class LoginPrompt2 : TextPrompt
    {
        public override char? EchoChar { get { return '*'; } }

        string Username;
        public LoginPrompt2(string username)
        {
            Username = username;
        }

        static string[] prompt = new string[] { "Password> " };

        protected override string[] QuestionText
        {
            get { return prompt; }
        }

        public override BaseTextPrompt OnResponse(TelNetState client, string password, bool cancel)
        {
            if (!cancel && client.Login(Username, password.Trim()))
                return new MainMenu(client.User.IsAdmin);
            else
                return new LoginPrompt();
        }
    }

}