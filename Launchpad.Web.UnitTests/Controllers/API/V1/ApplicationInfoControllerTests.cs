﻿using FluentAssertions;
using Launchpad.Services.Interfaces;
using Launchpad.UnitTests.Common;
using Launchpad.Web.Controllers.API.V1;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using Xunit;
using Launchpad.Models;
using System.Net;
using System.Net.Http;
using Launchpad.Models.Enum;
using System.Web.Http;
using System.Web.Http.Results;
using System.Threading;
using System.Configuration;
using System.IO;

namespace Launchpad.Web.UnitTests.Controllers.API.V1
{
    public class ApplicationInfoControllerTests : BaseUnitTest
    {
        private ApplicationInfoController _controller;
        private Mock<IApplicationInfoService> _mockApplicationInfoService;
        private Mock<IConfigurationManagerService> _mockConfigurationManagerService;
        private Mock<IFileReaderService> _mockFileReaderService;
        private Mock<IPathProviderService> _mockPathProviderService;


        public ApplicationInfoControllerTests()
        {
            _mockApplicationInfoService = Mock.Create<IApplicationInfoService>();
            _mockConfigurationManagerService = Mock.Create<IConfigurationManagerService>();
            _mockFileReaderService = Mock.Create<IFileReaderService>();
            _mockPathProviderService = Mock.Create<IPathProviderService>();

            _controller = new ApplicationInfoController(_mockApplicationInfoService.Object, _mockConfigurationManagerService.Object, _mockFileReaderService.Object,
               _mockPathProviderService.Object);
            _controller.Request = new System.Net.Http.HttpRequestMessage();
            _controller.Configuration = new System.Web.Http.HttpConfiguration();
        }

        [Fact]
        public void Ctor_Should_Throw_ArgumentNullException_when_ApplicationInfoService_Is_Null()
        {
            Action throwAction = () => new ApplicationInfoController(null, null, null, null);

            throwAction.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("applicationInfoService");
        }

        [Fact]
        public void Get_Should_Call_Status()
        {
            //Arrange

            _mockApplicationInfoService.Setup(_ => _.GetApplicationInfo())
                .Returns(new ApplicationInfoModel() { BuildNumber = "some_number" });
            _mockConfigurationManagerService.Setup(_ => _.GetAppSetting("ApplicationInfoFilePath")).Returns(string.Empty);
            _mockConfigurationManagerService.Setup(_ => _.GetAppSetting("ApplicationInfoFileName")).Returns(string.Empty);

            var fullFilePath = Path.Combine(
                _mockConfigurationManagerService.Object.GetAppSetting("ApplicationInfoFilePath"),
                _mockConfigurationManagerService.Object.GetAppSetting("ApplicationInfoFileName"));

            _mockFileReaderService.Setup(_ => _.ReadAllText(fullFilePath)).Returns("{ 'build': { 'buildNumber': 'some_number'}}");
            
            //Act

            var result = _controller.Get();

            //Assert

            result.Should().BeOfType<OkNegotiatedContentResult<ApplicationInfoModel>>();
            
            var resultContent = result.As<OkNegotiatedContentResult<ApplicationInfoModel>>();
            resultContent.Content.BuildNumber.Should().Be("some_number");
        }

        [Fact]
        public void Class_Should_Have_V1RoutePrefix_Attribute()
        {
            typeof(ApplicationInfoController).Should()
                .BeDecoratedWith<RoutePrefixAttribute>(
                _ => _.Prefix.Equals(Constants.RoutePrefixes.V1));
        }

        [Fact]
        public void Get_Should_Have_Route_Attribute()
        {
            ReflectionHelper.GetMethod<ApplicationInfoController>(_ => _.Get())
             .Should()
             .BeDecoratedWith<RouteAttribute>(_ => _.Template.Equals("statuses"));
        }
    }
}
