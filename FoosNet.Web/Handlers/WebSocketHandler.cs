﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using FoosNet.Vision;
using Microsoft.Web.WebSockets;

namespace FoosNet.Web.Handlers
{
    public class SocketHandler : WebSocketHandler, IDisposable
    {
        private static readonly ConcurrentDictionary<string, SocketHandler> s_Subscribers = new ConcurrentDictionary<string, SocketHandler>();
        private static readonly TableWatcher s_TableWatcher = new TableWatcher();
        private string m_Email = null;

        static SocketHandler()
        {
            s_TableWatcher.TableNowBusy += TableStatusChanged;
            s_TableWatcher.TableNowFree += TableStatusChanged;
        }

        private static void TableStatusChanged(object sender, EventArgs eventArgs)
        {
            BroadcastAll(new {type = "tablestatus", tablestatus = s_TableWatcher.TableUsage.ToString()});
        }

        public override void OnMessage(string message)
        {
            var m = Json.Decode(message);
            var action = m.action as string;
            switch (action)
            {
                case "subscribe":
                    Subscribe(m.email);
                    break;
                case "challenge":
                    Challenge(m.toChallenge);
                    break;
                case "respond":
                    Respond(m.challenger, m.response);
                    break;
                case "gametime":
                    Gametime(((DynamicJsonArray)m.players).Cast<string>().ToArray());
                    break;
                case "cancelgame":
                    CancelGame(((DynamicJsonArray)m.players).Cast<string>().ToArray());
                    break;
                case "players":
                    Players();
                    break;
                case "tablestatus":
                    TableStatus();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown action \"{0}\"", action));
            }
        }

        private void TableStatus()
        {
            Send(new { type = "tablestatus", tablestatus = s_TableWatcher.TableUsage.ToString() });
        }

        public override void OnClose()
        {
            Dispose();
            base.OnClose();
        }

        private void Subscribe(string email)
        {
            if (m_Email != null && m_Email != email) throw new InvalidOperationException("Cannot change email address during a connection");
            m_Email = email;
            s_Subscribers[email] = this;
            Broadcast(new { type = "player", player = email });
        }

        private void Challenge(string toChallenge)
        {
            SendTo(toChallenge, new { type = "challenge", challengedBy = m_Email });
        }

        private void Respond(string challengedBy, string response)
        {
            SendTo(challengedBy, new { type = "response", player = m_Email, response = response });
        }

        private void Gametime(string[] otherPlayers)
        {
            var allPlayers = new string[otherPlayers.Length + 1];
            allPlayers[0] = m_Email;
            otherPlayers.CopyTo(allPlayers, 1);
            foreach (var player in otherPlayers)
            {
                SendTo(player, new { type = "gametime", players = allPlayers });
            }
        }

        private static void CancelGame(IEnumerable<string> otherPlayers)
        {
            foreach (var player in otherPlayers)
            {
                SendTo(player, new { type = "cancelgame" });
            }
        }

        private void Players()
        {
            Send(new { type = "players", players = s_Subscribers.Keys.Where(e => e != m_Email).ToArray() });
        }

        private void Broadcast(object message)
        {
            var sendTo = s_Subscribers.Values.Where(s => s != this).ToList();
            BroadcastTo(message, sendTo);
        }

        private static void BroadcastAll(object message)
        {
            var sendTo = s_Subscribers.Values.ToList();
            BroadcastTo(message, sendTo);
        }

        private static void BroadcastTo(object message, IEnumerable<SocketHandler> sendTo)
        {
            foreach (var recipient in sendTo)
            {
                recipient.Send(message);
            }
        }

        private static void SendTo(string to, object message)
        {
            s_Subscribers[to].Send(message);
        }

        private void Send(object message)
        {
            base.Send(Json.Encode(message));
        }

        public void Dispose()
        {
            SocketHandler removed;
            s_Subscribers.TryRemove(m_Email, out removed);
            GC.SuppressFinalize(this);
        }

        ~SocketHandler()
        {
            Dispose();
        }
    }
}
