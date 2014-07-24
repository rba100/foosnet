using System;
using System.Web.Helpers;
using FoosNet.Utils;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace FoosNet.Network
{
    class ReconnectingWebSocket : Disposable
    {
        private readonly string m_Uri;
        private WebSocket m_WebSocket;
        private bool m_ShouldBeOpen;

        public event Action Connect;
        public event Action<Exception> Error;
        public event Action<string> MessageReceived;

        public ReconnectingWebSocket(string uri)
        {
            m_Uri = uri;
            m_ShouldBeOpen = false;
            CreateWebSocket(uri);
        }

        private void CreateWebSocket(string uri)
        {
            m_WebSocket = new WebSocket(uri);
            m_WebSocket.Opened += OnOpened;
            m_WebSocket.MessageReceived += OnMessageReceieved;
            m_WebSocket.Error += OnError;
            m_WebSocket.Closed += OnClosed;
        }

        private void OnOpened(object sender, EventArgs args)
        {
            if (Connect != null) Connect();
        }

        private void OnMessageReceieved(object sender, MessageReceivedEventArgs args)
        {
            if (MessageReceived != null) MessageReceived(args.Message);
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            if (Error != null) Error(args.Exception);
        }

        private void OnClosed(object sender, EventArgs args)
        {
            CreateWebSocket(m_Uri);
            if (m_ShouldBeOpen)
            {
                m_WebSocket.Open();
            }
        }

        public void Open()
        {
            m_ShouldBeOpen = true;
            m_WebSocket.Open();
        }

        public void SendAsJson(object message)
        {
            var json = Json.Encode(message);
            m_WebSocket.Send(json);
        }

        protected override void Dispose(bool disposing)
        {
            m_ShouldBeOpen = false;
            ((IDisposable)m_WebSocket).Dispose();
        }
    }
}
