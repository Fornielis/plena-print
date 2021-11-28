using System;
using System.Data;

namespace MinhaPre.Data
{
    public class Sistema
    {
        _MySql mySql = new _MySql();
        public EmailLista Email_DadosPorTipo(string tipo)
        {
            var emailLista = new EmailLista();

            mySql.LimparParametros();
            mySql.AdicionarParametro("varTipo", tipo);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "email_DadosPorTipo");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var email = new Email();
                email.IdEmail = Convert.ToInt32(dataRow["IdEmail"]);
                email.E_mail = Convert.ToString(dataRow["Email"]);
                email.Senha = Convert.ToString(dataRow["Senha"]);
                email.Tipo = Convert.ToString(dataRow["Senha"]);
                email.SMTP = Convert.ToString(dataRow["SMTP"]);
                email.Porta = Convert.ToInt32(dataRow["Porta"]);
                emailLista.Add(email);
            }
            return emailLista;
        }
        public void Sistema_Abre()
        {
            DateTime dataAtual = DateTime.Now;
            mySql.LimparParametros();
            mySql.AdicionarParametro("varAbertura", dataAtual);

            mySql.Persistir(CommandType.StoredProcedure, "sistema_Abre");
        }
        public SistemaAbertura Sistema_AberturaPorDia()
        {
            string data = DateTime.Now.ToString("yyyy/MM/dd");
            DateTime dataAtual = Convert.ToDateTime(data);
            var sistemaAbertura = new SistemaAbertura();

            mySql.LimparParametros();
            mySql.AdicionarParametro("varDataAtual", dataAtual);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "sistema_AberturaPorDia");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                sistemaAbertura.Id = Convert.ToInt32(dataRow["Id"]);

                if (sistemaAbertura.Id > 0)
                {
                    sistemaAbertura.Abertura = Convert.ToDateTime(dataRow["Abertura"]);
                    sistemaAbertura.Fechamento = Convert.ToDateTime(dataRow["Fechamento"]);
                }
                sistemaAbertura.TotalMinutos = Convert.ToInt32(dataRow["TotalMinutos"]);
            }

            return sistemaAbertura;
        }
        public SistemaAbertura Sistema_Fecha()
        {
            string data = DateTime.Now.ToString("yyyy/MM/dd");
            DateTime dataPesquisa = Convert.ToDateTime(data);

            var sistemaAbertura = Sistema_AberturaPorDia();

            DateTime dataFechamento = DateTime.Now;

            
            TimeSpan minutos = (dataFechamento.Subtract(sistemaAbertura.Abertura));
            sistemaAbertura.TotalMinutos = Convert.ToInt32(minutos.TotalMinutes);

            mySql.LimparParametros();
            mySql.AdicionarParametro("varId", sistemaAbertura.Id);
            mySql.AdicionarParametro("varFechamento", dataFechamento);
            mySql.AdicionarParametro("varTotalMinutos", sistemaAbertura.TotalMinutos);
            mySql.Consultar(CommandType.StoredProcedure, "sistema_Fecha");

      
            

            return sistemaAbertura;
        }
    }
}
