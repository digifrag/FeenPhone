﻿using Alienseed.BaseNetworkServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Alienseed.BaseNetworkServer
{
    class ServerHost : IDisposable
    {
        List<INetworkServer> Servers = new List<INetworkServer>();

        public ServerHost()
        {
            InitServers();
            StartServers();
        }

        void InitServers()
        {
            StopServers();
            Servers.Clear();

            Servers.Add(new Network.Telnet.TelnetServer());
            //  Servers.Add(new Network.WebSockets.WSServer());
        }

        void StartServers()
        {
            var failed = new List<INetworkServer>();

            foreach (var server in Servers.Where(m => !m.Running))
            {
                Console.WriteLine("STARTING {0} on port {1}", server, server.Port);
                if (server.Start())
                {
                    Console.WriteLine("Started server: {0} on port {1}", server, server.Port);
                    server.OnListenerCrash += server_OnListenerCrash;
                }
                else
                {
                    Console.WriteLine("FAILED server: {0} on port {1}", server, server.Port);
                    failed.Add(server);
                }
            }

            foreach (var fail in failed)
                Servers.Remove(fail);

        }

        private void StopServers()
        {
            foreach (var server in Servers.Where(m => m.Running))
            {
                Console.WriteLine("Stopping server: {0}", server);
                server.Stop();
            }
        }

        void server_OnListenerCrash(INetworkServer server)
        {
            Console.WriteLine("Server Crash: {0} on Port {1}", server, server.Port);

            server.Stop();
            Servers.Remove(server);
        }

        #region IDisposable Members

        public void Dispose()
        {
            StopServers();
            Servers.Clear();
        }

        #endregion
    }
}


