﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Voyage.Core.Exceptions;
using Voyage.Models;
using Voyage.UnitTests.Common;
using Voyage.Web;
using Voyage.Web.Filters;
using Xunit;

namespace Voyage.UnitTests.Web.Filters
{
    public class ApiExceptionFilterAttributeTests : BaseUnitTest
    {
        [Fact]
        public void Non_ApiException_Type_Exceptions_Should_Not_Set_Response()
        {
            // Arrange
            var apiExceptionFilter = new ApiExceptionFilterAttribute();

            var context = new HttpActionExecutedContext
            {
                Exception = new Exception()
            };

            // Act
            apiExceptionFilter.OnException(context);

            // Assert
            context.Response.Should().BeNull();
        }

        [Fact]
        public void NotFound_ApiException_Should_Set_Correct_NotFound_Response()
        {
            // Arrange
            var apiExceptionFilter = new ApiExceptionFilterAttribute();

            var controllerContext = new HttpControllerContext
            {
                Request = new HttpRequestMessage(),
                RequestContext =
                    new HttpRequestContext
                    {
                        Principal = new ClaimsPrincipal()
                    }
            };

            controllerContext.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var actionContext = new HttpActionContext(
                controllerContext,
                new Mock<HttpActionDescriptor> { CallBase = true }.Object);

            var context = new HttpActionExecutedContext
            {
                Exception = new NotFoundException("Item Not Found"),
                ActionContext = actionContext
            };

            // Act
            apiExceptionFilter.OnException(context);

            // Assert
            var response = context.Response;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var errorModelList = JsonConvert.DeserializeObject<List<ResponseErrorModel>>(response.Content.ReadAsStringAsync().Result);
            errorModelList.Should().HaveCount(1);
            errorModelList.First().ErrorDescription.Should().Be("Item Not Found");
        }
    }
}