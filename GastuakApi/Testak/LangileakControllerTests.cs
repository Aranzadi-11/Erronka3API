using JatetxeaApi.Controllerrak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
        public void P1_GetAllOndo_Ok()
        {
            // Arrange
            var langileak = new List<Langileak>
            {
                new Langileak
                {
                    Id = 1,
                    Izena = "Langile1",
                    Erabiltzailea = "erabiltzailea1",
                    Pasahitza = "pasahitza1",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 1
                },
                new Langileak
                {
                    Id = 2,
                    Izena = "Langile2",
                    Erabiltzailea = "erabiltzailea2",
                    Pasahitza = "pasahitza2",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 2
                }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<LangileakDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void P2_GetAllSalbuespena_InternalServerError()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void P3_LangileaExistitzenDa_Ok()
        {
            // Arrange
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile1",
                Erabiltzailea = "erabiltzailea1",
                Pasahitza = "pasahitza1",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);

            // Act
            var result = _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LangileakDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
        }

        [Fact]
        public void P4_LangileaEzExistitzen_NotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);

            // Act
            var result = _controller.Get(999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P5_GetSalbuespena_InternalServerError()
        {
            // Arrange
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void P6_LoginEskeraBalioezina_BadRequest()
        {
            // Arrange
            LoginRequest loginNull = null;
            var loginEmpty = new LoginRequest { Erabiltzailea = "", Pasahitza = "pass" };
            var loginNoPass = new LoginRequest { Erabiltzailea = "user", Pasahitza = "" };

            // Act & Assert
            var result1 = _controller.Login(loginNull);
            var badRequest1 = Assert.IsType<BadRequestObjectResult>(result1);
            dynamic value1 = badRequest1.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value1.mezua);

            var result2 = _controller.Login(loginEmpty);
            var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
            dynamic value2 = badRequest2.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value2.mezua);

            var result3 = _controller.Login(loginNoPass);
            var badRequest3 = Assert.IsType<BadRequestObjectResult>(result3);
            dynamic value3 = badRequest3.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value3.mezua);
        }

        [Fact]
        public void P7_KredentzialOkerrak_Unauthorized()
        {
            // Arrange
            var langileak = new List<Langileak>
            {
                new Langileak
                {
                    Izena = "Langile1",
                    Erabiltzailea = "erabiltzailea1",
                    Pasahitza = "pasahitza1",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 1
                }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);
            var login = new LoginRequest { Erabiltzailea = "erabiltzailea1", Pasahitza = "wrongpass" };

            // Act
            var result = _controller.Login(login);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            dynamic value = unauthorized.Value;
            Assert.Equal("Erabiltzailea edo pasahitza oker", value.mezua);
        }

        [Fact]
        public void P8_KredentzialZuzenak_Ok()
        {
            // Arrange
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile1",
                Erabiltzailea = "erabiltzailea1",
                Pasahitza = "pasahitza1",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            var langileak = new List<Langileak> { langilea };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);
            var login = new LoginRequest { Erabiltzailea = "erabiltzailea1", Pasahitza = "pasahitza1" };

            // Act
            var result = _controller.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LangileakDto>(okResult.Value);
            Assert.Equal(langilea.Id, dto.Id);
        }

        [Fact]
        public void P9_LoginSalbuespena_InternalServerError()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));
            var login = new LoginRequest { Erabiltzailea = "user", Pasahitza = "pass" };

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.Login(login));
        }

        [Fact]
        public void P10_LangileaSortuOndo_Ok()
        {
            // Arrange
            var dto = new LangileakSortuDto
            {
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",              // Aldaketa: bool -> string
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            Langileak savedLangile = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Langileak>())).Callback<Langileak>(l => savedLangile = l);

            // Act
            var result = _controller.Sortu(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Langilea sortuta", value.mezua);
            Assert.NotNull(savedLangile);
        }

        [Fact]
        public void P11_SortuSalbuespena_InternalServerError()
        {
            // Arrange
            var dto = new LangileakSortuDto
            {
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",              // Aldaketa: bool -> string
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Add(It.IsAny<Langileak>())).Throws(new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void P12_LangileaEzExistitzen_NotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);
            var dto = new LangileakSortuDto();  // DTO hutsa, ez da erabiliko

            // Act
            var result = _controller.Eguneratu(999, dto);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P13_LangileaEguneratuOndo_Ok()
        {
            // Arrange
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            var dto = new LangileakSortuDto
            {
                Izena = "LangileUpdated",
                Erabiltzailea = "userUpdated",
                Pasahitza = "passUpdated",
                Aktibo = "Ez",               // Aldaketa: bool -> string
                ErregistroData = DateTime.Now,
                RolaId = 2
            };

            // Act
            var result = _controller.Eguneratu(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            _repoMock.Verify(r => r.Update(langilea), Times.Once);
        }

        [Fact]
        public void P14_EguneratuSalbuespena_InternalServerError()
        {
            // Arrange
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            _repoMock.Setup(r => r.Update(It.IsAny<Langileak>())).Throws(new Exception("Database error"));
            var dto = new LangileakSortuDto();   // DTO hutsa, ez da erabiliko

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void P15_LangileaEzExistitzen_NotFound_Ezabatu()
        {
            // Arrange
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);

            // Act
            var result = _controller.Ezabatu(999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P16_LangileaEzabatuOndo_Ok()
        {
            // Arrange
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);

            // Act
            var result = _controller.Ezabatu(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _repoMock.Verify(r => r.Delete(langilea), Times.Once);
        }

        [Fact]
        public void P17_EzabatuSalbuespena_InternalServerError()
        {
            // Arrange
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            _repoMock.Setup(r => r.Delete(It.IsAny<Langileak>())).Throws(new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}