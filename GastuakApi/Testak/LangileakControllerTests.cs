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
    public class LangileakControllerTests
    {
        private readonly Mock<LangileakRepository> _repoMock;
        private readonly LangileakController _controller;

        public LangileakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<LangileakRepository>(sessionFactoryMock.Object);
            _controller = new LangileakController(_repoMock.Object);
        }

        [Fact]
        public void LangileakController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Langileak>
            {
                Langilea(1, "ane", "bat"),
                Langilea(2, "iker", "bi")
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<LangileakDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("ane", lista[0].Erabiltzailea);
        }

        [Fact]
        public void LangileakController_Get_LangileaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(Langilea(1, "ane", "sekretua"));

            var result = _controller.Get(1);

            var dto = AssertOkValue<LangileakDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("ane", dto.Erabiltzailea);
            Assert.Equal("sekretua", dto.Pasahitza);
        }

        [Fact]
        public void LangileakController_Get_LangileaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Langileak?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void LangileakController_Login_GorputzaNullDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.Login(null!);

            AssertBadRequestMessage(result, "Erabiltzailea eta pasahitza beharrezkoak dira.");
            _repoMock.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void LangileakController_Login_ErabiltzaileaHutsikDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.Login(new LoginRequest { Erabiltzailea = "", Pasahitza = "sekretua" });

            AssertBadRequestMessage(result, "Erabiltzailea eta pasahitza beharrezkoak dira.");
            _repoMock.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void LangileakController_Login_PasahitzaHutsikDenean_BadRequestObjectResultItzultzenDu()
        {
            var result = _controller.Login(new LoginRequest { Erabiltzailea = "ane", Pasahitza = "" });

            AssertBadRequestMessage(result, "Erabiltzailea eta pasahitza beharrezkoak dira.");
            _repoMock.Verify(r => r.GetAll(), Times.Never);
        }

        [Fact]
        public void LangileakController_Login_KredentzialOkerrekin_UnauthorizedObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Langileak>
            {
                Langilea(1, "ane", "sekretua")
            });

            var result = _controller.Login(new LoginRequest { Erabiltzailea = "ane", Pasahitza = "okerra" });

            AssertUnauthorizedMessage(result, "Erabiltzailea edo pasahitza oker");
        }

        [Fact]
        public void LangileakController_Login_KredentzialZuzenekin_OkObjectResultItzultzenDu()
        {
            var langilea = Langilea(1, "ANE", " sekretua ");
            _repoMock.Setup(r => r.GetAll()).Returns(new List<Langileak> { langilea });

            var result = _controller.Login(new LoginRequest { Erabiltzailea = "ane", Pasahitza = "sekretua" });

            var dto = AssertOkValue<LangileakDto>(result);
            Assert.Equal(langilea.Id, dto.Id);
            Assert.Equal(langilea.Erabiltzailea, dto.Erabiltzailea);
            Assert.Null(dto.Pasahitza);
        }

        [Fact]
        public void LangileakController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = LangileaSortuDto();
            Langileak? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Langileak>()))
                .Callback<Langileak>(l =>
                {
                    l.Id = 7;
                    gordeta = l;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Langilea sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.Erabiltzailea, gordeta.Erabiltzailea);
            Assert.Equal(dto.TxatBaimena, gordeta.TxatBaimena);
        }

        [Fact]
        public void LangileakController_Eguneratu_LangileaDagoenean_OkObjectResultItzultzenDu()
        {
            var langilea = Langilea(1, "zaharra", "zaharra");
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            var dto = LangileaSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.Izena, langilea.Izena);
            Assert.Equal(dto.Erabiltzailea, langilea.Erabiltzailea);
            Assert.Equal(dto.Pasahitza, langilea.Pasahitza);
            Assert.Equal(dto.Aktibo, langilea.Aktibo);
            Assert.Equal(dto.RolaId, langilea.RolaId);
            Assert.Equal(dto.TxatBaimena, langilea.TxatBaimena);
            _repoMock.Verify(r => r.Update(langilea), Times.Once);
        }

        [Fact]
        public void LangileakController_Eguneratu_LangileaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Langileak?)null);

            var result = _controller.Eguneratu(99, LangileaSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<Langileak>()), Times.Never);
        }

        [Fact]
        public void LangileakController_Ezabatu_LangileaDagoenean_OkObjectResultItzultzenDu()
        {
            var langilea = Langilea(1, "ane", "sekretua");
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(langilea), Times.Once);
        }

        [Fact]
        public void LangileakController_Ezabatu_LangileaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((Langileak?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<Langileak>()), Times.Never);
        }
    }
}
