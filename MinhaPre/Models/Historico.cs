using System;

namespace MinhaPre
{
    public class Historico
    {
        public int IdHistorico { get; set; }
        public int IdOS { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string OsEstatus { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int MinutoAcumulado { get; set; }
        public int MinutoTotal { get; set; }
    }
}
