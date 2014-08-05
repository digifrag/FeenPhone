﻿using Alienseed.BaseNetworkServer.Accounting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Alienseed.BaseNetworkServer.Network
{

    public abstract class TCPNetState<TReader, TWriter> : NetState, IDisposable
        where TReader : BaseStreamReader, new()
        where TWriter : BaseStreamWriter, new()
    {
        public TReader Reader { get; private set; }
        public TWriter Writer { get; private set; }

        private Stream Stream { get; set; }

        internal TCPNetState(Stream stream, EndPoint ep)
            : base(ep)
        {
            Stream = stream;

            Reader = new TReader(); Reader.SetStream(stream);
            Writer = new TWriter(); Writer.SetStream(stream);

            Reader.OnDisconnect += Dispose;

            OnConnected();
        }

        protected abstract void OnConnected();

        public override void Dispose()
        {
            base.Dispose();

            Reader.Dispose();
            Writer.Dispose();
            Stream.Dispose();
        }
    }

    public abstract class NetState : INetState
    {
        public EndPoint EndPoint { get; private set; }

        public virtual bool Connected { get { return !isDisposed; } }

        internal NetState(EndPoint ep)
        {
            EndPoint = ep;

            AddClient(this);
            LogLine("Connected");
        }

        public static IEnumerable<INetState> AllClients { get { return _clients; } }

        private static readonly List<INetState> _clients = new List<INetState>();
        protected static void AddClient(INetState client) { _clients.Add(client); }
        protected static void RemoveClient(INetState client) { _clients.Remove(client); }

        #region INetState Members

        private IUserClient _User = null;
        public IUserClient User
        {
            get { return _User; }
            private set
            {
                if (_User != value)
                {
                    if (_User != null)
                        _User.SetClient(null);
                    if (value != null && value.SetClient(this))
                        _User = value;
                    else
                        _User = null;
                }
            }
        }


        #endregion

        public abstract void Write(byte[] bytes);

        protected bool LoginSetUser(IUserClient user)
        {
            if (User != null)
            {
                if (user != null)
                    return false;
            }
            else
            {
                if (NetworkServer.AllUsers.Contains(user))
                {
                    LogLine("Login Rejected (online)");
                    return false;
                }
            }
            User = user;
            return true;
        }

        public void Logout()
        {
            LogLine("Logout");
            User = null;
        }

        public void LogLine(string format, object arg0)
        {
            LogLine(string.Format(format, arg0));
        }

        protected abstract string LogTitle { get; }
        protected abstract string ClientIdentifier { get; }

        public void LogLine(string format, params object[] args)
        {
            LogLine(string.Format(format, args));
        }

        public void LogLine(string text)
        {
            Console.Write("{0}: [{1}] ", LogTitle, ClientIdentifier);
            Console.WriteLine(text);
        }

        #region IClient Members

        //public virtual void OnGameTick(CardGames.IGameInternal game) { }
        //public virtual void OnTableStatusChange(CardGames.IGameInternal game, string newstatus) { }
        //public virtual void OnSit(CardGames.IGameInternal game, CardGames.ITableInternal table, CardGames.ISeatInternal seat, IUser user) { }
        //public virtual void OnLeave(CardGames.IGameInternal game, CardGames.ITableInternal table, CardGames.ISeatInternal seat, IUser user) { }
        //public virtual void OnBetPrompt(CardGames.IGameInternal game, CardGames.ITableInternal table, CardGames.ISeatInternal seat, CardGames.BetRequest prompt) { }
        //public virtual void OnBet(CardGames.IGameInternal game, CardGames.ITableInternal table, CardGames.ISeatInternal pokerSeat, int bet) { }
        //public virtual void OnFold(CardGames.IGameInternal game, CardGames.ITableInternal table, CardGames.ISeatInternal seat) { }
        //public virtual void OnDealtCard(CardGames.IGameInternal game, CardGames.ISeatInternal seat, ICard card) { }
        //public virtual void OnGameSequenceAdvance(CardGames.IGameInternal game) { }

        //public virtual void OnReset(IGame game) { }

        //public virtual void OnCheck(CardGames.ISeatInternal seat) { }
        //public virtual void OnShowCards(CardGames.IGameInternal game, CardGames.ISeatInternal seat, ICardSet cards) { }
        //public virtual void OnWinners(CardGames.IGameInternal game, IEnumerable<CardGames.ISeatInternal> seats) { }
        //public virtual void OnPotRefundedTo(CardGames.IGameInternal game, IBettingSeat seat, int amount) { }
        //public virtual void OnPotPaidTo(CardGames.IGameInternal game, IBettingSeat seat, int amount) { }

        //public virtual void OnHandChanged(CardGames.IGameInternal game, CardGames.ISeatInternal seat) { }
        #endregion

        bool isDisposed = false;
        public virtual void Dispose()
        {
            LogLine("Disconnected.");
            User = null;
            RemoveClient(this);

            isDisposed = true;
        }
    }

}