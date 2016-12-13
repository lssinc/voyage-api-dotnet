﻿using FluentAssertions;
using Launchpad.Models;
using Launchpad.Web.IntegrationTests.Extensions;
using Launchpad.Web.IntegrationTests.Hosting;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Launchpad.Web.IntegrationTests.Tests.Widgets
{
    [Trait("Category", "Self-Hosted")]
    [Collection(HostCollectionFixture.Name)]
    public class GetWidgetsTests : ApiTest
    {
        public override HttpMethod Method => HttpMethod.Get;

        public override string PathUnderTest => "/api/v1/widgets";

        public GetWidgetsTests(HostFixture hostFixture) : base(hostFixture)
        {
        }

        [Fact]
        public async void GetWidgets_Should_Return_Status_200()
        {
            // ARRANGE
            var httpRequestMessage = CreateSecureRequest(Method, PathUnderTest);

            // ACT
            var response = await Client.SendAsync(httpRequestMessage);

            // ASSERT
            response.Should()
                .HaveStatusCode(HttpStatusCode.OK);

            WidgetModel[] models = await response.ReadBody<WidgetModel[]>();
            models.Should().NotBeNullOrEmpty();
        }
    }
}
