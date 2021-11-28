using System;
using System.Web.Mvc;


namespace MinhaPre.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Inicio()
        {
            Session.Abandon();
            return View("Inicio");
        }
        [HttpPost]
        public ActionResult Autenticacao(Usuario usuario)
        {
            try
            {
                if ((usuario.Username == null || usuario.Username == "") || usuario.Senha == null || usuario.Senha == "")
                {
                    ViewBag.Erro = "CamposNull";
                    return View("Inicio");
                }
                else
                {
                    var data = new Data.Login();

                    //VERIFICA SE USUÁRIO EXISTE 
                    var usuarioBanco = data.login(usuario);

                    if (usuario.IdUsuario > 0)
                    {
                        Session["IdUsuario"] = usuario.IdUsuario;
                        Session["Nome"] = usuario.Nome;
                        Session["PerfilAcesso"] = usuario.NivelAcesso;
                        Session["Modulo"] = usuario.Modulo;
                        Session.Timeout = 120;

                        return RedirectToAction("Portal", "Sistema");
                    }

                    //SE USUÁRIO NÃO EXISTE 
                    else
                    {
                        ViewBag.Erro = "LogInnvalido";
                        usuario = null;
                        return View("Inicio");
                    }
                }
            }
            catch (Exception exception)
            {
                ViewBag.MensagemErro = exception.ToString();
                ViewBag.Erro = "Erro";
                return View("Inicio");
            }
        }
    }
}