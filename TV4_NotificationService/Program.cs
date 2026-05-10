using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NotificationService.Services;
var builder = WebApplication.CreateBuilder(args);

// 🔥 THÊM ĐOẠN NÀY
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5167, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));

builder.Services.AddGrpc();

var app = builder.Build();

// ĐĂNG KÝ HƯỚNG DẪN ĐƯỜNG ĐI CHO gRPC TỚI ĐÚNG CLASS
app.MapGrpcService<MyNotificationService>(); 

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.Run();