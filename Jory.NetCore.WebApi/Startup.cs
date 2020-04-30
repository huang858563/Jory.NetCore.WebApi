using Autofac;
using Jory.NetCore.Core.Enums;
using Jory.NetCore.Core.Interfaces;
using Jory.NetCore.Model.Data;
using Jory.NetCore.WebApi.Common;
using Jory.NetCore.WebApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;

namespace Jory.NetCore.WebApi
{
    public class Startup
    {
        //private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtBearerOption>(Configuration.GetSection("JwtBearer"));
            var jwtBearerOption = Configuration.GetSection("JwtBearer").Get<JwtBearerOption>();

            services.AddControllers(options =>
            {
                if (jwtBearerOption.Enabled)
                {
                    options.Filters.Add(new AuthorizeFilter());
                }

                //options.ReturnHttpNotAcceptable = true;
                //options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).AddXmlDataContractSerializerFormatters();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<JoryNetCoreDbContext>(options =>
            {
                if (bool.Parse(Configuration["EFCoreLogEnabled"]))
                {
                    options.UseLoggerFactory(new LoggerFactory(new[] {new EfCoreLoggerProvider()}));
                }

                options.UseSqlite("Data Source=JoryDB.db", p => p.MigrationsAssembly("Jory.NetCore.Model"));
            });

            //services.AddScoped<IJwtTokenValidationService, JwtTokenValidationService>();
           // services.AddScoped<IBaseRep, Bas>();
            //services.AddScoped<IUserRep, UserRep>(x=>new UserRep(x.GetService<JoryNetCoreDbContext>(), DbCategory.Sqlite ));

            services.AddDistributedMemoryCache();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                    });

                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });

            });

          
            if (jwtBearerOption.Enabled)
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Audience = jwtBearerOption.Audience;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // The signing key must match!
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearerOption.SecurityKey)),

                            // Validate the JWT Issuer (iss) claim
                            ValidateIssuer = true,
                            ValidIssuer = jwtBearerOption.Issuer,

                            // Validate the JWT Audience (aud) claim
                            ValidateAudience = true,
                            ValidAudience = jwtBearerOption.Audience,

                            // Validate the token expiry
                            ValidateLifetime = true,

                            // If you want to allow a certain amount of clock drift, set that here
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                        Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
                });
                //services.AddAuthorization();
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1", //版本 
                    Title = "Jory.NetCore.WebApi 接口文档-NetCore3.1", //标题
                    Description = "Jory.NetCore.WebApi Http API v1", //描述
                    Contact = new OpenApiContact { Name = "jory", Email = "", Url = new Uri("https://www.jory.top") }
                    //License = new OpenApiLicense { Name = "jory", Url = new Uri("http://jory.cnblogs.com") }
                });
                if (jwtBearerOption.Enabled)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        BearerFormat = "JWT",
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] { }
                        }
                    });
                }

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "Jory.NetCore.WebApi.xml"); //这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true); //默认的第二个参数是false,对方法的注释


                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Jory.NetCore.Model.xml"));


                //#region Jwt
                ////开启权限小锁
                //c.OperationFilter<AddResponseHeadersFilter>();
                //c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                ////在header中添加token，传递到后台
                //c.OperationFilter<SecurityRequirementsOperationFilter>();
                //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //{
                //    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) \"",
                //    Name = "Authorization",//jwt默认的参数名称
                //    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                //    Type = SecuritySchemeType.ApiKey
                //});


                //#endregion
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
                {
                    //NLogHelper.Logger.Error(context.Response);
                    //_logger.Info("error");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Unexpected Error");
                }));
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"Jory.NetCore.WebApi v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<JwtTokenValidationService>().As<IJwtTokenValidationService>();

            var repository = Assembly.Load("Jory.NetCore.Repository");
            builder.RegisterAssemblyTypes(repository).Where(t =>
                    typeof(IRepository).IsAssignableFrom(t)
                    && t != typeof(IRepository)
                    && !t.IsAbstract).AsImplementedInterfaces()
                .WithParameter((pi, c) => pi.Name == "context", (pi, c) => c.Resolve<JoryNetCoreDbContext>())
                .WithParameter("dbCategory", DbCategory.Sqlite); //.InstancePerDependency();
        }
    }
}
