using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace DotnetVideoHistory.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureCors();
            //services.AddDbContext<HistoryContext>(c => c.UseSqlServer(configuration.GetConnectionString("MainConnection")));
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "ToDo API",
            //        Description = "A simple example ASP.NET Core Web API",
            //        TermsOfService = new Uri("https://example.com/terms"),
            //    });
            //});
            services.ConfigureDependencies();
            services.AddHostedService<RabbitReceiver>();
        }

        //public static void ConfigureAuth(this IServiceCollection services, JwtConfig jwtConfig)
        //{
        //    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SecretKey));
        //    services.Configure<JwtIssuerOptions>(options =>
        //    {
        //        options.Audience = jwtConfig.Audience;
        //        options.InternalIssuer = jwtConfig.InternalIssuer;
        //        options.PortalIssuer = jwtConfig.PortalIssuer;
        //        options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        //    });
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidIssuers = new[] { jwtConfig.InternalIssuer, jwtConfig.PortalIssuer },

        //        ValidateAudience = true,
        //        ValidAudience = jwtConfig.Audience,

        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = signingKey,

        //        RequireExpirationTime = false,
        //        ValidateLifetime = true,
        //        ClockSkew = TimeSpan.Zero
        //    };
        //    services.AddAuthentication(option =>
        //    {
        //        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(configureOptions =>
        //    {
        //        configureOptions.TokenValidationParameters = tokenValidationParameters;
        //        configureOptions.ClaimsIssuer = jwtConfig.InternalIssuer;
        //        configureOptions.SaveToken = true;
        //        configureOptions.Events=new JwtBearerEvents
        //        {
        //            OnAuthenticationFailed = context =>
        //            {
        //                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
        //                {
        //                    context.Response.Headers.Add("Token-Expired", "true");
        //                }
        //                return Task.CompletedTask;
        //            }
        //        }
        //    })
        //}

        public static void ConfigureDependencies(this IServiceCollection services)
        {
            //services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddTransient<IRabbitService, RabbitService>();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }


    }
}