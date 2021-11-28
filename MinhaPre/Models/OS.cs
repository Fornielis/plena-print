using System;

namespace MinhaPre
{
    public class OS
    {
        public int IdOS { get; set; }
        public int Numero { get; set; }
        public string Cliente { get; set; }
        public string Material { get; set; }
        public string PrazoData { get; set; }
        public string[] Provas { get; set; }
        public string Prova { get; set; }
        public string ProvaEstatus { get; set; }
        public string[] Impressoras { get; set; }
        public string Impressora { get; set; }
        public string OsEstatus { get; set; }
        public string Observacao { get; set; }
        public string EstatusOrcamentoJaSalvo { get; set; }
        public string EstatusOrcamento { get; set; }
        public int IdMaterialTipo { get; set; }
        public string MaterialTipo { get; set; }
    }
}
