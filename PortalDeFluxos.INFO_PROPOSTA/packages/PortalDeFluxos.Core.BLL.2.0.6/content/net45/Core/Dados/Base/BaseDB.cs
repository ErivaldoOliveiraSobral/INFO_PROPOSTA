using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;

namespace PortalDeFluxos.Core.BLL.Dados
{
    /// <summary>Define o a operação realizada no banco.</summary>
    public enum OperacaoBanco
    {
        /// <summary>Operação de inclusão no banco de dados.</summary>
        Inserir = 1,
        /// <summary>Operação de atualização no banco de dados.</summary>
        Atualizar = 2,
        /// <summary>Operação de exclusão no banco de dados.</summary>
        Excluir = 3,
        /// <summary>Operação de consulta de um registro no banco de dados.</summary>
        Obter = 4,
        /// <summary>Operação de consulta no banco de dados.</summary>
        Consultar = 5
    }

    /// <summary>Contexto do banco de dados</summary>
    public class ContextoBanco : PortalDeFluxoEntities
    {
        public Boolean ExecutandoTransacao { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (!ExecutandoTransacao)
                base.Dispose(disposing);
        }

		public ContextoBanco(PortalWeb pWeb) : base(pWeb.Configuracao.ConnectionString) {  }
    }

    public class BaseDB
    {
        /// <summary>
        /// Retorna o contexto do banco de dados
        /// </summary>
        /// <param name="usarTransacaoContexto"></param>
        /// <returns></returns>
        public static ContextoBanco RetornarContextoDB(PortalWeb contexto, bool usarTransacaoContexto = true)
        {
            if (usarTransacaoContexto && contexto != null && contexto.Transacao != null)
                return contexto.Transacao.ContextoBanco;
            return new ContextoBanco(contexto);
        }

        /// <summary>
        /// Inicia uma transação no banco de dados
        /// </summary>
        /// <returns></returns>
        public static ContextoBanco IniciarTransacaoDB(PortalWeb contexto)
        {
            //Busca o contexto do banco
            var db = RetornarContextoDB(contexto, false);

            //Inicia a transação
            db.Database.BeginTransaction();
            db.ExecutandoTransacao = true;

            return db;
        }

        /// <summary>
        /// Define as propriedades básicas do objeto na operação
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="item">Objeto</param>
        /// <param name="operacao">Operação realizada</param>
        public static void DefinirValorPadrao<T>(T item, OperacaoBanco operacao)
        {
            String usuarioAtual = System.Threading.Thread.CurrentPrincipal.Identity != null && !String.IsNullOrWhiteSpace(System.Threading.Thread.CurrentPrincipal.Identity.Name) ?
                                  System.Threading.Thread.CurrentPrincipal.Identity.Name :
                                  String.Concat(Environment.UserDomainName, @"\", Environment.UserName);

            usuarioAtual = usuarioAtual.RemoverClaims();

            switch (operacao)
            {
                case OperacaoBanco.Inserir:
                    //Define os valores padrões
                    Reflexao.DefinePropriedade(item, "Ativo", true);
                    Reflexao.DefinePropriedade(item, "LoginInclusao", usuarioAtual);
                    Reflexao.DefinePropriedade(item, "DataInclusao", DateTime.Now);
                    break;
                case OperacaoBanco.Atualizar:
                    //Define os valores padrões
                    Reflexao.DefinePropriedade(item, "LoginAlteracao", usuarioAtual);
                    Reflexao.DefinePropriedade(item, "DataAlteracao", DateTime.Now);
                    break;
                case OperacaoBanco.Excluir:
                    //Define os valores padrões
                    Reflexao.DefinePropriedade(item, "LoginAlteracao", usuarioAtual);
                    Reflexao.DefinePropriedade(item, "DataAlteracao", DateTime.Now);
                    Reflexao.DefinePropriedade(item, "Ativo", false);
                    break;
            }
        }

        /// <summary>Busca os erros de validação encontrados</summary>
        /// <param name="erro">Erro</param>
        /// <returns>Mensagem de erro</returns>
        public static string GerarMensagemErroValidacaoDB(DbEntityValidationException erro)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in erro.EntityValidationErrors)
            {
                sb.AppendLine(String.Format("A entidade \"{0}\" no estado \"{1}\" teve os seguintes erros de validação:",
                                            item.Entry.Entity.GetType().Name, item.Entry.State));

                foreach (var ve in item.ValidationErrors)
                    sb.AppendLine(String.Format("- Propriedade: \"{0}\", Erro: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
            }
            return sb.ToString();
        }

        /// <summary>Cria o command para execução no banco</summary>
        /// <param name="contextoDB">Contexto do Banco</param>
        /// <param name="procedure">Procedure</param>
        /// <returns>Command</returns>
        public static DbCommand CriarCommand(ContextoBanco contextoDB, String procedure)
        {
            DbCommand command = contextoDB.Database.Connection.CreateCommand();

            //Define a procedure
            command.CommandText = procedure;
            command.CommandType = System.Data.CommandType.StoredProcedure;

            //Abre a conexão se necessário
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();

            return command;
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        public static void ExecutarProcedure(PortalWeb pWeb, String procedure, Int32? tempoExecucao, object[] parametros)
        {
            using (ContextoBanco db = RetornarContextoDB(pWeb))
            {
                if (tempoExecucao.HasValue)
                    db.Database.CommandTimeout = tempoExecucao;

                //Executa a procedure spLimparLog
                if (parametros == null || parametros.Length == 0)
                    db.Database.ExecuteSqlCommand(procedure);
                else
                    db.Database.ExecuteSqlCommand(procedure, parametros);
            }
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        public static void ExecutarProcedure(PortalWeb pWeb, String procedure, object[] parametros)
        {
            ExecutarProcedure(pWeb, procedure, null, parametros);
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        /// <param name="converter">Função de conversão de objeto</param>
        /// <returns>Lista do objeto</returns>
        public static List<T> ExecutarProcedure<T>(PortalWeb pWeb, String procedure, Func<DbDataReader, T> converter, List<DbParameter> parametros = null)
            where T : class, new()
        {
            //Lista de e-mails de tarefas escalonadas para envio
            List<T> itens = new List<T>();

            using (ContextoBanco db = RetornarContextoDB(pWeb))
            using (DbCommand command = CriarCommand(db, procedure))
            {
                //Define os parâmetros de execução
                if (parametros != null)
                    foreach (var parametro in parametros)
                        command.Parameters.Add(parametro);

                //Executa o comando
                using (DbDataReader dr = command.ExecuteReader())
                    while (dr.Read())
                        itens.Add(converter(dr));
            }

            return itens;
        }


        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        /// <returns>Dataset</returns>
        public static DataSet ExecutarProcedureDataSet(PortalWeb pWeb, String procedure, List<DbParameter> parametros = null, string[] dsHeader = null)
        {
            DataSet ds = new DataSet();

            using (ContextoBanco db = RetornarContextoDB(pWeb))
            {
                using (DbCommand command = CriarCommand(db, procedure))
                {
                    //Define os parâmetros de execução
                    if (parametros != null)
                        foreach (var parametro in parametros)
                            command.Parameters.Add(parametro);

                    //Executa o comando
                    using (DbDataReader dr = command.ExecuteReader())
                        ds.Load(dr, LoadOption.OverwriteChanges, dsHeader);
                }
            }

            return ds;
        }
    }
}
