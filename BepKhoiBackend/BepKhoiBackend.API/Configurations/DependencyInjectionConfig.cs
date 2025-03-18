using BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract;
using BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.BusinessObject.Abstract.UnitAbstract;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.BusinessObject.Services.MenuService;
using BepKhoiBackend.BusinessObject.Services.ProductCategoryService;
using BepKhoiBackend.BusinessObject.Services.UnitService;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.DataAccess.Abstract.UnitAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.LoginRepository;
using BepKhoiBackend.DataAccess.Repository.ProductCategoryRepository;
using BepKhoiBackend.DataAccess.Repository.UnitRepository;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<bepkhoiContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injection for Services and Repositories
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();

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
