using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mnf_Portal.APIs.Errors;
using Mnf_Portal.APIs.Helpers;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Infrastructure.Identity;
using Mnf_Portal.Infrastructure.Persistence;
using Mnf_Portal.Infrastructure.Persistence.Repositories;
using Mnf_Portal.Services;

namespace Mnf_Portal.APIs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerDocumentation()
                    .AddDbContextServices(config)
                    .AddRepositories()
                    .AddApiBehavior();

            return services;
        }

        private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        private static IServiceCollection AddDbContextServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MnfDbContext>(options => options.UseSqlServer(connectionString));

            var identityConnection = configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<MnfIdentityDbContext>(
              options => options.UseSqlServer(identityConnection));
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<INewsService, NewsService>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped(typeof(IMnfContextRepo<>), typeof(MnfContextRepo<>));
            services.AddScoped(typeof(IMnfIdentityContextRepo<>), typeof(MnfIdentityContextRepo<>));
            services.AddScoped(typeof(IEmailService), typeof(ContactUsService));

            return services;
        }

        private static IServiceCollection AddApiBehavior(this IServiceCollection services)
        {
            services
    .Configure<ApiBehaviorOptions>(
        option => option.InvalidModelStateResponseFactory =
            actionContext =>
            {
                var errors = actionContext.ModelState
                    .Where(P => P.Value!.Errors.Count > 0)
                    .SelectMany(P => P.Value!.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray();
                var responseError = new ApiValidationErrorResponse { Errors = errors };

                return new BadRequestObjectResult(responseError);
            });

            return services;
        }
    }
}
