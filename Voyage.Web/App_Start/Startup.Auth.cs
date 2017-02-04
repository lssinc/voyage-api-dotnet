﻿using Autofac;
using FluentValidation.WebApi;
using Voyage.Web.AuthProviders;
using Voyage.Web.Middleware;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Voyage.Web.Auth_Stuff;

namespace Voyage.Web
{
    public partial class Startup
    {
        public void Configure(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();

            // Build the container
            ContainerConfig.Register(httpConfig);

            // Configure API
            WebApiConfig.Register(httpConfig);

            // configure FluentValidation model validator provider
            FluentValidationModelValidatorProvider.Configure(httpConfig);

            // 1. Use the autofac scope for owin
            app.UseAutofacLifetimeScopeInjector(ContainerConfig.Container);

            // 2. Allow cors requests
            app.UseCors(CorsOptions.AllowAll);

            // 3. Use the readable response middleware
            app.Use<RewindResponseMiddleware>();

            // 4. Register the activty auditing here so that anonymous activity is captured
            app.UseMiddlewareFromContainer<ActivityAuditMiddleware>();

            // 5. Configure OAuth.
            // Enable Application Sign In Cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Application",
                AuthenticationMode = AuthenticationMode.Passive,
                LoginPath = new PathString(Paths.LoginPath),
                LogoutPath = new PathString(Paths.LogoutPath),
            });

            // Enable External Sign In Cookie
            app.SetDefaultSignInAsAuthenticationType("External");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "External",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "External",
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
            });

            // Setup Authorization Server
            var oauthProvider = ContainerConfig.Container.Resolve<ApplicationOAuthProvider>();
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString(Paths.AuthorizePath),
                TokenEndpointPath = new PathString(Paths.TokenPath),
                ApplicationCanDisplayErrors = true,

                // If the config is wrong, let the application crash
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["oAuth:TokenExpireSeconds"])),

                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = bool.Parse(ConfigurationManager.AppSettings["oAuth:AllowInsecureHttp"]),

                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = oauthProvider

                // Authorization code provider which creates and receives authorization code
                // AuthorizationCodeProvider = new AuthenticationTokenProvider
                // {
                //    OnCreate = CreateAuthenticationCode,
                //    OnReceive = ReceiveAuthenticationCode,
                // },

                //// Refresh token provider which creates and receives referesh token
                // RefreshTokenProvider = new AuthenticationTokenProvider
                // {
                //    OnCreate = CreateRefreshToken,
                //    OnReceive = ReceiveRefreshToken,
                // }
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // 6. Add web api to pipeline
            app.UseWebApi(httpConfig);
        }
    }
}