using System;
using System.Data;
using System.Linq;


namespace MinhaPre.Data
{
    public class PreImpressao
    {
        _MySql mySql = new _MySql();
        public int Preimpressao_novaOS(OS os)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varNumero", os.Numero);
            mySql.AdicionarParametro("varCliente", os.Cliente);
            mySql.AdicionarParametro("varMaterial", os.Material);
            mySql.AdicionarParametro("varPrazoData", os.PrazoData);
            mySql.AdicionarParametro("varProva", os.Prova);
            mySql.AdicionarParametro("varProvaEstatus", "AGURDANDO");
            mySql.AdicionarParametro("varImpressora", os.Impressora);
            mySql.AdicionarParametro("varOsEstatus", "NEW");
            mySql.AdicionarParametro("varEstatusOrcamento", os.EstatusOrcamento);

            if (os.IdMaterialTipo == 0)
            {
                mySql.AdicionarParametro("varIdMaterialTipo", 1);
            }
            else
            {
                mySql.AdicionarParametro("varIdMaterialTipo", os.IdMaterialTipo);
            }

            mySql.AdicionarParametro("varObservacao", os.Observacao);

            int retorno = Convert.ToInt32(mySql.Persistir(CommandType.StoredProcedure, "pre_NovaOs"));
            return retorno;
        }
        public void Pre_NovoHistorico(Historico historico)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", historico.IdOS);
            mySql.AdicionarParametro("varIdUsuario", historico.IdUsuario);
            mySql.AdicionarParametro("varUsuario", historico.Usuario);
            mySql.AdicionarParametro("varOsEstatus", historico.OsEstatus);
            mySql.AdicionarParametro("varDescricao", historico.Descricao);
            mySql.AdicionarParametro("varDataInicio", historico.DataInicio);
            mySql.AdicionarParametro("varDataFim", historico.DataFim);
            mySql.AdicionarParametro("varMinutoAcumulado", historico.MinutoAcumulado);
            mySql.AdicionarParametro("varMinutoTotal", historico.MinutoTotal);

            mySql.Persistir(CommandType.StoredProcedure, "pre_NovoHistorico");
        }
        public void Pre_AtualizaHistorico(Historico historico)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdHistorico", historico.IdHistorico);
            mySql.AdicionarParametro("varDataFim", historico.DataFim);
            mySql.AdicionarParametro("varMinutoTotal", historico.MinutoTotal);

            mySql.Persistir(CommandType.StoredProcedure, "pre_AtualizaHistorico");
        }
        public HistoricoLista Pre_HistoricoPorOs(int idOs)
        {
            var historicoLista = new HistoricoLista();

            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", idOs);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_HistoricoPorOs");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var historico = new Historico();
                historico.IdHistorico = Convert.ToInt32(dataRow["IdHistorico"]);
                historico.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                historico.IdUsuario = Convert.ToInt32(dataRow["IdUsuario"]);
                historico.Usuario = Convert.ToString(dataRow["Usuario"]);
                historico.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                historico.Descricao = Convert.ToString(dataRow["Descricao"]);
                historico.DataInicio = Convert.ToDateTime(dataRow["DataInicio"]);
                historico.DataFim = Convert.ToDateTime(dataRow["DataFim"]);
                historico.MinutoAcumulado = Convert.ToInt32(dataRow["MinutoAcumulado"]);
                historico.MinutoTotal = Convert.ToInt32(dataRow["MinutoTotal"]);

                historicoLista.Add(historico);
            }

            return historicoLista;
        }
        public HistoricoLista Pre_HistoricoEmAberto()
        {
            var historicoLista = new HistoricoLista();

            mySql.LimparParametros();
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_HistoricoEmAberto");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var historico = new Historico();
                historico.IdHistorico = Convert.ToInt32(dataRow["IdHistorico"]);
                historico.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                historico.IdUsuario = Convert.ToInt32(dataRow["IdUsuario"]);
                historico.Usuario = Convert.ToString(dataRow["Usuario"]);
                historico.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                historico.Descricao = Convert.ToString(dataRow["Descricao"]);
                historico.DataInicio = Convert.ToDateTime(dataRow["DataInicio"]);
                historico.DataFim = Convert.ToDateTime(dataRow["DataFim"]);
                historico.MinutoAcumulado = Convert.ToInt32(dataRow["MinutoAcumulado"]);
                historico.MinutoTotal = Convert.ToInt32(dataRow["MinutoTotal"]);

                historicoLista.Add(historico);
            }

            return historicoLista;
        }
        public void Pre_AtualizaHistoricoEmAberto()
        {
            // RECUPERA DATA ABERTURA DIA ATUAL
            var dataSistema = new Data.Sistema();
            var sistemaAbertura = dataSistema.Sistema_AberturaPorDia();
            DateTime dataAberturaDiaAtual = sistemaAbertura.Abertura;

            // CAPTURA DATA ATUAL
            DateTime dataAtual = DateTime.Now;        

            var historicoLista = Pre_HistoricoEmAberto();

            foreach (var historico in historicoLista)
            {
                mySql.LimparParametros();

                if (historico.MinutoAcumulado > 0)
                {
                    // CASO HAJA VALOR ACUMULADO
                    // SETA MINUTOS ACUMULADOS DIA ATUAL + VALOR JA ACUMULADO
                    TimeSpan minutosDiaAtual = (dataAtual.Subtract(dataAberturaDiaAtual));
                    int acumuladoDiaAtual = Convert.ToInt32(minutosDiaAtual.TotalMinutes);
                    int acumuladoAtualizado = historico.MinutoAcumulado + acumuladoDiaAtual;
                    mySql.AdicionarParametro("varMinutoAcumulado", acumuladoAtualizado);
                }
                else
                {
                    // NÃO EXISTINDO VALOR ACUMULADO
                    // SETA MINUTOS ACUMULADOS DIA ATUAL
                    TimeSpan minutosDiaAtual = (dataAtual.Subtract(historico.DataInicio));
                    int acumuladoDiaAtual = Convert.ToInt32(minutosDiaAtual.TotalMinutes);
                    int acumuladoAtualizado = historico.MinutoAcumulado + acumuladoDiaAtual;
                    mySql.AdicionarParametro("varMinutoAcumulado", acumuladoAtualizado);
                }
                
                mySql.AdicionarParametro("varIdHistorico", historico.IdHistorico);               
                mySql.Persistir(CommandType.StoredProcedure, "pre_AtualizaHistoricoEmAberto");
            }
        }
        public Historico Pre_HistoricoUltimoPorOs(int idOs)
        {
            var historico = new Historico();

            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", idOs);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_HistoricoUltimoPorOs");

            foreach (DataRow dataRow in dataTable.Rows)
            {               
                historico.IdHistorico = Convert.ToInt32(dataRow["IdHistorico"]);
                historico.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                historico.IdUsuario = Convert.ToInt32(dataRow["IdUsuario"]);
                historico.Usuario = Convert.ToString(dataRow["Usuario"]);
                historico.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                historico.Descricao = Convert.ToString(dataRow["Descricao"]);
                historico.DataInicio = Convert.ToDateTime(dataRow["DataInicio"]);
                historico.DataFim = Convert.ToDateTime(dataRow["DataFim"]);
                historico.MinutoAcumulado = Convert.ToInt32(dataRow["MinutoAcumulado"]);
                historico.MinutoTotal = Convert.ToInt32(dataRow["MinutoTotal"]);
            }

            return historico;
        }
        public void Pre_AtualizaEstatusOs(int IdOS, string Estatus)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varOsEstatus", Estatus);
            mySql.AdicionarParametro("varIdOS", IdOS);
            mySql.Persistir(CommandType.StoredProcedure, "pre_AtualizaEstatusOs");
        }
        public OSLista OsLista()
        {
            var osLista = new OSLista();

            mySql.LimparParametros();
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_ListaOs");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var os = new OS();
                os.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                os.Numero = Convert.ToInt32(dataRow["Numero"]);
                os.Cliente = Convert.ToString(dataRow["Cliente"]);
                os.Material = Convert.ToString(dataRow["Material"]);
                os.PrazoData = Convert.ToString(dataRow["PrazoData"]);
                os.Prova = Convert.ToString(dataRow["Prova"]);
                os.ProvaEstatus = Convert.ToString(dataRow["ProvaEstatus"]);
                os.Impressora = Convert.ToString(dataRow["Impressora"]);
                os.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                os.Observacao = Convert.ToString(dataRow["Observacao"]);
                os.EstatusOrcamento = Convert.ToString(dataRow["EstatusOrcamento"]);
                os.MaterialTipo = Convert.ToString(dataRow["MaterialTipo"]);
                os.IdMaterialTipo = Convert.ToInt32(dataRow["IdMaterialTipo"]);
                osLista.Add(os);
            }

            return osLista;
        }
        public OS Pre_OsPorID(int IdOS)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", IdOS);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_OsPorID");

            var os = new OS();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                os.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                os.Numero = Convert.ToInt32(dataRow["Numero"]);
                os.Cliente = Convert.ToString(dataRow["Cliente"]);
                os.Material = Convert.ToString(dataRow["Material"]);
                os.PrazoData = Convert.ToString(dataRow["PrazoData"]);
                os.Prova = Convert.ToString(dataRow["Prova"]);
                os.ProvaEstatus = Convert.ToString(dataRow["ProvaEstatus"]);
                os.Impressora = Convert.ToString(dataRow["Impressora"]);
                os.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                os.Observacao = Convert.ToString(dataRow["Observacao"]);
                os.EstatusOrcamento = Convert.ToString(dataRow["EstatusOrcamento"]);
                os.MaterialTipo = Convert.ToString(dataRow["MaterialTipo"]);
                os.IdMaterialTipo = Convert.ToInt32(dataRow["IdMaterialTipo"]);
            }

            return os;
        }
        public void Pre_AtualizaEstatusProva(int IdOS, string ProvaEstatus)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varProvaEstatus", ProvaEstatus);
            mySql.AdicionarParametro("varIdOS", IdOS);
            mySql.Persistir(CommandType.StoredProcedure, "pre_AtualizaEstatusProva");
        }
        public void pre_AtualizaOs(OS os)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", os.IdOS);
            mySql.AdicionarParametro("varNumero", os.Numero);
            mySql.AdicionarParametro("varCliente", os.Cliente);
            mySql.AdicionarParametro("varMaterial", os.Material);
            mySql.AdicionarParametro("varPrazoData", os.PrazoData);
            mySql.AdicionarParametro("varProva", os.Prova);
            mySql.AdicionarParametro("varImpressora", os.Impressora);
            mySql.AdicionarParametro("varEstatusOrcamento", os.EstatusOrcamento);
            mySql.AdicionarParametro("varIdMaterialTipo", os.IdMaterialTipo);
            mySql.AdicionarParametro("varObservacao", os.Observacao);

            mySql.Persistir(CommandType.StoredProcedure, "pre_AtualizaOs");
        }
        public void Pre_DeletarOS(int idOS)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varIdOS", idOS);
            mySql.Consultar(CommandType.StoredProcedure, "pre_DeletarOS");
        }
        public OSLista Pre_ListaOsFiltros(OS os)
        {
            var osLista = new OSLista();

            string numero = "";
            if (os.Numero > 0)
            {
                numero = "%" + Convert.ToString(os.Numero) + "%";
            }
            else
            {
                numero = "%%";
            }
            string cliente = "%"+os.Cliente+"%";
            string material = "%"+os.Material+"%";
            string prazoData = "%"+os.PrazoData+"%";
            string impressora = "%"+os.Impressora+"%";
            string estatusOS = "%"+os.OsEstatus+"%";
            string estatusOrcamento = "%" + os.EstatusOrcamento + "%";

            int teste = numero.Count();

            mySql.LimparParametros();
            mySql.AdicionarParametro("varNumero", numero);
            mySql.AdicionarParametro("varCliente", cliente);
            mySql.AdicionarParametro("varMaterial", material);
            mySql.AdicionarParametro("varPrazoData", prazoData);
            mySql.AdicionarParametro("varImpressora", impressora);
            mySql.AdicionarParametro("varOsEstatus", estatusOS);
            mySql.AdicionarParametro("varEstatusOrcamento", estatusOrcamento);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_ListaOsFiltros");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var osFiltrada = new OS();
                osFiltrada.IdOS = Convert.ToInt32(dataRow["IdOS"]);
                osFiltrada.Numero = Convert.ToInt32(dataRow["Numero"]);
                osFiltrada.Cliente = Convert.ToString(dataRow["Cliente"]);
                osFiltrada.Material = Convert.ToString(dataRow["Material"]);
                osFiltrada.PrazoData = Convert.ToString(dataRow["PrazoData"]);
                osFiltrada.Prova = Convert.ToString(dataRow["Prova"]);
                osFiltrada.ProvaEstatus = Convert.ToString(dataRow["ProvaEstatus"]);
                osFiltrada.Impressora = Convert.ToString(dataRow["Impressora"]);
                osFiltrada.OsEstatus = Convert.ToString(dataRow["OsEstatus"]);
                osFiltrada.EstatusOrcamento = Convert.ToString(dataRow["EstatusOrcamento"]);
                osLista.Add(osFiltrada);
            }

            return osLista;
        }
        public int Pre_ChapaGravada(Chapa chapa)
        {

            mySql.LimparParametros();
            mySql.AdicionarParametro("varNumeroOS", chapa.NumeroOS);
            mySql.AdicionarParametro("varFormato", chapa.Formato);
            mySql.AdicionarParametro("varCores", chapa.Cores);
            mySql.AdicionarParametro("varQuantidade", chapa.Quantidade);
            mySql.AdicionarParametro("varRegravacao", chapa.Regravacao);
            mySql.AdicionarParametro("varExterno", chapa.Externo);
            mySql.AdicionarParametro("varMotivo", chapa.Motivo);
            mySql.AdicionarParametro("varMes", chapa.Mes);

            int retorno = Convert.ToInt32(mySql.Persistir(CommandType.StoredProcedure, "pre_ChapaGravada"));
            return retorno;
        }
        public ChapaLista Pre_ChapaPorIdOs(int NumeroOS)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varNumeroOS", NumeroOS);
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_ChapaPorIdOs");

            var chapaLista = new ChapaLista();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var chapa = new Chapa();
                chapa.Id = Convert.ToInt32(dataRow["Id"]);
                chapa.NumeroOS = Convert.ToInt32(dataRow["NumeroOS"]);
                chapa.Formato = Convert.ToString(dataRow["Formato"]);
                chapa.Cores = Convert.ToString(dataRow["Cores"]);
                chapa.Quantidade = Convert.ToInt32(dataRow["Quantidade"]);
                chapa.Regravacao = Convert.ToInt32(dataRow["Regravacao"]);
                chapa.Externo = Convert.ToInt32(dataRow["Externo"]);
                chapaLista.Add(chapa);
            }

            return chapaLista;
        }
        public void Pre_DeletarChapa(int id)
        {
            mySql.LimparParametros();
            mySql.AdicionarParametro("varId", id);
            mySql.Consultar(CommandType.StoredProcedure, "pre_DeletarChapa");
        }
        public MaterialTipoLista MaterialTipoLista()
        {
            var lista = new MaterialTipoLista();

            mySql.LimparParametros();
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_ListaMaterialTipo");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var tipo = new MaterialTipo();
                tipo.Id = Convert.ToInt32(dataRow["Id"]);
                tipo.Material = Convert.ToString(dataRow["MaterialTipo"]);
                lista.Add(tipo);
            }

            return lista;
        }
        public ChapaMotivoLista ChapaMotivo()
        {
            var motivoLista = new ChapaMotivoLista();

            mySql.LimparParametros();
            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "pre_ChapaMotivo");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var motivo = new ChapaMotivo();
                motivo.Id = Convert.ToInt32(dataRow["Id"]);
                motivo.Motivo = Convert.ToString(dataRow["Motivo"]);
                motivoLista.Add(motivo);
            }

            return motivoLista;
        }
    }
}
