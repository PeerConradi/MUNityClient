using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ganss.XSS;

namespace MUNityClient
{
    public class Program
    {
        public static readonly string API_URL = "https://localhost:44349";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            // builder.HostEnvironment.BaseAddress
            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(API_URL) });
            builder.Services.AddSingleton<Services.ResolutionService>();
            builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(x =>
            {
                // Configure sanitizer rules as needed here.
                // For now, just use default rules + allow class attributes
                var sanitizer = new Ganss.XSS.HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                return sanitizer;
            });

            await builder.Build().RunAsync();
        }
    }
}
