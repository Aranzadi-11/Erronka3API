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
    public class KategoriaControllerTests
    {
        private readonly Mock<KategoriaRepository> _repoMock;
        private readonly KategoriaController _controller;

        public KategoriaControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<KategoriaRepository>(sessionFactoryMock.Object);
            _controller = new KategoriaController(_repoMock.Object);
        }

        [Fact]
        public void KategoriaController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Kategoria>
            {
                Kategoria(1, "Edariak"),
                Kategoria(2, "Postreak")
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<KategoriaDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Edariak", lista[0].Izena);
        }

        [Fact]
        public void KategoriaController_Get_KategoriaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Kategoria(1, "Edariak"));

            var result = _controller.Get(1);

            var dto = AssertOkValue<KategoriaDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Edariak", dto.Izena);
        }

        [Fact]
        public void KategoriaController_Get_KategoriaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Kategoria?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void KategoriaController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = KategoriaSortuDto("Gosariak");
            Kategoria? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Kategoria>()))
                .Callback<Kategoria>(k =>
                {
                    k.Id = 7;
                    gordeta = k;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Kategoria sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal("Gosariak", gordeta.Izena);
        }

        [Fact]
        public void KategoriaController_Eguneratu_KategoriaDagoenean_OkObjectResultItzultzenDu()
        {
            var kategoria = Kategoria(1, "Zaharra");
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);
            var dto = KategoriaSortuDto("Berria");

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal("Berria", kategoria.Izena);
            _repoMock.Verify(r => r.Update(kategoria), Times.Once);
        }

        [Fact]
        public void KategoriaController_Eguneratu_KategoriaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Kategoria?)null);

            var result = _controller.Eguneratu(99, KategoriaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Kategoria>()), Times.Never);
        }

        [Fact]
        public void KategoriaController_Ezabatu_KategoriaDagoenean_OkObjectResultItzultzenDu()
        {
            var kategoria = Kategoria(1, "Edariak");
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(kategoria), Times.Once);
        }

        [Fact]
        public void KategoriaController_Ezabatu_KategoriaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Kategoria?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Kategoria>()), Times.Never);
        }
    }
}
