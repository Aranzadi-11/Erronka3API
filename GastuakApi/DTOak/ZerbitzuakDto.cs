using System;
using System.Collections.Generic;

namespace JatetxeaApi.DTOak
{
    public class ZerbitzuakDto
    {
        public int Id { get; set; }
        public int LangileId { get; set; }
        public int MahaiaId { get; set; }
        public int? ErreserbaId { get; set; }
        public DateTime? EskaeraData { get; set; }
        public string Egoera { get; set; }
        public decimal? Guztira { get; set; }
    }

    public class ZerbitzuakSortuDto
    {
        public int LangileId { get; set; }
        public int MahaiaId { get; set; }
        public int? ErreserbaId { get; set; }
        public DateTime? EskaeraData { get; set; }
        public string Egoera { get; set; } = "Itxaropean";
        public decimal? Guztira { get; set; }
    }

    public class PlateraEskariaDto
    {
        public int PlateraId { get; set; }
        public int Kantitatea { get; set; }
    }

    public class ZerbitzuaEskariaDto
    {
        public int LangileId { get; set; }
        public int MahaiaId { get; set; }
        public int ErreserbaId { get; set; }
        public DateTime EskaeraData { get; set; }
        public List<PlateraEskariaDto> Platerak { get; set; } = new();
    }
}
