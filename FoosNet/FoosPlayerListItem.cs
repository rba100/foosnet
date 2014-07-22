﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Network;

namespace FoosNet
{
    public enum GameState { None, Pending, Accepted, Declined }

    public class FoosPlayerListItem : IFoosPlayer
    {
        private Status m_Status;
        private string m_DisplayName;
        public string Email { get; set; }

        public string DisplayName
        {
            get { return m_DisplayName; }
            set
            {
                if (value == m_DisplayName) return;
                m_DisplayName = value;
                OnPropertyChanged();
            }
        }

        public Status Status
        {
            get { return m_Status; }
            set
            {
                if (value == m_Status) return;
                m_Status = value;
                OnPropertyChanged();
            }
        }

        public int Priority { get; set; }

        public GameState GameState { get; set; }

        public FoosPlayerListItem(string email, Status status, int priority)
        {
            Email = email;
            DisplayName = email;
            Status = status;
            if (priority < 1 || priority > 99)
            {
                throw new Exception("Player priority invalid, has to be between 1 and 99");
            }

            Priority = priority;
        }

        public FoosPlayerListItem(IFoosPlayer player)
        {
            Email = player.Email;
            DisplayName = player.DisplayName;
            Status = player.Status;
            Priority = 50;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}