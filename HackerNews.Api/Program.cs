using HackerNews;
using HackerNews.Core;
using HackerNews.Core.Model.Front;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System.Reflection;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var settings = builder.Configuration.GetSection("HackerNewsApi").Get<HackerNewsApiSettings>();
        builder.Services.AddSingleton(settings);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(Program).Assembly.Location.Replace(".dll", ".xml")));
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(StoryResponse).Assembly.Location.Replace(".dll", ".xml")));
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddHostedService<CacheRefresherService>();
        builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>().AddPolicyHandler(GetRetryPolicy());
        builder.Services.AddSingleton<IHackerNewsService, HackerNewsService>();
        
        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}