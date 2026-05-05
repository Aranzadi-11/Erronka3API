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
    public class JatetxekoInfoControllerTest
    {
        private readonly Mock<JatetxekoInfoRepository> _repoMock;
        private readonly JatetxekoInfoController _controller;

        public JatetxekoInfoControllerTest()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<JatetxekoInfoRepository>(sessionFactoryMock.Object);
            _controller = new JatetxekoInfoController(_repoMock.Object);
        }

        [Fact]
        public void JatetxekoInfoController_GetAll_Zerrenda_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<JatetxekoInfo>
            {
                JatetxekoInfo(1),
                JatetxekoInfo(2)
            });

            var result = _controller.GetAll();

            var lista = AssertOkEnumerable<JatetxekoInfoDto>(result);
            Assert.Equal(2, lista.Count);
            Assert.Equal("Jatetxea", lista[0].Izena);
        }

        [Fact]
        public void JatetxekoInfoController_Get_InformazioaDagoenean_OkObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(1)).Returns(JatetxekoInfo(1));

            var result = _controller.Get(1);

            var dto = AssertOkValue<JatetxekoInfoDto>(result);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Jatetxea", dto.Izena);
        }

        [Fact]
        public void JatetxekoInfoController_Get_InformazioaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((JatetxekoInfo?)null);

            var result = _controller.Get(99);

            AssertNotFoundMessage(result);
        }

        [Fact]
        public void JatetxekoInfoController_Sortu_DatuBaliozkoekin_OkObjectResultItzultzenDu()
        {
            var dto = JatetxekoInfoSortuDto();
            JatetxekoInfo? gordeta = null;
            _repoMock.Setup(r => r.Add(It.IsAny<JatetxekoInfo>()))
                .Callback<JatetxekoInfo>(j =>
                {
                    j.Id = 7;
                    gordeta = j;
                });

            var result = _controller.Sortu(dto);

            var ok = AssertOk(result);
            Assert.Equal("Informazioa sortuta", Property<string>(ok.Value!, "mezua"));
            Assert.Equal(7, Property<int>(ok.Value!, "id"));
            Assert.NotNull(gordeta);
            Assert.Equal(dto.Izena, gordeta.Izena);
            Assert.Equal(dto.KaxaTotal, gordeta.KaxaTotal);
        }

        [Fact]
        public void JatetxekoInfoController_Eguneratu_InformazioaDagoenean_OkObjectResultItzultzenDu()
        {
            var info = JatetxekoInfo(1);
            _repoMock.Setup(r => r.Get(1)).Returns(info);
            var dto = JatetxekoInfoSortuDto();

            var result = _controller.Eguneratu(1, dto);

            AssertOkMessage(result, "Eguneratuta");
            Assert.Equal(dto.Izena, info.Izena);
            Assert.Equal(dto.KaxaTotal, info.KaxaTotal);
            Assert.Equal(dto.Helbidea, info.Helbidea);
            Assert.Equal(dto.TelefonoZenbakia, info.TelefonoZenbakia);
            _repoMock.Verify(r => r.Update(info), Times.Once);
        }

        [Fact]
        public void JatetxekoInfoController_Eguneratu_InformazioaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((JatetxekoInfo?)null);

            var result = _controller.Eguneratu(99, JatetxekoInfoSortuDto());

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Update(It.IsAny<JatetxekoInfo>()), Times.Never);
        }

        [Fact]
        public void JatetxekoInfoController_Ezabatu_InformazioaDagoenean_OkObjectResultItzultzenDu()
        {
            var info = JatetxekoInfo(1);
            _repoMock.Setup(r => r.Get(1)).Returns(info);

            var result = _controller.Ezabatu(1);

            AssertOkMessage(result, "Ezabatuta");
            _repoMock.Verify(r => r.Delete(info), Times.Once);
        }

        [Fact]
        public void JatetxekoInfoController_Ezabatu_InformazioaEzDagoenean_NotFoundObjectResultItzultzenDu()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((JatetxekoInfo?)null);

            var result = _controller.Ezabatu(99);

            AssertNotFoundMessage(result);
            _repoMock.Verify(r => r.Delete(It.IsAny<JatetxekoInfo>()), Times.Never);
        }
    }
}
