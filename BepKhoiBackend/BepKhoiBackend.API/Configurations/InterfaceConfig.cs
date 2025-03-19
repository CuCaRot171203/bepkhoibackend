using Microsoft.Extensions.DependencyInjection;
using BepKhoiBackend.DataAccess.Repository;
using BepKhoiBackend.BusinessObject.Services.LoginService;

using BepKhoiBackend.DataAccess.Repository.LoginRepository;
using BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface;
using BepKhoiBackend.BusinessObject.Services.RoomAreaService;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using BepKhoiBackend.BusinessObject.Services.RoomService;
using BepKhoiBackend.DataAccess.Repository.RoomRepository;

namespace BepKhoiBackend.API.Configurations
{
    public static class InterfaceConfig
    {
        public static void ConfigureDependencies(IServiceCollection services)
        {
            

            // Đăng ký các Service và Repository
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserRepository, UserRepository>();

            // RoomArea
            services.AddScoped<IRoomAreaRepository, RoomAreaRepository>();
            services.AddScoped<IRoomAreaService, RoomAreaService>();

            //room 
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomService, RoomService>();
        }
    }
}
