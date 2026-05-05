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
    public class InbentarioaControllerTests
    {
        private readonly Mock<InbentarioaRepository> _repoMock;
        private readonly InbentarioaController _controller;

        public InbentarioaControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<InbentarioaRepository>(sessionFactoryMock.Object);
            _controller = new InbentarioaController(_repoMock.Object);
        }

        [Fact]
        public void InbentarioaController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Inbentarioa>
            {
                InbentarioaElementua(1, "Arroza"),
                InbentarioaElementua(2, "Esnea")
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<InbentarioaDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Arroza", lista[0].Izena);
        }

        [Fact]
        public void InbentarioaController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = InbentarioaSortuDto();
            Inbentarioa? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Inbentarioa>()))
                .Callback<Inbentarioa>(i =>
                {
                    i.Id = 7;
                    gordeta = i;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Elementua sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.Izena, gordeta.Izena);
            Assert.NotNull(gordeta.AzkenEguneratzea);
        }

        [Fact]
        public void InbentarioaController_Get_ElementuaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(InbentarioaElementua(1, "Arroza"));

            var result = _controller.Get(1);

            var dto = AssertOkValue<InbentarioaDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Arroza", dto.Izena);
        }

        [Fact]
        public void InbentarioaController_Get_ElementuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Inbentarioa?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void InbentarioaController_Eguneratu_ElementuaDagoenean_OkObjectResultItzultzenDu()
        {
            var elementua = InbentarioaElementua(1, "Arroza");
            _repoMock.Setup(r => r.Get(1)).Returns(elementua);
            var dto = InbentarioaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.Izena, elementua.Izena);
            Assert.Equal(dto.Deskribapena, elementua.Deskribapena);
            Assert.Equal(dto.Kantitatea, elementua.Kantitatea);
            Assert.Equal(dto.NeurriaUnitatea, elementua.NeurriaUnitatea);
            Assert.Equal(dto.StockMinimoa, elementua.StockMinimoa);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void InbentarioaController_Eguneratu_ElementuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Inbentarioa?)null);

            var result = _controller.Eguneratu(99, InbentarioaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Inbentarioa>()), Times.Never);
        }

        [Fact]
        public void InbentarioaController_EguneratuZatia_EremuGuztiekin_OkObjectResultItzultzenDu()
        {
            var elementua = InbentarioaElementua(1, "Arroza");
            _repoMock.Setup(r => r.Get(1)).Returns(elementua);
            var dto = new InbentarioaPatchDto
            {
                Izena = "Arroza berria",
                Deskribapena = "Integrala",
                Kantitatea = 25,
                NeurriaUnitatea = "poltsa",
                StockMinimoa = 4
            };

            var result = _controller.EguneratuZatia(1, dto);

            AssertOkMessage(result, "Zati batean eguneratuta");
            Assert.Equal(dto.Izena, elementua.Izena);
            Assert.Equal(dto.Deskribapena, elementua.Deskribapena);
            Assert.Equal(dto.Kantitatea, elementua.Kantitatea);
            Assert.Equal(dto.NeurriaUnitatea, elementua.NeurriaUnitatea);
            Assert.Equal(dto.StockMinimoa, elementua.StockMinimoa);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void InbentarioaController_EguneratuZatia_EremurikGabe_OkObjectResultItzultzenDu()
        {
            var elementua = InbentarioaElementua(1, "Arroza");
            var azkena = elementua.AzkenEguneratzea;
            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var result = _controller.EguneratuZatia(1, new InbentarioaPatchDto());

            AssertOkMessage(result, "Zati batean eguneratuta");
            Assert.Equal("Arroza", elementua.Izena);
            Assert.Equal("Deskribapena", elementua.Deskribapena);
            Assert.Equal(10, elementua.Kantitatea);
            Assert.Equal("kg", elementua.NeurriaUnitatea);
            Assert.Equal(2, elementua.StockMinimoa);
            Assert.True(elementua.AzkenEguneratzea > azkena);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void InbentarioaController_EguneratuZatia_ElementuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Inbentarioa?)null);

            var result = _controller.EguneratuZatia(99, new InbentarioaPatchDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Inbentarioa>()), Times.Never);
        }

        [Fact]
        public void InbentarioaController_Ezabatu_ElementuaDagoenean_OkObjectResultItzultzenDu()
        {
            var elementua = InbentarioaElementua(1, "Arroza");
            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(elementua), Times.Once);
        }

        [Fact]
        public void InbentarioaController_Ezabatu_ElementuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Inbentarioa?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Inbentarioa>()), Times.Never);
        }

        [Fact]
        public void InbentarioaController_AldatuKantitatea_EmaitzaOndoDenean_OkObjectResultItzultzenDu()
        {
            var emaitza = new EragiketaEmaitzaDto { Ondo = true, Mezua = "Kantitatea eguneratuta", Id = 1, KantitateBerria = 15 };
            _repoMock.Setup(r => r.AldatuKantitatea(1, 5)).Returns(emaitza);

            var result = _controller.AldatuKantitatea(1, new KantitateaAldatuDto { Aldaketa = 5 });

            var value = AssertOkValue<EragiketaEmaitzaDto>(result);
            Assert.Same(emaitza, value);
        }

        [Fact]
        public void InbentarioaController_AldatuKantitatea_EmaitzaOndoEzDenean_BadRequestObjectResultItzultzenDu()
        {
            var emaitza = new EragiketaEmaitzaDto { Ondo = false, Mezua = "Ez da aurkitu", Id = 99 };
            _repoMock.Setup(r => r.AldatuKantitatea(99, -5)).Returns(emaitza);

            var result = _controller.AldatuKantitatea(99, new KantitateaAldatuDto { Aldaketa = -5 });

            AssertBadRequestMessage(result, "Ez da aurkitu");
        }
    }
}
