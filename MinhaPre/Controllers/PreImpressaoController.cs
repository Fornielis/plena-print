using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MinhaPre.Controllers
{
    public class PreImpressaoController : Controller
    {
        // MÉTODOS
        public string[] Estatus()
        {
            string[] listaEstatus = { "PEN", "PRE", "PRO", "APR", "ENG", "MON", "CTP", "DIG", "FIM" };
            return listaEstatus;
        }
        public string DescricaoHistorico(string estatusAnterior, string estatusAtual)
        {
            string Descricao = "";

            switch (estatusAtual)
            {
                case "PEN":
                    Descricao = "JOB com divergências.";
                    break;
                case "PRE":
                    Descricao = "JOB em processo de análize na Pré.";
                    break;
                case "PRO":
                    Descricao = "Processo de confecção das provas.";
                    break;
                case "APR":
                    Descricao = "JOB encaminhado para aprovação.";
                    break;
                case "ENG":
                    Descricao = "JOB aprovado encaminhado para engenharia.";
                    break;
                case "MON":
                    Descricao = "JOB encaminhado para processo de montagem.";
                    break;
                case "CTP":
                    Descricao = "Processo gração de chapas.";
                    break;
                case "DIG":
                    Descricao = "JOB encaminhado para impressão digital";
                    break;
                case "FIM":
                    if (estatusAnterior == "CTP")
                    {
                        Descricao = "Chapas foram gravadas.";
                    }
                    else if (estatusAnterior == "DIG")
                    {
                        Descricao = "Impressão digital concluída.";
                    }
                    break;
            }

            return Descricao;
        }
        public void NovoHistorico(int idOs, string osEstatus, string descricao)
        {
            // INSTÂNCIA DAT PRÉ IMPRESSÃO
            var data = new Data.PreImpressao();

            // INSTÂNCIA HISTORIO - POPULA DADOS
            var historico = new Historico();
            historico.IdOS = idOs;
            historico.IdUsuario = Convert.ToInt32(Session["IdUsuario"]);
            historico.Usuario = Session["Nome"].ToString();
            historico.OsEstatus = osEstatus;
            historico.Descricao = descricao;
            historico.DataInicio = DateTime.Now;

            // RESGATA E VERIFICA ULTIMO HISTORIVO 
            var ultimoHistorico = data.Pre_HistoricoUltimoPorOs(idOs);
            if (ultimoHistorico.IdHistorico > 0)
            {
                // CASO JÁ EXISTA HISTORICOS ANTERIORES
                // POPULA DADOS ULTIMO HISTORICO PARA FINALIZAR
                ultimoHistorico.DataFim = DateTime.Now;

                // VERIFICA SE EXISTEM MINUTOS ACUMULADOS
                if (ultimoHistorico.MinutoAcumulado > 0)
                {
                    // EXISTINDO VALOR ACUMULADO 
                    // RECUPERA DATA ABERTURA DIA ATUAL
                    var dataSistema = new Data.Sistema();
                    var sistemaAbertura = dataSistema.Sistema_AberturaPorDia();
                    DateTime dataAberturaDiaAtual = sistemaAbertura.Abertura;

                    // ATUALIZA VALOR ACUMULADO
                    TimeSpan minutosAcumulados = (ultimoHistorico.DataFim.Subtract(dataAberturaDiaAtual));
                    ultimoHistorico.MinutoAcumulado += Convert.ToInt32(minutosAcumulados.TotalMinutes);
                    ultimoHistorico.MinutoTotal += ultimoHistorico.MinutoAcumulado;
                    ultimoHistorico.MinutoAcumulado = 0;
                }
                else
                {
                    // SEM MINUTOS ACUMULADOS
                    // OBTEM MINUTOS ENTRE DATA INICIO E FIM
                    TimeSpan minutosTotal = (ultimoHistorico.DataFim.Subtract(ultimoHistorico.DataInicio));
                    ultimoHistorico.MinutoTotal = Convert.ToInt32(minutosTotal.TotalMinutes) + ultimoHistorico.MinutoAcumulado;
                    if (ultimoHistorico.MinutoTotal == 0)
                    {
                        ultimoHistorico.MinutoTotal += 1;
                    }
                }               

                data.Pre_AtualizaHistorico(ultimoHistorico);
            }
            else
            {
                // CASO SEJA O PRIMEIRO HISTORICO
                historico.MinutoAcumulado = 0;
                historico.MinutoTotal = 0;
            }

            // GRAVA HISTORICO NO BANCO
            data.Pre_NovoHistorico(historico);
        }

        //CONTROLERS
        public ActionResult Lista()
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                var listaOs = data.OsLista();
                ViewBag.Estatus = Estatus();
                return PartialView("_ListaOS", listaOs);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult MaisOS()
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var os = new OS();
                var data = new Data.PreImpressao();
                ViewBag.MaterialTipoLista = data.MaterialTipoLista();

                return PartialView("_MaisOS", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        [HttpPost]
        public ActionResult MaisOS(OS os)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                if (os.Provas == null)
                {
                    return PartialView("~/Views/PreImpressao/_CamposObrigatorios.cshtml");
                }

                // VETOR DO FORMULARIO TRANSFORMADO EM TEXTOS
                var funcaoVetor = new Funcoes.Vetores();
                os.Prova = funcaoVetor.VetorComoTexto(os.Provas);

                if (!(os.Impressoras == null))
                {
                    os.Impressora = funcaoVetor.VetorComoTexto(os.Impressoras);
                }

                var data = new Data.PreImpressao();

                int idNovaOs = data.Preimpressao_novaOS(os);

                NovoHistorico(idNovaOs, "NEW", "Criou nova OS sistema.");

                if (os.EstatusOrcamento == "OK")
                {
                    os = data.Pre_OsPorID(idNovaOs);

                    var bllsistema = new Data.Sistema();
                    var emailLista = bllsistema.Email_DadosPorTipo("SYS");
                    var funcaoEmail = new FuncaoEmail();

                    funcaoEmail.EnviarEmailComercial(os);
                }

                return RedirectToAction("Lista");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }
        }
        public ActionResult OsPorID(int IdOS)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();

                var os = data.Pre_OsPorID(IdOS);

                ViewBag.Estatus = Estatus();

                ViewBag.Historico = data.Pre_HistoricoPorOs(IdOS);

                return PartialView("_OsPorID", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult AtualizaEstatus(int IdOS, string EstatusAnterior, string EstatusAtual, bool redirectLista)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                // INSTANCIAL DATA PRE IMPRESSAO
                var data = new Data.PreImpressao();

                // ATUALIZA ESTATUS OS TABELA OS
                data.Pre_AtualizaEstatusOs(IdOS, EstatusAtual);

                // POPULA OS COM DADOS ATUALIZADOS
                var os = data.Pre_OsPorID(IdOS);

                // PROCESSO GERAÇÃO HISTORICO
                var historico = new Historico();
                string descricao = DescricaoHistorico(EstatusAnterior, EstatusAtual); // metodo obtem descrição
                NovoHistorico(os.IdOS, EstatusAtual, descricao); // metodo para gravar novo historico

                // CARREGA LISTA DE ESTATUS
                ViewBag.Estatus = Estatus();

                // RESGATA LISTA HISTORICO DA OS
                ViewBag.Historico = data.Pre_HistoricoPorOs(os.IdOS);

                // RESULTADO QUANDO AS CHAPAS GRAVADAS ESTIVEREM OK
                if (redirectLista == true)
                {
                    return RedirectToAction("Lista");
                }

                // RESULTADO PARA QUALQUER ESTATUS EXCERO - FIM
                return PartialView("_Povas_Estatus_Historico", os);

            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }
        }
        [HttpPost]
        public ActionResult AtualizaProvas(OS os)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();

                data.Pre_AtualizaEstatusProva(os.IdOS, os.ProvaEstatus);

                os = data.Pre_OsPorID(os.IdOS);
                ViewBag.Estatus = Estatus();

                ViewBag.Historico = data.Pre_HistoricoPorOs(os.IdOS);

                return PartialView("_Povas_Estatus_Historico", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult EditarOS(int IdOS)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                var os = data.Pre_OsPorID(IdOS);
                ViewBag.MaterialTipoLista = data.MaterialTipoLista();
                return PartialView("_EditarOS", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        [HttpPost]
        public ActionResult EditarOS(OS os)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var funcoes = new Funcoes.Vetores();
                if (os.Impressoras != null)
                {
                    os.Impressora = funcoes.VetorComoTexto(os.Impressoras);
                }
                if (os.Provas != null)
                {
                    os.Prova = funcoes.VetorComoTexto(os.Provas);
                }

                var data = new Data.PreImpressao();

                data.pre_AtualizaOs(os);

                ViewBag.Estatus = Estatus();

                ViewBag.Historico = data.Pre_HistoricoPorOs(os.IdOS);

                if (os.EstatusOrcamento == "OK" && os.EstatusOrcamentoJaSalvo == "Divergente")
                {
                    var bllsistema = new Data.Sistema();
                    var email = bllsistema.Email_DadosPorTipo("SYS");
                    var funcaoEmail = new FuncaoEmail();
                    funcaoEmail.EnviarEmailComercial(os);
                }

                os = data.Pre_OsPorID(os.IdOS);
                return PartialView("_OsPorID", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult DeletarOS(int IdOS)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                data.Pre_DeletarOS(IdOS);
                return RedirectToAction("Lista");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult Filtros()
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                var os = new OS();
                ViewBag.MaterialTipoLista = data.MaterialTipoLista();
                ViewBag.Estatus = Estatus();
                return PartialView("_Filtros", os);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        [HttpPost]
        public ActionResult Filtros(OS os)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                var listaOs = data.Pre_ListaOsFiltros(os);
                ViewBag.Estatus = Estatus();
                return PartialView("_ListaOS", listaOs);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult ChapasGravadas(int NumeroOS, int IdOS)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();

                var chapa = new Chapa();
                chapa.NumeroOS = NumeroOS;

                var listaChapas = new ChapaLista();
                listaChapas = data.Pre_ChapaPorIdOs(NumeroOS);

                int totalItensLista = listaChapas.Count();

                ViewBag.IdOS = IdOS;
                ViewBag.TotalItensLista = totalItensLista;
                ViewBag.ChapasGravasLista = listaChapas;

                return PartialView("_Chapas", chapa);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        [HttpPost]
        public ActionResult ChapasGravadas(Chapa chapa)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();

                int retorno = data.Pre_ChapaGravada(chapa);

                var listaChapas = new ChapaLista();
                listaChapas = data.Pre_ChapaPorIdOs(chapa.NumeroOS);

                int totalItensLista = listaChapas.Count();

                ViewBag.TotalItensLista = totalItensLista;
                ViewBag.ChapasGravasLista = listaChapas;

                return PartialView("_ChapasLista");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult DeletarChapasGravadas(int id, int NumeroOS)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();

                data.Pre_DeletarChapa(id);

                var listaChapas = new ChapaLista();
                listaChapas = data.Pre_ChapaPorIdOs(NumeroOS);

                int totalItensLista = listaChapas.Count();

                ViewBag.TotalItensLista = totalItensLista;
                ViewBag.ChapasGravasLista = listaChapas;

                return PartialView("_ChapasLista");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        public ActionResult ChapaRegravada()
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                var data = new Data.PreImpressao();
                var motivos = new ChapaMotivoLista();
                motivos = data.ChapaMotivo();

                return PartialView("_ChapasRegravadas", motivos);
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
        [HttpPost]
        public ActionResult ChapaRegravada(Chapa chapa)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                // INSTÂNCIA CONTROLER
                var data = new Data.PreImpressao();

                // OBTEM MES ATUAL
                DateTime dataAtual = DateTime.Now;
                string mes = dataAtual.ToString(@"MMMM", new CultureInfo("PT-pt"));

                // GRAVA CHAPA NO BANCO
                int retorno = data.Pre_ChapaGravada(chapa);

                return RedirectToAction("Lista");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return PartialView("~/Views/Sistema/_Erro.cshtml");
            }

        }
    }
}