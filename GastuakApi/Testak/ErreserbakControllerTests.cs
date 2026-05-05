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
    public class ErreserbakControllerTests
    {
        private readonly Mock<ErreserbakRepository> _repoMock;
        private readonly Mock<ZerbitzuakRepository> _zerbitzuRepoMock;
        private readonly ErreserbakController _controller;

        public ErreserbakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<ErreserbakRepository>(sessionFactoryMock.Object);
            _zerbitzuRepoMock = new Mock<ZerbitzuakRepository>(sessionFactoryMock.Object);
            _controller = new ErreserbakController(_repoMock.Object, _zerbitzuRepoMock.Object);
        }

        [Fact]
        public void ErreserbakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Erreserbak>
            {
                Erreserba(1),
                Erreserba(2)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Erreserba1", lista[0].Izena);
        }

        [Fact]
        public void ErreserbakController_Get_ErreserbaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Erreserba(1));

            var result = _controller.Get(1);

            var dto = AssertOkValue<ErreserbakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Erreserba1", dto.Izena);
        }

        [Fact]
        public void ErreserbakController_Get_ErreserbaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Erreserbak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void ErreserbakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = ErreserbaSortuDto();
            Erreserbak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Erreserbak>()))
                .Callback<Erreserbak>(e =>
                {
                    e.Id = 7;
                    gordeta = e;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Erreserba sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.MahaiaId, gordeta.MahaiaId);
            Assert.Equal(dto.Izena, gordeta.Izena);
        }

        [Fact]
        public void ErreserbakController_Eguneratu_ErreserbaDagoenean_OkObjectResultItzultzenDu()
        {
            var erreserba = Erreserba(1);
            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            var dto = ErreserbaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.MahaiaId, erreserba.MahaiaId);
            Assert.Equal(dto.Izena, erreserba.Izena);
            Assert.Equal(dto.Telefonoa, erreserba.Telefonoa);
            Assert.Equal(dto.ErreserbaData, erreserba.ErreserbaData);
            Assert.Equal(dto.PertsonaKop, erreserba.PertsonaKop);
            Assert.Equal(dto.Egoera, erreserba.Egoera);
            Assert.Equal(dto.Oharrak, erreserba.Oharrak);
            _repoMock.Verify(r => r.Update(erreserba), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Eguneratu_ErreserbaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Erreserbak?)null);

            var result = _controller.Eguneratu(99, ErreserbaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Erreserbak>()), Times.Never);
        }

        [Fact]
        public void ErreserbakController_Ezabatu_ErreserbaLoturarikGabeDagoenean_OkObjectResultItzultzenDu()
        {
            var erreserba = Erreserba(1);
            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _zerbitzuRepoMock.Setup(r => r.GetAll()).Returns(new List<Zerbitzuak>
            {
                Zerbitzua(2, erreserbaId: 99)
            });

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _zerbitzuRepoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Never);
            _repoMock.Verify(r => r.Delete(erreserba), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Ezabatu_ErreserbaLotutakoZerbitzuekinDagoenean_OkObjectResultItzultzenDu()
        {
            var erreserba = Erreserba(1);
            var zerbitzuak = new List<Zerbitzuak>
            {
                Zerbitzua(1, erreserbaId: 1),
                Zerbitzua(2, erreserbaId: 1),
                Zerbitzua(3, erreserbaId: 3)
            };
            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _zerbitzuRepoMock.Setup(r => r.GetAll()).Returns(zerbitzuak);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            Assert.Null(zerbitzuak[0].ErreserbaId);
            Assert.Null(zerbitzuak[1].ErreserbaId);
            Assert.Equal(3, zerbitzuak[2].ErreserbaId);
            _zerbitzuRepoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Exactly(2));
            _repoMock.Verify(r => r.Delete(erreserba), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Ezabatu_ErreserbaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Erreserbak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _zerbitzuRepoMock.Verify(r => r.GetAll(), Times.Never);
            _repoMock.Verify(r => r.Delete(It.IsAny<Erreserbak>()), Times.Never);
        }

        [Fact]
        public void ErreserbakController_GetGaur_GaurkoZerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetGaur()).Returns(new List<Erreserbak> { Erreserba(1) });

            var result = _controller.GetGaur();

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            Assert.Equal(1, lista[0].Id);
        }

        [Fact]
        public void ErreserbakController_GetEtorkizunak_EtorkizunekoZerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetEtorkizunak()).Returns(new List<Erreserbak> { Erreserba(2) });

            var result = _controller.GetEtorkizunak();

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            Assert.Equal(2, lista[0].Id);
        }

        [Fact]
        public void ErreserbakController_Bilatu_ParametrorikGabe_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Bilatu(null, null)).Returns(new List<Erreserbak> { Erreserba(1) });

            var result = _controller.Bilatu(null, null);

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            _repoMock.Verify(r => r.Bilatu(null, null), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Bilatu_DataOkerrarekin_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.Bilatu("okerra", null);

            AssertBadRequestMessage(result, "data ez da zuzena");
            _repoMock.Verify(r => r.Bilatu(It.IsAny<DateTime?>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        [Fact]
        public void ErreserbakController_Bilatu_OrduaOkerrarekin_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.Bilatu("2026-05-04", "okerra");

            AssertBadRequestMessage(result, "ordua ez da zuzena");
            _repoMock.Verify(r => r.Bilatu(It.IsAny<DateTime?>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        [Fact]
        public void ErreserbakController_Bilatu_DataBaliozkoarekin_OkObjectResultItzultzenDu()
        {
            var data = new DateTime(2026, 5, 4);
            _repoMock.Setup(r => r.Bilatu(It.IsAny<DateTime?>(), null)).Returns(new List<Erreserbak> { Erreserba(1) });

            var result = _controller.Bilatu("2026-05-04", null);

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            _repoMock.Verify(r => r.Bilatu(data, null), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Bilatu_OrduaBaliozkoarekin_OkObjectResultItzultzenDu()
        {
            var ordua = new TimeSpan(14, 30, 0);
            _repoMock.Setup(r => r.Bilatu(null, It.IsAny<TimeSpan?>())).Returns(new List<Erreserbak> { Erreserba(1) });

            var result = _controller.Bilatu(null, "14:30");

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            _repoMock.Verify(r => r.Bilatu(null, ordua), Times.Once);
        }

        [Fact]
        public void ErreserbakController_Bilatu_DataEtaOrduaBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var data = new DateTime(2026, 5, 4);
            var ordua = new TimeSpan(14, 30, 0);
            _repoMock.Setup(r => r.Bilatu(It.IsAny<DateTime?>(), It.IsAny<TimeSpan?>())).Returns(new List<Erreserbak> { Erreserba(1) });

            var result = _controller.Bilatu("2026-05-04", "14:30");

            var lista = AssertOkEnumerable<ErreserbakDto>(result);
            Assert.Single(lista);
            _repoMock.Verify(r => r.Bilatu(data, ordua), Times.Once);
        }
    }
}
