using DragonAcc.Infrastructure;
using DragonAcc.Service;
using DragonAcc.Service.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace DragonAcc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Cấu hình Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()  // Đặt mức độ log tối thiểu là Information
                .WriteTo.Console()  // Ghi log ra console
                .WriteTo.File("logs/api.log", rollingInterval: RollingInterval.Day)  // Ghi log vào file với tên api.log, cuộn lại theo ngày
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

                // Thêm bảo mật API key bằng JWT Bearer
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                // Thêm yêu cầu bảo mật cho tất cả các yêu cầu API
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                options.CustomSchemaIds(type => type.ToString());
            });

            // Cấu hình CORS cho phép tất cả các nguồn (Origin), phương thức và header
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyCorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()  // Cho phép tất cả các nguồn
                        .AllowAnyMethod()  // Cho phép tất cả các phương thức HTTP
                        .AllowAnyHeader()); // Cho phép tất cả các header
            });

            // Thêm các dịch vụ ứng dụng
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddSingleton<DatabaseBackupService>(sp =>
               new DatabaseBackupService(builder.Configuration.GetConnectionString("connection")));
            var app = builder.Build();
            var backupService = app.Services.GetRequiredService<DatabaseBackupService>();
            backupService.BackupDatabase();
            // Đảm bảo ghi log yêu cầu HTTP trong môi trường phát triển
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Sử dụng Static Files
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(@"C:\Host1\public"),
                RequestPath = "/public"  // Chỉ định URL Path cho các file tĩnh
            });

            // Thiết lập bảo mật HTTP và CORS
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
}
