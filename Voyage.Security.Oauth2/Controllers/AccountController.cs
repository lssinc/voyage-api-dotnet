﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Voyage.Core.Exceptions;
using Voyage.Security.Oauth2.Models;
using Voyage.Services.User;

namespace Voyage.Security.Oauth2.Controllers
{
    public class AccountController : Controller
    {
        public async Task<ActionResult> Login()
        {
            var loginModel = GetModel();
            if (Request.HttpMethod == "POST")
            {
                var isPersistent = !string.IsNullOrEmpty(Request.Form.Get("isPersistent"));

                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Signin")))
                {
                    var context = HttpContext.GetOwinContext().GetAutofacLifetimeScope();
                    var userService = context.Resolve<IUserService>();
                    try
                    {
                        // check if user exits. if not throw exeception
                        var user = await userService.GetUserByNameAsync(Request.Form["username"]);

                        // Check if locked out
                        if (await userService.IsLockedOutAsync(user.Id))
                        {
                            throw new NotFoundException("User Locked Out");
                        }

                        var isValidCredential = await userService.IsValidCredentialAsync(Request.Form["username"], Request.Form["password"]);
                        if (!isValidCredential)
                        {
                            await userService.AccessFailedAsync(user.Id);
                            throw new NotFoundException("Invalid Credentials");
                        }

                        // ResetAccess
                        await userService.ResetAccessFailedCountAsync(user.Id);

                        var authentication = HttpContext.GetOwinContext().Authentication;
                        ClaimsIdentity identity = await userService.CreateClaimsIdentityAsync(Request.Form["username"], OAuthDefaults.AuthenticationType);
                        authentication.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, new ClaimsIdentity(identity.Claims, "Application"));
                    }
                    catch (NotFoundException notFoundException)
                    {
                        loginModel.NotFoundException = notFoundException;                        
                        return View("Login", loginModel);
                    }
                }
            }

            return View("Login", loginModel);
        }
        
        private LoginModel GetModel()
        {
            var loginModel = new LoginModel();
            try
            {
                // Get client return url
                loginModel.ReturnUrl = Server.UrlEncode(Request.QueryString["ReturnUrl"]);

                return loginModel;
            }
            catch (Exception)
            {
                return loginModel;
            }
        }
    }
}