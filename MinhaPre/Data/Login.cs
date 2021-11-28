using System;
using System.Data;

namespace MinhaPre.Data
{
    public class Login
    {
        // INSTÂNCIA MYSQL
        _MySql mySql = new _MySql();

        public Usuario login(Usuario usuario)
        {
            mySql.LimparParametros();

            mySql.AdicionarParametro("varUsername", usuario.Username);
            mySql.AdicionarParametro("varSenha", usuario.Senha);

            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "login");


            foreach (DataRow dataRow in dataTable.Rows)
            {
                usuario.IdUsuario = Convert.ToInt32(dataRow["IdUsuario"]);
                usuario.Nome = Convert.ToString(dataRow["Nome"]);
                usuario.NivelAcesso = Convert.ToString(dataRow["NivelAcesso"]);
                usuario.Modulo = Convert.ToString(dataRow["Modulo"]);
            }

            usuario.Username = null;
            usuario.Senha = null;

            return usuario;
        }

    }
}
