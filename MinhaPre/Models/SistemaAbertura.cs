using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MinhaPre
{
    public class SistemaAbertura
    {
        public int Id { get; set; }
        public DateTime Abertura { get; set; }
        public DateTime Fechamento { get; set; }
        public int TotalMinutos { get; set; }
    }
}