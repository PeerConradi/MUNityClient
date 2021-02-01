﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MUNityClient.Shared.VirtualCommittee.ActiveRoom
{
    public class ActiveRoomLayoutWrapper : INotifyPropertyChanged
    {
        public enum MainContents
        {
            Home,
            Agenda,
            Voting,
            Resolution
        }

        private bool _showMemberlist = true;
        public bool ShowMemberlist
        {
            get => _showMemberlist;
            set
            {
                if (_showMemberlist == value) return;
                _showMemberlist = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showListOfSpeakers = true;
        public bool ShowListOfSpeakers 
        {
            get => _showListOfSpeakers;
            set
            {
                if (_showListOfSpeakers == value) return;
                _showListOfSpeakers = value;
                NotifyPropertyChanged();
            }
        }

        private MainContents _mainContent = MainContents.Home;
        public MainContents MainContent
        {
            get => _mainContent;
            set
            {
                if (_mainContent == value) return;
                _mainContent = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


    }
}
