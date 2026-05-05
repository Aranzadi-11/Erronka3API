using JatetxeaApi.Controllerrak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Moq;
using NHibernate;
using Xunit;

using static JatetxeaApi.Testak.Controllerrak.ControllerTestHelpers;

namespace JatetxeaApi.Testak.Controllerrak
{
    public class PlaterenOsagaiakControllerTest
    {
        private readonly Mock<PlaterenOsagaiakRepository> _repoMock;
        private readonly PlaterenOsagaiakController _controller;

        public PlaterenOsagaiakControllerTest()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<PlaterenOsagaiakRepository>(sessionFactoryMock.Object);
            _controller = new PlaterenOsagaiakController(_repoMock.Object);
        }

        [Fact]
        public void PlaterenOsagaiakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PlaterenOsagaiak>
            {
                PlaterenOsagaia(1, 2),
                PlaterenOsagaia(1, 3)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<PlaterenOsagaiak>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal(2, lista[0].InbentarioaId);
        }

        [Fact]
        public void PlaterenOsagaiakController_Get_OsagaiaDagoenean_OkObjectResultItzultzenDu()
        {
            var osagaia = PlaterenOsagaia(1, 2);
            _repoMock.Setup(r => r.Get(1, 2)).Returns(osagaia);

            var result = _controller.Get(1, 2);

            var dto = AssertOkValue<PlaterenOsagaiak>(result);
            Assert.Same(osagaia, dto);
        }

        [Fact]
        public void PlaterenOsagaiakController_Get_OsagaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1, 99)).Returns((PlaterenOsagaiak?)null);

            var result = _controller.Get(1, 99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void PlaterenOsagaiakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = PlaterenOsagaia(4, 5);
            PlaterenOsagaiak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<PlaterenOsagaiak>()))
                .Callback<PlaterenOsagaiak>(p => gordeta = p);

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Elementua sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(4, Property<int>(ok.Value!, "platareaId"));
            Assert.Equal(5, Property<int>(ok.Value!, "inbentarioaId"));
            Assert.NotNull(gordeta);
            Assert.Equal(3m, gordeta.Kantitatea);
        }

        [Fact]
        public void PlaterenOsagaiakController_Eguneratu_OsagaiaDagoenean_OkObjectResultItzultzenDu()
        {
            var osagaia = PlaterenOsagaia(1, 2);
            _repoMock.Setup(r => r.Get(1, 2)).Returns(osagaia);
            var dto = new PlaterenOsagaiak { PlateraId = 1, InbentarioaId = 2, Kantitatea = 8m };

            var result = _controller.Eguneratu(1, 2, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(8m, osagaia.Kantitatea);
            _repoMock.Verify(r => r.Update(osagaia), Times.Once);
        }

        [Fact]
        public void PlaterenOsagaiakController_Eguneratu_OsagaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1, 99)).Returns((PlaterenOsagaiak?)null);

            var result = _controller.Eguneratu(1, 99, PlaterenOsagaia(1, 99));

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<PlaterenOsagaiak>()), Times.Never);
        }

        [Fact]
        public void PlaterenOsagaiakController_Ezabatu_OsagaiaDagoenean_OkObjectResultItzultzenDu()
        {
            var osagaia = PlaterenOsagaia(1, 2);
            _repoMock.Setup(r => r.Get(1, 2)).Returns(osagaia);

            var result = _controller.Ezabatu(1, 2);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(osagaia), Times.Once);
        }

        [Fact]
        public void PlaterenOsagaiakController_Ezabatu_OsagaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1, 99)).Returns((PlaterenOsagaiak?)null);

            var result = _controller.Ezabatu(1, 99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<PlaterenOsagaiak>()), Times.Never);
        }
    }
}
