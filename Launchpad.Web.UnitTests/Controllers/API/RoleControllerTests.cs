﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Ploeh.AutoFixture;
using Launchpad.UnitTests.Common;
using Launchpad.Web.Controllers.API;
using System.Web.Http;
using Launchpad.Services.Interfaces;
using Launchpad.Models;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Net;

namespace Launchpad.Web.UnitTests.Controllers.API
{
    public class RoleControllerTests : BaseUnitTest
    {
        private RoleController _roleController;
        private Mock<IRoleService> _mockRoleService;

        public RoleControllerTests()
        {
            _mockRoleService = Mock.Create<IRoleService>();
            _roleController = new RoleController(_mockRoleService.Object);
            _roleController.Request = new System.Net.Http.HttpRequestMessage();
            _roleController.Configuration = new HttpConfiguration();
        }

        [Fact]
        public async void CreateRole_Should_Call_RoleService_And_Return_Created()
        {
            var model = new RoleModel
            {
                Name = "Great Role"
            };

            _mockRoleService.Setup(_ => _.CreateRoleAsync(model))
                .ReturnsAsync(IdentityResult.Success);

            //ACT
            var result = await _roleController.CreateRole(model);

            var message = await result.ExecuteAsync(new CancellationToken());

            message.StatusCode.Should().Be(HttpStatusCode.Created);
        }


        [Fact]
        public async void CreateRole_Should_Call_RoleService_And_Return_BadRequest_On_Failure()
        {
            var model = new RoleModel
            {
                Name = "Great Role"
            };

            _mockRoleService.Setup(_ => _.CreateRoleAsync(model))
                .ReturnsAsync(new IdentityResult());

            //ACT
            var result = await _roleController.CreateRole(model);

            var message = await result.ExecuteAsync(new CancellationToken());

            message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Ctor_Should_Throw_ArgumentNullException_When_RoleService_Is_Null()
        {
            Action throwAction = () => new RoleController(null);

            throwAction.ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("roleService");
        }

        [Fact]
        public void Class_Should_Have_Authorize_Attribute()
        {
            typeof(RoleController)
                .Should()
                .BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void Class_Should_Have_RoutePrefix_Attribute()
        {
            typeof(RoleController)
                .Should()
                .BeDecoratedWith<RoutePrefixAttribute>(value => value.Prefix == Constants.RoutePrefixes.Role);
        }

        [Fact]
        public void CreateRole_Should_Have_HttpPost_Attribute()
        {
            ReflectionHelper.GetMethod<RoleController>(_ => _.CreateRole(new Models.RoleModel()))
                .Should().BeDecoratedWith<HttpPostAttribute>();
        }

    }
}
