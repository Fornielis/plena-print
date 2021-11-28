using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MinhaPre.Controllers
{
    public class SistemaController : Controller
    {
        public ActionResult Portal()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return View("Portal");
                }

                ViewBag.Perfil = Session["PerfilAcesso"].ToString();
                return View("Portal");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
        public ActionResult Menu()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    return PartialView("_ErroSessao");
                }

                return PartialView("_Menu");
            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
        public ActionResult DadosIniciais()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    return PartialView("_ErroSessao");
                }

                // VERIFICA SE SISTEMA ESTA ABERTO
                var data = new Data.Sistema();
                var sistemaAbertura = data.Sistema_AberturaPorDia();

                if (sistemaAbertura.TotalMinutos == 0 && sistemaAbertura.Id > 0)
                {
                    // VERIFICA MÓDULO QUE USUÁRIO FAZ PARTE CARREGA VIEW RESPECTIVA
                    string modulo = Session["Modulo"].ToString();

                    if (modulo == "ALL" || modulo == "PRE" || modulo == "COM")
                    {
                        return RedirectToAction("Lista", "PreImpressao");
                    }
                }
                else
                {
                    return RedirectToAction("SistemaFechado", "Sistema");
                }

                return PartialView("_ErroSessao");

            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
        public ActionResult AbreSistema()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    return PartialView("_ErroSessao");
                }

                // INSTANCIA DATA SISTEMA
                var data = new Data.Sistema();

                // ABRE SISTEMA
                data.Sistema_Abre();

                return RedirectToAction("Lista", "PreImpressao");

            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
        public ActionResult SistemaFechado()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    return PartialView("_ErroSessao");
                }

                return PartialView("_SistemaFechado");

            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
        public ActionResult FechaSistema()
        {
            try
            {
                // TESTA SEÇÃO DO USUÁRIO
                if (Session["IdUsuario"] == null)
                {
                    return PartialView("_ErroSessao");
                }

                // INSTANCIA DATA SISTEMA
                var data = new Data.Sistema();

                // FECHA SISTEMA
                data.Sistema_Fecha();

                // ATUALIZA MINUTOS ACUMULADOS
                var dataPreimpressao = new Data.PreImpressao();
                dataPreimpressao.Pre_AtualizaHistoricoEmAberto();

                return RedirectToAction("SistemaFechado", "Sistema");

            }
            catch (Exception exception)
            {
                string mensagemErro = exception.ToString();
                ViewBag.Erro = true;
                return View("Erro");
            }
        }
    }
}