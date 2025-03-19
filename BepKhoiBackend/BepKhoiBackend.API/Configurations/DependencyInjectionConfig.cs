using BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract;
using BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.BusinessObject.Abstract.UnitAbstract;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.BusinessObject.Services.MenuService;
using BepKhoiBackend.BusinessObject.Services.ProductCategoryService;
using BepKhoiBackend.BusinessObject.Services.UnitService;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.DataAccess.Abstract.UnitAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.LoginRepository;
using BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.ProductCategoryRepository;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.RoomRepository;
using BepKhoiBackend.DataAccess.Repository.UnitRepository;

using Microsoft.EntityFrameworkCore;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.BusinessObject.Services.RoomAreaService;
using BepKhoiBackend.BusinessObject.Services.RoomService;

namespace BepKhoiBackend.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<bepkhoiContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
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


            //Dependency Injection for Services and Repositories

            // Menu Repositories and Services
           services.AddScoped<IMenuRepository, MenuRepository>(); // Repository
            services.AddScoped<IMenuService, MenuService>();       // Service

            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IUnitService, UnitService>();
        }
    }
}
