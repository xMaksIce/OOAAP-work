using Microsoft.AspNetCore;
using Spacebattle.Server;

IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
    .UseKestrel(options =>
    {
        options.ListenAnyIP(8080);
        options.AllowSynchronousIO = true;
    })
    .UseStartup<Startup>();

IWebHost app = builder.Build();
app.Run();
