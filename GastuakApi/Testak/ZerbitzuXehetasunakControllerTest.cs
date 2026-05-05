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
    public class ZerbitzuXehetasunakControllerTest
    {
        private readonly Mock<ZerbitzuXehetasunakRepository> _repoMock;
        private readonly ZerbitzuXehetasunakController _controller;

        public ZerbitzuXehetasunakControllerTest()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<ZerbitzuXehetasunakRepository>(sessionFactoryMock.Object);
            _controller = new ZerbitzuXehetasunakController(_repoMock.Object);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<ZerbitzuXehetasunak>
            {
                ZerbitzuXehetasuna(1, 1),
                ZerbitzuXehetasuna(2, 1)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<ZerbitzuXehetasunakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal(1, lista[0].ZerbitzuaId);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = ZerbitzuXehetasunaSortuDto();
            ZerbitzuXehetasunak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<ZerbitzuXehetasunak>()))
                .Callback<ZerbitzuXehetasunak>(z =>
                {
                    z.Id = 7;
                    gordeta = z;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Xehetasuna sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.ZerbitzuaId, gordeta.ZerbitzuaId);
            Assert.Equal(dto.Zerbitzatuta, gordeta.Zerbitzatuta);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Get_XehetasunaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(ZerbitzuXehetasuna(1, 1));

            var result = _controller.Get(1);

            var dto = AssertOkValue<ZerbitzuXehetasunakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal(1, dto.ZerbitzuaId);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Get_XehetasunaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((ZerbitzuXehetasunak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Eguneratu_XehetasunaDagoenean_OkObjectResultItzultzenDu()
        {
            var xehetasuna = ZerbitzuXehetasuna(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(xehetasuna);
            var dto = ZerbitzuXehetasunaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.ZerbitzuaId, xehetasuna.ZerbitzuaId);
            Assert.Equal(dto.PlateraId, xehetasuna.PlateraId);
            Assert.Equal(dto.Kantitatea, xehetasuna.Kantitatea);
            Assert.Equal(dto.PrezioUnitarioa, xehetasuna.PrezioUnitarioa);
            Assert.Equal(dto.Zerbitzatuta, xehetasuna.Zerbitzatuta);
            _repoMock.Verify(r => r.Update(xehetasuna), Times.Once);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Eguneratu_XehetasunaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((ZerbitzuXehetasunak?)null);

            var result = _controller.Eguneratu(99, ZerbitzuXehetasunaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<ZerbitzuXehetasunak>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Ezabatu_XehetasunaDagoenean_OkObjectResultItzultzenDu()
        {
            var xehetasuna = ZerbitzuXehetasuna(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(xehetasuna);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(xehetasuna), Times.Once);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_Ezabatu_XehetasunaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((ZerbitzuXehetasunak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<ZerbitzuXehetasunak>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_GetByZerbitzua_Xehetasunekin_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetByZerbitzuaId(5)).Returns(new List<ZerbitzuXehetasunak>
            {
                ZerbitzuXehetasuna(1, 5),
                ZerbitzuXehetasuna(2, 5)
            });

            var result = _controller.GetByZerbitzua(5);

            var lista = AssertOkEnumerable<ZerbitzuXehetasunakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.All(lista, x => Assert.Equal(5, x.ZerbitzuaId));
        }

        [Fact]
        public void ZerbitzuXehetasunakController_GetByZerbitzua_XehetasunikGabe_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetByZerbitzuaId(5)).Returns(new List<ZerbitzuXehetasunak>());

            var result = _controller.GetByZerbitzua(5);

            var lista = AssertOkEnumerable<ZerbitzuXehetasunakDto>(result);
            Assert.Empty(lista);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_AldatuZerbitzatuta_EmaitzaOndoDenean_OkObjectResultItzultzenDu()
        {
            var emaitza = new EragiketaEmaitzaDto { Ondo = true, Mezua = "Platera eginda", Id = 1 };
            _repoMock.Setup(r => r.AldatuZerbitzatutaEtaStock(1, true)).Returns(emaitza);

            var result = _controller.AldatuZerbitzatuta(1, new ZerbitzatutaPatchDto { Zerbitzatuta = true });

            var value = AssertOkValue<EragiketaEmaitzaDto>(result);
            Assert.Same(emaitza, value);
        }

        [Fact]
        public void ZerbitzuXehetasunakController_AldatuZerbitzatuta_EmaitzaOndoEzDenean_BadRequestObjectResultItzultzenDu()
        {
            var emaitza = new EragiketaEmaitzaDto { Ondo = false, Mezua = "Ez dago stock nahikorik", Id = 1 };
            _repoMock.Setup(r => r.AldatuZerbitzatutaEtaStock(1, true)).Returns(emaitza);

            var result = _controller.AldatuZerbitzatuta(1, new ZerbitzatutaPatchDto { Zerbitzatuta = true });

            AssertBadRequestMessage(result, "Ez dago stock nahikorik");
        }
    }
}
