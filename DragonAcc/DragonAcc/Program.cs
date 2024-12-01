using DragonAcc.Infrastructure;
using DragonAcc.Service.Services;
using DragonAcc.Service;
using Microsoft.OpenApi.Models;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        // Cấu hình Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/api.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        // Xóa các provider logging mặc định và thay thế bằng Serilog
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        // Thêm các dịch vụ cần thiết
        builder.Services.AddControllers();

        // Cấu hình Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DRAGONACC 2024 WEB API",
                Version = "v1",
            });
        });

        // Cấu hình CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyCorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Thêm các dịch vụ ứng dụng
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        // Đăng ký dịch vụ sao lưu cơ sở dữ liệu
        builder.Services.AddSingleton<DatabaseBackupService>(sp =>
            new DatabaseBackupService(builder.Configuration.GetConnectionString("connection")));

        var app = builder.Build();

        // Lấy dịch vụ sao lưu và thực thi sao lưu
        var backupService = app.Services.GetRequiredService<DatabaseBackupService>();
        backupService.BackupDatabase(); // Gọi sao lưu cơ sở dữ liệu khi API khởi động

        // Đảm bảo ghi log yêu cầu HTTP trong môi trường phát triển
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Sử dụng Static Files
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAnyCorsPolicy");
        app.UseAuthorization();

        // Định tuyến các controller
        app.MapControllers();

        // Chạy ứng dụng
        app.Run();

        // Đảm bảo giải phóng tài nguyên của Serilog khi ứng dụng kết thúc
        Log.CloseAndFlush();
    }
}
