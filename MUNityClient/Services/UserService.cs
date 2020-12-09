using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;

namespace MUNityClient.Services
{
    public class UserService
    {
        public delegate void OnLoggedIn(Models.User.AuthenticationResponse user);

        public event OnLoggedIn UserLoggedIn;

        private readonly HttpService _httpService;

        private Models.User.UserInformation loggedInUser;

        public Models.User.UserInformation CurrentUser => loggedInUser;

        public async Task<Models.User.UserInformation> GetMyself()
        {
            var token = await _httpService.GetStoredToken();
            if (string.IsNullOrEmpty(token)) return null;
            var httpClient = await this._httpService.WithToken();
            var user = await httpClient.GetFromJsonAsync<Models.User.UserInformation>($"/api/User/WhoAmI");
            return user;
        }

        public async Task<Models.User.AuthenticationResponse> Login(Models.User.AuthenticateRequest request)
        {
            var content = JsonContent.Create(request);
            var response = await this._httpService.HttpClient.PostAsync($"/api/User/Login", content);
            if (!response.IsSuccessStatusCode) return null;

            var user = await response.Content.ReadFromJsonAsync<Models.User.AuthenticationResponse>();
            if (user != null)
            {
                await this._httpService.SetToken(user.Token);
                this.UserLoggedIn?.Invoke(user);
                return user;
            }
            return null;
        }

        public async Task<HttpResponseMessage> Register(Models.User.RegisterRequest request)
        {
            var body = JsonContent.Create(request);
            return await this._httpService.HttpClient.PostAsync($"/api/User/Register", body);
        }

        public async Task Logout()
        {
            await this._httpService.RemoveToken();
            this.loggedInUser = null;
        }

        public UserService(HttpService httpService)
        {
            this._httpService = httpService;
        }
    }
}
