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
    public class MahaiakControllerTests
    {
        private readonly Mock<MahaiakRepository> _repoMock;
        private readonly Mock<ErreserbakRepository> _erreserbakRepoMock;
        private readonly MahaiakController _controller;

        public MahaiakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<MahaiakRepository>(sessionFactoryMock.Object);
            _erreserbakRepoMock = new Mock<ErreserbakRepository>(sessionFactoryMock.Object);
            _controller = new MahaiakController(_repoMock.Object, _erreserbakRepoMock.Object);
        }

        [Fact]
        public void MahaiakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Mahaiak>
            {
                Mahaia(1, 1),
                Mahaia(2, 2)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<MahaiakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal(1, lista[0].MahaiaZbk);
        }

        [Fact]
        public void MahaiakController_Get_MahaiaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Mahaia(1, 1));

            var result = _controller.Get(1);

            var dto = AssertOkValue<MahaiakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal(1, dto.MahaiaZbk);
        }

        [Fact]
        public void MahaiakController_Get_MahaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Mahaiak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void MahaiakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = MahaiaSortuDto();
            Mahaiak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Mahaiak>()))
                .Callback<Mahaiak>(m =>
                {
                    m.Id = 7;
                    gordeta = m;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Mahai sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.MahaiaZbk, gordeta.MahaiaZbk);
            Assert.Equal(dto.Edukiera, gordeta.Edukiera);
        }

        [Fact]
        public void MahaiakController_Eguneratu_MahaiaDagoenean_OkObjectResultItzultzenDu()
        {
            var mahaia = Mahaia(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(mahaia);
            var dto = MahaiaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.MahaiaZbk, mahaia.MahaiaZbk);
            Assert.Equal(dto.Edukiera, mahaia.Edukiera);
            Assert.Equal(dto.Egoera, mahaia.Egoera);
            _repoMock.Verify(r => r.Update(mahaia), Times.Once);
        }

        [Fact]
        public void MahaiakController_Eguneratu_MahaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Mahaiak?)null);

            var result = _controller.Eguneratu(99, MahaiaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Mahaiak>()), Times.Never);
        }

        [Fact]
        public void MahaiakController_Ezabatu_MahaiaDagoenean_OkObjectResultItzultzenDu()
        {
            var mahaia = Mahaia(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(mahaia);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(mahaia), Times.Once);
        }

        [Fact]
        public void MahaiakController_Ezabatu_MahaiaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Mahaiak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Mahaiak>()), Times.Never);
        }

        [Fact]
        public void MahaiakController_GetLibre_DataOrduaHutsikDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.GetLibre("");

            AssertBadRequestMessage(result, "dataOrdua beharrezkoa da");
            _erreserbakRepoMock.Verify(r => r.GetAll(), Times.Never);
            _repoMock.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void MahaiakController_GetLibre_DataOrduaOkerraDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.GetLibre("okerra");

            AssertBadRequestMessage(result, "dataOrdua ez da zuzena");
            _erreserbakRepoMock.Verify(r => r.GetAll(), Times.Never);
            _repoMock.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void MahaiakController_GetLibre_ErreserbarikEzDagoenean_OkObjectResultItzultzenDu()
        {
            var data = new DateTime(2026, 5, 4, 14, 30, 0);
            _erreserbakRepoMock.Setup(r => r.GetAll()).Returns(new List<Erreserbak>());
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Mahaiak>
            {
                Mahaia(2, 2),
                Mahaia(1, 1)
            });

            var result = _controller.GetLibre(data.ToString("yyyy-MM-dd HH:mm:ss"));

            var lista = AssertOkEnumerable<MahaiakDto>(result);
            Assert.Equal(new[] { 1, 2 }, lista.Select(m => m.MahaiaZbk).ToArray());
        }

        [Fact]
        public void MahaiakController_GetLibre_ErreserbaAktiboekin_OkObjectResultItzultzenDu()
        {
            var data = new DateTime(2026, 5, 4, 14, 30, 0);
            _erreserbakRepoMock.Setup(r => r.GetAll()).Returns(new List<Erreserbak>
            {
                Erreserba(1, mahaiaId: 2, erreserbaData: data, egoera: "Baieztatuta"),
                Erreserba(2, mahaiaId: 3, erreserbaData: data.AddHours(1), egoera: "Baieztatuta")
            });
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Mahaiak>
            {
                Mahaia(1, 1),
                Mahaia(2, 2),
                Mahaia(3, 3)
            });

            var result = _controller.GetLibre(data.ToString("yyyy-MM-dd HH:mm:ss"));

            var lista = AssertOkEnumerable<MahaiakDto>(result);
            Assert.Equal(new[] { 1, 3 }, lista.Select(m => m.Id).ToArray());
        }

        [Fact]
        public void MahaiakController_GetLibre_ErreserbaKantzelatuEdoDataGabeekin_OkObjectResultItzultzenDu()
        {
            var data = new DateTime(2026, 5, 4, 14, 30, 0);
            _erreserbakRepoMock.Setup(r => r.GetAll()).Returns(new List<Erreserbak>
            {
                Erreserba(1, mahaiaId: 1, erreserbaData: data, egoera: "Kantzelatuta"),
                new Erreserbak
                {
                    Id = 2,
                    MahaiaId = 2,
                    Izena = "Data gabe",
                    Telefonoa = 600000002,
                    ErreserbaData = null,
                    PertsonaKop = 4,
                    Egoera = "Baieztatuta"
                }
            });
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Mahaiak>
            {
                Mahaia(1, 1),
                Mahaia(2, 2)
            });

            var result = _controller.GetLibre(data.ToString("yyyy-MM-dd HH:mm:ss"));

            var lista = AssertOkEnumerable<MahaiakDto>(result);
            Assert.Equal(new[] { 1, 2 }, lista.Select(m => m.Id).ToArray());
        }
    }
}
