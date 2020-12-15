using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Models.ListOfSpeakers;
using Microsoft.JSInterop;

namespace MUNityClient.Services
{
    public class ListOfSpeakerService
    {
        public delegate void OnStorageChanged();

        public event OnStorageChanged StorageChanged;

        private readonly HttpService _httpService;

        private readonly ILocalStorageService _localStorage;

        public async Task<ListOfSpeakers> CreateListOfSpeakers()
        {
            var listOfSpeakers = new ListOfSpeakers();
            await StoreListOfSpeakers(listOfSpeakers);
            return listOfSpeakers;
        }

        public async Task<ListOfSpeakers> GetListOfSpeakers(string id)
        {
            return await this._localStorage.GetItemAsync<ListOfSpeakers>(ListOfSpeakerIdInStorage(id));
        }

        public async Task StoreListOfSpeakers(ListOfSpeakers list)
        {
            await this._localStorage.SetItemAsync(ListOfSpeakerIdInStorage(list.ListOfSpeakersId), list);
        }

        private string ListOfSpeakerIdInStorage(string id) => "mtlos_" + id;

        [JSInvokable]
        public Task StorageHasChanged()
        {
            this.StorageChanged?.Invoke();
            return Task.FromResult("");
        }

        public ListOfSpeakerService(HttpService httpService, ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            this._httpService = httpService;
            this._localStorage = localStorage;
            jsRuntime.InvokeVoidAsync("registerStorageListener", DotNetObjectReference.Create(this));
        }
    }
}
