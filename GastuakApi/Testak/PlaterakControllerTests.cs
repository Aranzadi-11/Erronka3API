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
    public class PlaterakControllerTests
    {
        private readonly Mock<PlaterakRepository> _repoMock;
        private readonly PlaterakController _controller;

        public PlaterakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<PlaterakRepository>(sessionFactoryMock.Object);
            _controller = new PlaterakController(_repoMock.Object);
        }

        [Fact]
        public void PlaterakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Platerak>
            {
                Platera(1, "Entsalada"),
                Platera(2, "Zopa")
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<PlaterakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Entsalada", lista[0].Izena);
        }

        [Fact]
        public void PlaterakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = PlateraSortuDto();
            Platerak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Platerak>()))
                .Callback<Platerak>(p =>
                {
                    p.Id = 7;
                    gordeta = p;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Platera sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.Izena, gordeta.Izena);
            Assert.NotNull(gordeta.SortzeData);
        }

        [Fact]
        public void PlaterakController_Get_PlateraDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Platera(1, "Entsalada"));

            var result = _controller.Get(1);

            var dto = AssertOkValue<PlaterakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Entsalada", dto.Izena);
        }

        [Fact]
        public void PlaterakController_Get_PlateraEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Platerak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void PlaterakController_Eguneratu_PlateraDagoenean_OkObjectResultItzultzenDu()
        {
            var platera = Platera(1, "Zaharra");
            _repoMock.Setup(r => r.Get(1)).Returns(platera);
            var dto = PlateraSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.Izena, platera.Izena);
            Assert.Equal(dto.Deskribapena, platera.Deskribapena);
            Assert.Equal(dto.Prezioa, platera.Prezioa);
            Assert.Equal(dto.KategoriaId, platera.KategoriaId);
            Assert.Equal(dto.Erabilgarri, platera.Erabilgarri);
            Assert.Equal(dto.Irudia, platera.Irudia);
            _repoMock.Verify(r => r.Update(platera), Times.Once);
        }

        [Fact]
        public void PlaterakController_Eguneratu_PlateraEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Platerak?)null);

            var result = _controller.Eguneratu(99, PlateraSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Platerak>()), Times.Never);
        }

        [Fact]
        public void PlaterakController_EguneratuZatia_EremuGuztiekin_OkObjectResultItzultzenDu()
        {
            var platera = Platera(1, "Entsalada");
            _repoMock.Setup(r => r.Get(1)).Returns(platera);
            var dto = new PlaterakPatchDto
            {
                Izena = "Entsalada berria",
                Deskribapena = "Berritua",
                Prezioa = 20m,
                KategoriaId = 3,
                Erabilgarri = "Ez",
                Irudia = "berria.png"
            };

            var result = _controller.EguneratuZatia(1, dto);

            AssertOkMessage(result, "Zati batean eguneratuta");
            Assert.Equal(dto.Izena, platera.Izena);
            Assert.Equal(dto.Deskribapena, platera.Deskribapena);
            Assert.Equal(dto.Prezioa, platera.Prezioa);
            Assert.Equal(dto.KategoriaId, platera.KategoriaId);
            Assert.Equal(dto.Erabilgarri, platera.Erabilgarri);
            Assert.Equal(dto.Irudia, platera.Irudia);
            _repoMock.Verify(r => r.Update(platera), Times.Once);
        }

        [Fact]
        public void PlaterakController_EguneratuZatia_EremurikGabe_OkObjectResultItzultzenDu()
        {
            var platera = Platera(1, "Entsalada");
            _repoMock.Setup(r => r.Get(1)).Returns(platera);

            var result = _controller.EguneratuZatia(1, new PlaterakPatchDto());

            AssertOkMessage(result, "Zati batean eguneratuta");
            Assert.Equal("Entsalada", platera.Izena);
            Assert.Equal("Freskoa", platera.Deskribapena);
            Assert.Equal(12.5m, platera.Prezioa);
            Assert.Equal(1, platera.KategoriaId);
            Assert.Equal("Bai", platera.Erabilgarri);
            Assert.Equal("irudia.png", platera.Irudia);
            _repoMock.Verify(r => r.Update(platera), Times.Once);
        }

        [Fact]
        public void PlaterakController_EguneratuZatia_PlateraEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Platerak?)null);

            var result = _controller.EguneratuZatia(99, new PlaterakPatchDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Platerak>()), Times.Never);
        }

        [Fact]
        public void PlaterakController_Ezabatu_PlateraDagoenean_OkObjectResultItzultzenDu()
        {
            var platera = Platera(1, "Entsalada");
            _repoMock.Setup(r => r.Get(1)).Returns(platera);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(platera), Times.Once);
        }

        [Fact]
        public void PlaterakController_Ezabatu_PlateraEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Platerak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Platerak>()), Times.Never);
        }

        [Fact]
        public void PlaterakController_GetDisponibilitatea_Zerrenda_OkObjectResultItzultzenDu()
        {
            var erabilgarritasuna = new List<PlateraDisponibilitateaDto>
            {
                new PlateraDisponibilitateaDto
                {
                    Id = 1,
                    Izena = "Entsalada",
                    KategoriaId = 1,
                    KategoriaIzena = "Hotzak",
                    Erabilgarri = "Bai",
                    PrestatuDaitezkeenUnitateak = 5
                }
            };
            _repoMock.Setup(r => r.GetDisponibilitatea()).Returns(erabilgarritasuna);

            var result = _controller.GetDisponibilitatea();

            var lista = AssertOkEnumerable<PlateraDisponibilitateaDto>(result);
            Assert.Single(lista);
            Assert.Equal(5, lista[0].PrestatuDaitezkeenUnitateak);
        }
    }
}
