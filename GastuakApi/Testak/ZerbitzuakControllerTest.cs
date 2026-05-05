using GastuakApi.DTOak;
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
    public class ZerbitzuakControllerTest
    {
        private readonly Mock<ZerbitzuakRepository> _repoMock;
        private readonly ZerbitzuakController _controller;

        public ZerbitzuakControllerTest()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<ZerbitzuakRepository>(sessionFactoryMock.Object);
            _controller = new ZerbitzuakController(_repoMock.Object);
        }

        [Fact]
        public void ZerbitzuakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Zerbitzuak>
            {
                Zerbitzua(1, 1),
                Zerbitzua(2, 2)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<ZerbitzuakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal(1, lista[0].Id);
        }

        [Fact]
        public void ZerbitzuakController_Get_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Zerbitzua(1, 1));

            var result = _controller.Get(1);

            var dto = AssertOkValue<ZerbitzuakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal(1, dto.ErreserbaId);
        }

        [Fact]
        public void ZerbitzuakController_Get_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Zerbitzuak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void ZerbitzuakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = ZerbitzuaSortuDto();
            Zerbitzuak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Zerbitzuak>()))
                .Callback<Zerbitzuak>(z =>
                {
                    z.Id = 7;
                    gordeta = z;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Zerbitzuak sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.LangileId, gordeta.LangileId);
            Assert.Equal(dto.MahaiaId, gordeta.MahaiaId);
            Assert.Equal(dto.ErreserbaId, gordeta.ErreserbaId);
            Assert.NotNull(gordeta.EskaeraData);
        }

        [Fact]
        public void ZerbitzuakController_Eguneratu_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            var zerbitzua = Zerbitzua(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);
            var dto = ZerbitzuaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.LangileId, zerbitzua.LangileId);
            Assert.Equal(dto.MahaiaId, zerbitzua.MahaiaId);
            Assert.Equal(dto.ErreserbaId, zerbitzua.ErreserbaId);
            Assert.Equal(dto.EskaeraData, zerbitzua.EskaeraData);
            Assert.Equal(dto.Egoera, zerbitzua.Egoera);
            Assert.Equal(dto.Guztira, zerbitzua.Guztira);
            _repoMock.Verify(r => r.Update(zerbitzua), Times.Once);
        }

        [Fact]
        public void ZerbitzuakController_Eguneratu_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Zerbitzuak?)null);

            var result = _controller.Eguneratu(99, ZerbitzuaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuakController_Ezabatu_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            var zerbitzua = Zerbitzua(1, 1);
            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(zerbitzua), Times.Once);
        }

        [Fact]
        public void ZerbitzuakController_Ezabatu_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Zerbitzuak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Zerbitzuak>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuakController_ZerbitzuaEgin_EmaitzaOndoDenean_OkObjectResultItzultzenDu()
        {
            var dto = ZerbitzuaEskaria();
            var emaitza = new ZerbitzuaEmaitzaDto { Ondo = true, ZerbitzuaId = 10 };
            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var value = AssertOkValue<ZerbitzuaEmaitzaDto>(result);
            Assert.Same(emaitza, value);
            Assert.True(value.Ondo);
        }

        [Fact]
        public void ZerbitzuakController_ZerbitzuaEgin_EmaitzaOndoEzDenean_OkObjectResultItzultzenDu()
        {
            var dto = ZerbitzuaEskaria();
            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = false,
                ZerbitzuaId = null,
                Erroreak = new List<ZerbitzuErroreaDto>
                {
                    new ZerbitzuErroreaDto { PlateraId = 1, PlateraIzena = "Entsalada" }
                }
            };
            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var value = AssertOkValue<ZerbitzuaEmaitzaDto>(result);
            Assert.Same(emaitza, value);
            Assert.False(value.Ondo);
        }

        [Fact]
        public void ZerbitzuakController_GetPlaterakByErreserba_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            var zerbitzua = Zerbitzua(30, 99);
            var platerak = new List<object>
            {
                new { PlateraId = 1, Kantitatea = 2, Zerbitzatuta = false },
                new { PlateraId = 2, Kantitatea = 1, Zerbitzatuta = true }
            };
            _repoMock.Setup(r => r.GetByErreserbaId(99)).Returns(zerbitzua);
            _repoMock.Setup(r => r.GetPlaterakLaburpenaByZerbitzuaId(30)).Returns(platerak);

            var result = _controller.GetPlaterakByErreserba(99);

            var lista = AssertOkEnumerable<object>(result);
            Assert.Equal(2, lista.Count);
        }

        [Fact]
        public void ZerbitzuakController_GetPlaterakByErreserba_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetByErreserbaId(99)).Returns((Zerbitzuak?)null);

            var result = _controller.GetPlaterakByErreserba(99);

            AssertNotFoundMessage(result, "Ez dago zerbitzurik erreserba honekin");
            _repoMock.Verify(r => r.GetPlaterakLaburpenaByZerbitzuaId(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuakController_GetGaur_GaurkoZerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetGaur()).Returns(new List<Zerbitzuak> { Zerbitzua(1, 1) });

            var result = _controller.GetGaur();

            var lista = AssertOkEnumerable<ZerbitzuakDto>(result);
            Assert.Single(lista);
            Assert.Equal(1, lista[0].Id);
        }

        [Fact]
        public void ZerbitzuakController_GetEgunekoak_EgunekoZerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetEgunekoak()).Returns(new List<Zerbitzuak> { Zerbitzua(2, 2) });

            var result = _controller.GetEgunekoak();

            var lista = AssertOkEnumerable<ZerbitzuakDto>(result);
            Assert.Single(lista);
            Assert.Equal(2, lista[0].Id);
        }

        [Fact]
        public void ZerbitzuakController_GetByErreserba_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetByErreserbaId(99)).Returns(Zerbitzua(1, 99));

            var result = _controller.GetByErreserba(99);

            var dto = AssertOkValue<ZerbitzuakDto>(result);
            Assert.Equal(99, dto.ErreserbaId);
        }

        [Fact]
        public void ZerbitzuakController_GetByErreserba_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetByErreserbaId(99)).Returns((Zerbitzuak?)null);

            var result = _controller.GetByErreserba(99);

            AssertNotFoundMessage(result, "Ez dago zerbitzurik erreserba honekin");
        }

        [Fact]
        public void ZerbitzuakController_GetLaburpenaByErreserba_Laburpenarekin_OkObjectResultItzultzenDu()
        {
            var laburpena = new List<ZerbitzuLaburpenaDto>
            {
                new ZerbitzuLaburpenaDto
                {
                    ZerbitzuaId = 1,
                    ZerbitzuXehetasunaId = 2,
                    PlateraId = 3,
                    PlateraIzena = "Entsalada",
                    Kantitatea = 2,
                    PrezioUnitarioa = 9.5m,
                    Zerbitzatuta = false
                }
            };
            _repoMock.Setup(r => r.GetLaburpenaByErreserbaId(99)).Returns(laburpena);

            var result = _controller.GetLaburpenaByErreserba(99);

            var lista = AssertOkEnumerable<ZerbitzuLaburpenaDto>(result);
            Assert.Single(lista);
            Assert.Equal("Entsalada", lista[0].PlateraIzena);
        }

        [Fact]
        public void ZerbitzuakController_GetLaburpenaByErreserba_LaburpenikGabe_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetLaburpenaByErreserbaId(99)).Returns(new List<ZerbitzuLaburpenaDto>());

            var result = _controller.GetLaburpenaByErreserba(99);

            var lista = AssertOkEnumerable<ZerbitzuLaburpenaDto>(result);
            Assert.Empty(lista);
        }

        [Fact]
        public void ZerbitzuakController_AldatuEgoera_GorputzaNullDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.AldatuEgoera(1, null!);

            AssertBadRequestMessage(result, "Egoera beharrezkoa da");
            _repoMock.Verify(r => r.AldatuEgoera(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuakController_AldatuEgoera_EgoeraHutsikDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.AldatuEgoera(1, new ZerbitzuEgoeraPatchDto { Egoera = "   " });

            AssertBadRequestMessage(result, "Egoera beharrezkoa da");
            _repoMock.Verify(r => r.AldatuEgoera(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ZerbitzuakController_AldatuEgoera_ZerbitzuaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.AldatuEgoera(99, "Amaituta")).Returns(false);

            var result = _controller.AldatuEgoera(99, new ZerbitzuEgoeraPatchDto { Egoera = "Amaituta" });

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void ZerbitzuakController_AldatuEgoera_ZerbitzuaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.AldatuEgoera(1, "Amaituta")).Returns(true);

            var result = _controller.AldatuEgoera(1, new ZerbitzuEgoeraPatchDto { Egoera = "Amaituta" });

            AssertOkMessage(result, "Egoera eguneratuta");
        }

        private static ZerbitzuaEskariaDto ZerbitzuaEskaria()
        {
            return new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 3,
                EskaeraData = new DateTime(2026, 5, 4, 14, 30, 0),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 1, Kantitatea = 2 }
                }
            };
        }
    }
}
