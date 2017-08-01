﻿using System;
using System.IdentityModel.Claims;
using Voyage.Core;
using Voyage.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Security;
using Voyage.Services.User;
using Voyage.Services.Verification;

namespace Voyage.Api.API.V1
{
    [RoutePrefix(Constants.RoutePrefixes.V1)]
    public class AccountController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IVerificationService _verificationService;
        private readonly IAuthenticationManager _authenticationManager;

        public AccountController(IUserService userService, IVerificationService verificationService, IAuthenticationManager authenticationManager)
        {
            _userService = userService.ThrowIfNull(nameof(userService));
            _verificationService = verificationService.ThrowIfNull(nameof(verificationService));
            _authenticationManager = authenticationManager.ThrowIfNull(nameof(authenticationManager));
        }

        /**
        * @api {post} /v1/profile Create profile
        * @apiVersion 1.0.0
        * @apiName CreateProfile
        * @apiGroup Profile
        *
        * @apiPermission none
        * @apiSampleRequest http://qa-api-ms.voyageframework.com/api/v1/account/register
        * @apiParam {String} email User's email
        * @apiParam {String} password User's password
        * @apiParam {String} confirmPassword User's password (x2)
        * @apiParam {String} firstName First name
        * @apiParam {String} lastName Last name
        * @apiParam {Object[]} users.phones User phone numbers
        * @apiParam {String} users.phones.phoneNumber Phone number
        * @apiParam {String} users.phones.phoneType Phone type
        *
        * @apiHeader (Response Headers) {String} location Location of the newly created resource
        *
        * @apiHeaderExample {json} Location-Example
        * {
        *    "Location": "http://voyageframework.com/api/v1/users/b78ae241-1fa6-498c-aa48-9742245d0d2f"
        * }
        * 
        * @apiSuccessExample Success-Response:        
        * 
        * HTTP/1.1 201 Created
        * {
        *    "id": "f9d69894-7908-4606-918e-410dca8c3238",
        *    "firstName": "FirstName",
        *    "lastName": "LastName",
        *    "username": "FirstName3@app.com",
        *    "email": "FirstName3@app.com",
        *    "phones": [
        *        {
        *            "id": 3,
        *            "userId": "f9d69894-7908-4606-918e-410dca8c3238",
        *            "phoneNumber": "5555551212",
        *            "phoneType": "Mobile"
        *        }
        *    ],
        *    "isActive": true
        * }
        *
        * @apiUse BadRequestError
        */
        [Route("profile")]
        public async Task<IHttpActionResult> Register(RegistrationModel model)
        {
            try
            {
                var result = await _userService.RegisterAsync(model);                
                return CreatedAtRoute("GetUserAsync", new { userId = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // TODO: Add documentation
        [Route("verify/send")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> SendVerificationCode()
        {
            var userId = _authenticationManager.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            await _verificationService.SendCode(userId);
            return Ok();
        }

        // TODO: Add documentation
        [Route("verify")]
        [HttpPost]
        public async Task<IHttpActionResult> VerifyCode(VerifyModel model)
        {
            var userId = _authenticationManager.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            await _verificationService.VerifyCodeAsync(userId, model.Code);
            return Ok();
        }
    }
}
