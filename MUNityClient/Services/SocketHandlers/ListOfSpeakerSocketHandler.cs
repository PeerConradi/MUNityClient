using Microsoft.AspNetCore.SignalR.Client;
using MUNity.Models.ListOfSpeakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Services.SocketHandlers
{
    public class ListOfSpeakerSocketHandler : MUNity.Hubs.ITypedListOfSpeakerHub
    {
        public delegate void OnSpeakerlistChanged(ListOfSpeakers newList);
        public event OnSpeakerlistChanged ListOfSpeakersChanged;

        private ListOfSpeakers _listOfSpeakers;

        public HubConnection HubConnection { get; set; }

        private ListOfSpeakerSocketHandler(ListOfSpeakers listOfSpeakers)
        {
            _listOfSpeakers = listOfSpeakers;

            HubConnection = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/slsocket").Build();
            HubConnection.On<int>(nameof(QuestionTimerStarted), (seconds) => QuestionTimerStarted(seconds));
            HubConnection.On<ListOfSpeakers>(nameof(SpeakerListChanged), (list) => SpeakerListChanged(list));
            HubConnection.On<int>(nameof(SpeakerTimerStarted), (seconds) => SpeakerTimerStarted(seconds));
            HubConnection.On<string>(nameof(TimerStopped),(s) => TimerStopped());
        }

        public static async Task<ListOfSpeakerSocketHandler> CreateHandler(ListOfSpeakers resolution)
        {
            var instance = new ListOfSpeakerSocketHandler(resolution);
            await instance.HubConnection.StartAsync();
            return instance;
        }

        public Task QuestionTimerStarted(int seconds)
        {
            throw new NotImplementedException();
        }

        public Task SpeakerListChanged(ListOfSpeakers listOfSpeaker)
        {
            ListOfSpeakersChanged?.Invoke(listOfSpeaker);
            return null;
        }

        public Task SpeakerTimerStarted(int seconds)
        {
            throw new NotImplementedException();
        }

        public Task TimerStopped()
        {
            throw new NotImplementedException();
        }
    }
}
