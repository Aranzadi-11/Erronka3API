using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace JatetxeaApi.Testak.Controllerrak
{
    internal static class ControllerTestHelpers
    {
        public static T Property<T>(object value, string propertyName)
        {
            var property = value.GetType().GetProperty(propertyName);
            Assert.NotNull(property);
            return (T)property.GetValue(value)!;
        }

        public static OkObjectResult AssertOk(IActionResult result)
        {
            return Assert.IsType<OkObjectResult>(result);
        }

        public static T AssertOkValue<T>(IActionResult result)
        {
            var ok = AssertOk(result);
            return Assert.IsType<T>(ok.Value);
        }

        public static List<T> AssertOkEnumerable<T>(IActionResult result)
        {
            var ok = AssertOk(result);
            return Assert.IsAssignableFrom<IEnumerable<T>>(ok.Value).ToList();
        }

        public static void AssertOkMessage(IActionResult result, string expected)
        {
            var ok = AssertOk(result);
            Assert.Equal(expected, Property<string>(ok.Value!, "mezua"));
        }

        public static void AssertNotFoundMessage(IActionResult result, string expected = "Ez da aurkitu")
        {
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(expected, Property<string>(notFound.Value!, "mezua"));
        }

        public static void AssertBadRequestMessage(IActionResult result, string expected)
        {
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expected, Property<string>(badRequest.Value!, "mezua"));
        }

        public static void AssertUnauthorizedMessage(IActionResult result, string expected)
        {
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(expected, Property<string>(unauthorized.Value!, "mezua"));
        }

        public static Erreserbak Erreserba(int id = 1, int mahaiaId = 1, DateTime? erreserbaData = null, string egoera = "Itxaropean")
        {
            return new Erreserbak
            {
                Id = id,
                MahaiaId = mahaiaId,
                Izena = $"Erreserba{id}",
                Telefonoa = 600000000 + id,
                ErreserbaData = erreserbaData ?? new DateTime(2026, 5, 4, 14, 30, 0),
                PertsonaKop = 2 + id,
                Egoera = egoera,
                Oharrak = "Oharra"
            };
        }

        public static ErreserbakSortuDto ErreserbaSortuDto()
        {
            return new ErreserbakSortuDto
            {
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 600111222,
                ErreserbaData = new DateTime(2026, 5, 4, 14, 30, 0),
                PertsonaKop = 4,
                Egoera = "Baieztatuta",
                Oharrak = "Leiho ondoan"
            };
        }

        public static Inbentarioa InbentarioaElementua(int id = 1, string izena = "Arroza")
        {
            return new Inbentarioa
            {
                Id = id,
                Izena = izena,
                Deskribapena = "Deskribapena",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = new DateTime(2026, 5, 1)
            };
        }

        public static InbentarioaSortuDto InbentarioaSortuDto()
        {
            return new InbentarioaSortuDto
            {
                Izena = "Irina",
                Deskribapena = "Garia",
                Kantitatea = 20,
                NeurriaUnitatea = "kg",
                StockMinimoa = 5
            };
        }

        public static JatetxekoInfo JatetxekoInfo(int id = 1)
        {
            return new JatetxekoInfo
            {
                Id = id,
                Izena = "Jatetxea",
                KaxaTotal = 100m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 943000000
            };
        }

        public static JatetxekoInfoSortuDto JatetxekoInfoSortuDto()
        {
            return new JatetxekoInfoSortuDto
            {
                Izena = "Jatetxe berria",
                KaxaTotal = 200m,
                Helbidea = "Kalea 2",
                TelefonoZenbakia = 943111111
            };
        }

        public static Kategoria Kategoria(int id = 1, string izena = "Edariak")
        {
            return new Kategoria { Id = id, Izena = izena };
        }

        public static KategoriaSortuDto KategoriaSortuDto(string izena = "Postreak")
        {
            return new KategoriaSortuDto { Izena = izena };
        }

        public static Langileak Langilea(int id = 1, string erabiltzailea = "ane", string pasahitza = "sekretua")
        {
            return new Langileak
            {
                Id = id,
                Izena = $"Langilea{id}",
                Erabiltzailea = erabiltzailea,
                Pasahitza = pasahitza,
                Aktibo = "Bai",
                ErregistroData = new DateTime(2026, 5, 1),
                RolaId = 1,
                TxatBaimena = true
            };
        }

        public static LangileakSortuDto LangileaSortuDto()
        {
            return new LangileakSortuDto
            {
                Izena = "Langile berria",
                Erabiltzailea = "berria",
                Pasahitza = "pasahitz berria",
                Aktibo = "Bai",
                ErregistroData = new DateTime(2026, 5, 2),
                RolaId = 2,
                TxatBaimena = false
            };
        }

        public static Mahaiak Mahaia(int id = 1, int mahaiaZbk = 1)
        {
            return new Mahaiak
            {
                Id = id,
                MahaiaZbk = mahaiaZbk,
                Edukiera = 4,
                Egoera = "Libre"
            };
        }

        public static MahaiakSortuDto MahaiaSortuDto()
        {
            return new MahaiakSortuDto
            {
                MahaiaZbk = 10,
                Edukiera = 6,
                Egoera = "Libre"
            };
        }

        public static Platerak Platera(int id = 1, string izena = "Entsalada")
        {
            return new Platerak
            {
                Id = id,
                Izena = izena,
                Deskribapena = "Freskoa",
                Prezioa = 12.5m,
                KategoriaId = 1,
                Erabilgarri = "Bai",
                SortzeData = new DateTime(2026, 5, 1),
                Irudia = "irudia.png"
            };
        }

        public static PlaterakSortuDto PlateraSortuDto()
        {
            return new PlaterakSortuDto
            {
                Izena = "Platera berria",
                Deskribapena = "Deskribapena",
                Prezioa = 15m,
                KategoriaId = 2,
                Erabilgarri = "Bai",
                Irudia = "berria.png"
            };
        }

        public static PlaterenOsagaiak PlaterenOsagaia(int plateraId = 1, int inbentarioaId = 2)
        {
            return new PlaterenOsagaiak
            {
                PlateraId = plateraId,
                InbentarioaId = inbentarioaId,
                Kantitatea = 3m
            };
        }

        public static Rolak Rola(int id = 1, string izena = "Admin")
        {
            return new Rolak { Id = id, Izena = izena };
        }

        public static RolakSortuDto RolaSortuDto(string izena = "Zerbitzaria")
        {
            return new RolakSortuDto { Izena = izena };
        }

        public static Zerbitzuak Zerbitzua(int id = 1, int erreserbaId = 1)
        {
            return new Zerbitzuak
            {
                Id = id,
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = erreserbaId,
                EskaeraData = new DateTime(2026, 5, 4, 14, 30, 0),
                Egoera = "Eskatuta",
                Guztira = 25m
            };
        }

        public static ZerbitzuakSortuDto ZerbitzuaSortuDto()
        {
            return new ZerbitzuakSortuDto
            {
                LangileId = 3,
                MahaiaId = 4,
                ErreserbaId = 5,
                EskaeraData = new DateTime(2026, 5, 4, 15, 0, 0),
                Egoera = "Amaituta",
                Guztira = 40m
            };
        }

        public static ZerbitzuXehetasunak ZerbitzuXehetasuna(int id = 1, int zerbitzuaId = 1)
        {
            return new ZerbitzuXehetasunak
            {
                Id = id,
                ZerbitzuaId = zerbitzuaId,
                PlateraId = 2,
                Kantitatea = 3,
                PrezioUnitarioa = 9.5m,
                Zerbitzatuta = false
            };
        }

        public static ZerbitzuXehetasunakSortuDto ZerbitzuXehetasunaSortuDto()
        {
            return new ZerbitzuXehetasunakSortuDto
            {
                ZerbitzuaId = 2,
                PlateraId = 3,
                Kantitatea = 4,
                PrezioUnitarioa = 11m,
                Zerbitzatuta = true
            };
        }
    }
}
