using System;
using System.Data;


namespace MinhaPre.Data
{
    public class UsuarioDados
    {
        _MySql mySql = new _MySql();

        public int alterarSenha(Usuario usuario)
        {
            mySql.LimparParametros();

            mySql.AdicionarParametro("varIdusuario", usuario.IdUsuario);
            mySql.AdicionarParametro("varSenha", usuario.Senha);
            mySql.AdicionarParametro("varNovaSenha", usuario.NovaSenha);

            DataTable dataTable = mySql.Consultar(CommandType.StoredProcedure, "usuario_AlterarSenha");

            int retorno = 0;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                retorno = Convert.ToInt32(dataRow["IdUsuario"]);
            }

            return retorno;
        }
    }
}
