using System;
using System.Data.Entity;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public sealed class ContextoTransacional : IDisposable
    {
        public ContextoBanco ContextoBanco = null;

        /// <summary>
        /// Define se a instância já foi destruida.
        /// </summary>
        private Boolean disposed;

        /// <summary>Inicia o contexto transacional da aplicação (Só pode ser iniciado através do PortalWeb)</summary>
        internal ContextoTransacional(PortalWeb contexto)
        {
            ContextoBanco = BaseDB.IniciarTransacaoDB(contexto);
        }

        /// <summary>
        /// Executa a confirmação de mudancas.
        /// </summary>
        public void ConfirmarMudancas()
        {
            // Realizando o commit da transação
            if (ContextoBanco == null)
                return;

            if (ContextoBanco.Database.CurrentTransaction != null)
                ContextoBanco.Database.CurrentTransaction.Commit();

            ContextoBanco.ExecutandoTransacao = false;
            ContextoBanco.Dispose();
            ContextoBanco = null;
        }

        /// <summary>
        /// Executa o cancelamento de mudancas.
        /// </summary>
        public void CancelarMudancas()
        {
            // Realizando o rollback da transação
            if (ContextoBanco == null)
                return;

            if (ContextoBanco.Database.CurrentTransaction != null)
                ContextoBanco.Database.CurrentTransaction.Rollback();

            ContextoBanco.ExecutandoTransacao = false;
            ContextoBanco.Dispose();
            ContextoBanco = null;
        }

        /// <summary>
        /// Implementa o método de dstruição do objeto de banco de dados do Entity Framework.
        /// </summary>
        private void DestruirObjetoBanco()
        {
            if (ContextoBanco != null)
            {
                try
                {
                    if (ContextoBanco.Database.CurrentTransaction != null)
                        ContextoBanco.Database.CurrentTransaction.Rollback();
                }
                // Ocultando erros para casos onde a transação já sofreu rollback.
                catch { }

                ContextoBanco.ExecutandoTransacao = false;
                ContextoBanco.Dispose();
                ContextoBanco = null;
            }
        }

         /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        ~ContextoTransacional()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        /// <param name="disposing">Define se a destruição foi realizada explicitamente.</param>
        internal void Dispose(bool disposing)
        {
            // Verificando se o objeto já está em processo de liberação
            if (this.disposed)
                return;
            disposed = true;

            // Realizando a liberação do objeto e da sessão de banco de dados
            DestruirObjetoBanco();
        }
    }
}
