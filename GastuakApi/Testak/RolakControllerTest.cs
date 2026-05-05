using JatetxeaApi.Controllerrak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Moq;
using NHibernate;
using Xunit;

using static JatetxeaApi.Testak.Controllerrak.ControllerTestHelpers;

namespace JatetxeaApi.Testak.Controllerrak
{
    public class RolakControllerTest
    {
        private readonly Mock<RolakRepository> _repoMock;
        private readonly RolakController _controller;

        public RolakControllerTest()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<RolakRepository>(sessionFactoryMock.Object);
            _controller = new RolakController(_repoMock.Object);
        }

        [Fact]
        public void RolakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Rolak>
            {
                Rola(1, "Admin"),
                Rola(2, "Zerbitzaria")
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<RolakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Admin", lista[0].Izena);
        }

        [Fact]
        public void RolakController_Get_RolaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Rola(1, "Admin"));

            var result = _controller.Get(1);

            var dto = AssertOkValue<RolakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Admin", dto.Izena);
        }

        [Fact]
        public void RolakController_Get_RolaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Rolak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void RolakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = RolaSortuDto("Sukaldaria");
            Rolak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Rolak>()))
                .Callback<Rolak>(r =>
                {
                    r.Id = 7;
                    gordeta = r;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Rola sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal("Sukaldaria", gordeta.Izena);
        }

        [Fact]
        public void RolakController_Eguneratu_RolaDagoenean_OkObjectResultItzultzenDu()
        {
            var rola = Rola(1, "Zaharra");
            _repoMock.Setup(r => r.Get(1)).Returns(rola);
            var dto = RolaSortuDto("Berria");

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal("Berria", rola.Izena);
            _repoMock.Verify(r => r.Update(rola), Times.Once);
        }

        [Fact]
        public void RolakController_Eguneratu_RolaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Rolak?)null);

            var result = _controller.Eguneratu(99, RolaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Rolak>()), Times.Never);
        }

        [Fact]
        public void RolakController_Ezabatu_RolaDagoenean_OkObjectResultItzultzenDu()
        {
            var rola = Rola(1, "Admin");
            _repoMock.Setup(r => r.Get(1)).Returns(rola);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(rola), Times.Once);
        }

        [Fact]
        public void RolakController_Ezabatu_RolaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Rolak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Rolak>()), Times.Never);
        }
    }
}
