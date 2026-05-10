using Grpc.Core;
using NotificationService.Protos;

namespace NotificationService.Services;

public class MyNotificationService : Notification.NotificationBase
{
    // HÀM 1: NHẬN THÔNG BÁO TỪ STUDENT VÀ LƯU VÀO KHO
    public override Task<LogReply> SendLog(LogRequest request, ServerCallContext context)
    {
        // Lấy giờ hiện tại cho nó ngầu
        var time = DateTime.Now.ToString("HH:mm:ss");
        var logMessage = $"[{time}] {request.Message}";
        
        // Vẫn in ra màn hình console giống cũ
        Console.WriteLine($"[CÓ BIẾN]: {request.Message}");
        
        // ⚠️ DÒNG QUAN TRỌNG NHẤT: Phải nhét vào kho LogStore thì Streaming mới có data để lấy!
        LogStore.Logs.Insert(0, logMessage);
        
        return Task.FromResult(new LogReply { Success = true });
    }

    // HÀM 2: MỞ VÒI XẢ STREAMING VỀ CHO STUDENT
    public override async Task DownloadLogStream(StreamRequest request, IServerStreamWriter<LogMessage> responseStream, ServerCallContext context)
    {
        // Lấy danh sách log đang có trong kho (tối đa bằng con số MaxItems mà Student xin)
        var logsToSend = LogStore.Logs.Take(request.MaxItems).ToList();

        // Xả từ từ từng dòng một về
        foreach (var log in logsToSend)
        {
            await responseStream.WriteAsync(new LogMessage { Content = log });
            await Task.Delay(1000); // Cố tình delay 1s để thấy hiệu ứng nước chảy
        }
    }
}