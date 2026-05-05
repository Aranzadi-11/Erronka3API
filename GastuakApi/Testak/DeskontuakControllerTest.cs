using JatetxeaApi.Controllerrak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

using static JatetxeaApi.Testak.Controllerrak.ControllerTestHelpers;

namespace JatetxeaApi.Testak.Controllerrak
{
    public class DeskontuakControllerTest
    {
        private readonly Mock<DeskuntuakRepository> _repoMock;
        private readonly DeskontuakController _controller;

        public DeskontuakControllerTest()
        {
            var configuration = new ConfigurationBuilder().Build();
            _repoMock = new Mock<DeskuntuakRepository>(configuration);
            _controller = new DeskontuakController(_repoMock.Object);
        }

        [Fact]
        public async Task DeskontuakController_Validate_KodeaDagoenean_OkObjectResultItzultzenDu()
        {
            var dto = new DeskuntuKodeaDto { Code = "UDA10" };
            var emaitza = new DeskuntuEmaitzaDto { Valid = true, Message = "OK", Percentage = 10, CodeId = 1 };
            _repoMock.Setup(r => r.ValidateAsync("UDA10")).ReturnsAsync(emaitza);

            var result = await _controller.Validate(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(emaitza, ok.Value);
            _repoMock.Verify(r => r.ValidateAsync("UDA10"), Times.Once);
        }

        [Fact]
        public async Task DeskontuakController_Validate_GorputzaNullDenean_OkObjectResultItzultzenDu()
        {
            var emaitza = new DeskuntuEmaitzaDto { Valid = false, Message = "Kodea beharrezkoa da" };
            _repoMock.Setup(r => r.ValidateAsync(null)).ReturnsAsync(emaitza);

            var result = await _controller.Validate(null);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(emaitza, ok.Value);
            _repoMock.Verify(r => r.ValidateAsync(null), Times.Once);
        }

        [Fact]
        public async Task DeskontuakController_Apply_KodeaDagoenean_OkObjectResultItzultzenDu()
        {
            var dto = new DeskuntuKodeaDto { Kodea = "UDA10" };
            var emaitza = new DeskuntuEmaitzaDto { Valid = true, Message = "OK", Percentage = 10, CodeId = 1 };
            _repoMock.Setup(r => r.ApplyAsync("UDA10")).ReturnsAsync(emaitza);

            var result = await _controller.Apply(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(emaitza, ok.Value);
            _repoMock.Verify(r => r.ApplyAsync("UDA10"), Times.Once);
        }

        [Fact]
        public async Task DeskontuakController_Apply_GorputzaNullDenean_OkObjectResultItzultzenDu()
        {
            var emaitza = new DeskuntuEmaitzaDto { Valid = false, Message = "Kodea beharrezkoa da" };
            _repoMock.Setup(r => r.ApplyAsync(null)).ReturnsAsync(emaitza);

            var result = await _controller.Apply(null);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(emaitza, ok.Value);
            _repoMock.Verify(r => r.ApplyAsync(null), Times.Once);
        }
    }
}
