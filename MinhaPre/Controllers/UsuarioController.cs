using System;
using System.Web.Mvc;

namespace MinhaPre.Controllers
{
    public class UsuarioController : Controller
    {
        public ActionResult Usuario()
        {
            return PartialView("_Usuario");
        }

        [HttpPost]
        public PartialViewResult AlteraSenha(Usuario usuario)
        {
            try
            {
                if (Session["IdUsuario"] == null)
                {
                    ViewBag.ErroSessao = true;
                    return PartialView("~/Views/Sistema/_ErroSessao.cshtml");
                }

                if (usuario.Senha == usuario.NovaSenha)
                {
                    ViewBag.Retorno = "SenhasIguais";
                    return PartialView("_Retorno");
                }

                var data = new Data.UsuarioDados();
                usuario.IdUsuario = Convert.ToInt32(Session["IdUsuario"]);
                int retorno = data.alterarSenha(usuario);

                if (retorno > 0)
                {
                    ViewBag.Retorno = "Alterada";
                    return PartialView("_Retorno");
                }

                else
                {
                    ViewBag.Retorno = "SenhaAtualErrada";
                    return PartialView("_Retorno");
                }
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