using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class ComumDB : BaseDB
    {
        #region [ Métodos de Inclusão ]
        /// <summary>Inclui o item no banco</summary>
        /// <returns>Objeto</returns>
        public static void Inserir<T>(T item) 
            where T : Entidade, new()
        {
            //Verifica se existe algum item
            if (item == null)
                return;

            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                //Define os valores padrões do item
                DefinirValorPadrao(item, OperacaoBanco.Inserir);

                db.Set<T>().Add(item);
                db.Entry(item).State = EntityState.Added;

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new Exception(GerarMensagemErroValidacaoDB(ex));
                }
            }
        }

        /// <summary>Efetua a inclusão da lista enviada</summary>
        /// <param name="type">lista de itens</param>
        /// <returns>Objeto</returns>
        public static void Inserir<T>(List<T> itens)
            where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            using (ContextoBanco db = RetornarContextoDB(itens[0].Contexto))
            {
                DbSet<T> tabela = db.Set<T>();

                itens.ForEach(i => 
                    {
                        //Define os valores padrões do item
                        DefinirValorPadrao(i, OperacaoBanco.Inserir);

                        tabela.Add(i);
                        db.Entry(i).State = EntityState.Added;
                    });

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new Exception(GerarMensagemErroValidacaoDB(ex));
                }
            }
        }
        #endregion

        #region [ Atualizar ]
        /// <summary>Efetua a atualização do objeto informado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static void Atualizar<T>(T item, List<String> propriedades = null)
            where T : Entidade, new()
        {
            // Verificando a necessidade de attach ou apenas atualização de valores
            Boolean attach = false;
            ObjectStateEntry registro;

            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                //Define os valores padrões do item
                DefinirValorPadrao(item, OperacaoBanco.Atualizar);

                ObjectContext contexto = (db as IObjectContextAdapter).ObjectContext;
                if (contexto.ObjectStateManager.TryGetObjectStateEntry(
                        contexto.CreateEntityKey(ObterEntitySetName(typeof(T)), item),
                        out registro
                    )
                )
                {
                    // Verificando a necessidade de Attach
                    attach = registro.State == EntityState.Detached;

                    // Atualizando informações do registro existente
                    registro.ApplyCurrentValues(item);

                    // Atualizando o parametro de entidade
                    item = (T)registro.Entity;
                }
                else
                    attach = true;

                if (attach)
                    contexto.AttachTo(ObterEntitySetName(typeof(T)), item);

                if (propriedades != null)
                {
                    foreach (String prop in propriedades)
                        try
                        {
                            db.Entry(item).Property(prop).IsModified = true;
                        }
                        catch { }//Extensions - propriedades não mapeadas no entity da erro (TODO:melhorar esse código)
                }
                else
                {
                    // Forçando o estado como modificado
                    db.Entry(item).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new Exception(GerarMensagemErroValidacaoDB(ex));
                }
            }
        }

        /// <summary>Atualiza a lista de objetos informados</summary>
        /// <typeparam name="T">Tipo de objeto para ser atualizado</typeparam>
        /// <param name="itens">Lista de entidades</param>
        /// <returns></returns>
        public static void Atualizar<T>(List<T> itens)
            where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            // Verificando a necessidade de attach ou apenas atualização de valores
            Boolean attach = false;
            ObjectStateEntry registro;

            using (ContextoBanco db = RetornarContextoDB(itens[0].Contexto))
            {
                ObjectContext contexto = (db as IObjectContextAdapter).ObjectContext;

                //Atualiza todos os itens
                itens.ForEach(item =>
                    {
                        //Define os valores padrões do item
                        DefinirValorPadrao(item, OperacaoBanco.Atualizar);
                        
                        if (contexto.ObjectStateManager.TryGetObjectStateEntry(
                                contexto.CreateEntityKey(ObterEntitySetName(typeof(T)), item),
                                out registro
                            )
                        )
                        {
                            // Verificando a necessidade de Attach
                            attach = registro.State == EntityState.Detached;

                            // Atualizando informações do registro existente
                            registro.ApplyCurrentValues(item);

                            // Atualizando o parametro de entidade
                            item = (T)registro.Entity;
                        }
                        else
                            attach = true;

                        if (attach)
                            contexto.AttachTo(ObterEntitySetName(typeof(T)), item);

                        // Forçando o estado como modificado
                        db.Entry(item).State = EntityState.Modified;
                    });

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw new Exception(GerarMensagemErroValidacaoDB(ex));
                }
            }
        }
        #endregion

        #region [ Métodos para Excluir o item ]
        /// <summary>Exclui o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static void Excluir<T>(T item)
            where T : Entidade, new()
        {
            //Define os valores padrões do item
            DefinirValorPadrao(item, OperacaoBanco.Excluir);

            //Exclusão lógica do item
            Atualizar(item);
        }

        /// <summary>Exclui a lista de objetos informados</summary>
        /// <param name="itens">Lista de entidades</param>
        /// <returns>Objeto</returns>
        public static void Excluir<T>(List<T> itens)
            where T : Entidade, new()
        {
            //Verifica se possui item na lista
            if (itens == null || itens.Count == 0)
                return;

            //Define os valores padrões do item
            itens.ForEach(i => DefinirValorPadrao(i, OperacaoBanco.Excluir));

            //Exclusão lógica do item
            Atualizar(itens);
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static void Excluir<T>(T entidade, List<int> chaves)
            where T : Entidade, new()
        {
            if (chaves.Count == 0)
                return;

            //Efetua a exclusão dos itens
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                DbSet<T> tabela = db.Set<T>();
                List<T> itens  = new List<T>();

                chaves.ForEach(i =>
                    {
                        T item = tabela.Find(i);
                        if (item != null)
                            itens.Add(item);
                    }
                );

                //Exclui todos os itens
                if (itens.Count > 0)
                    Excluir(itens);
            }
        }
        #endregion

        #region [ Métodos Obter ]
        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static T Obter<T>(T entidade, Int32 chave) where T : Entidade, new()
        {
            T retorno = default(T);

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                DbSet<T> tabela = db.Set<T>();
                retorno = tabela.Find(chave);
            }

            //Retorna
            return retorno;
        }

        /// <summary>Retorna o objeto solicitado</summary>
        /// <param name="cod_anexo">ID do item</param>
        /// <returns>Objeto</returns>
        public static T Obter<T>(T entidade, Expression<Func<T, Boolean>> filtro) where T : Entidade, new()
        {
            T retorno = default(T);

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                DbSet<T> tabela = db.Set<T>();
                retorno = tabela.Where(filtro).FirstOrDefault();
            }

            //Retorna
            return retorno;
        }
        #endregion

        #region [ Métodos Listar ]
        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<T> Consultar<T>(T entidade) where T : Entidade, new()
        {
            List<T> retorno = new List<T>();

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                DbSet<T> tabela = db.Set<T>();
                retorno = tabela.ToList();
            }

            //Retorna
            return retorno;
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static List<T> Consultar<T>(T entidade, List<Int32> chaves) where T : Entidade, new()
        {
            List<T> retorno = new List<T>();

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                DbSet<T> tabela = db.Set<T>();
                chaves.ForEach(i =>
                        {
                            T item = tabela.Find(i);
                            if (item != null)
                                retorno.Add(item);
                        }
                    );
            }

            //Retorna
            return retorno;
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<T> Consultar<T>(T entidade, Expression<Func<T, Boolean>> filtro) where T : Entidade, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade);
            else
            {
                List<T> retorno = new List<T>();

                //Busca o objeto solicitado
                using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
                {
                    DbSet<T> tabela = db.Set<T>();
                    retorno = tabela.Where(filtro).ToList();
                }

                //Retorna
                return retorno;
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<T> Consultar<T, TOrdem>(T entidade, Expression<Func<T, Boolean>> filtro
			, Expression<Func<T, TOrdem>> ordenacao, Int32 paginaAtual, Int32 itensPagina, Boolean descendente = false) where T : EntidadeDB, new()
        {
            if (filtro == null)
                return Consultar<T>(entidade);
            else
            {
                List<T> retorno = new List<T>();
				Int32 itensTotal = -1;

				paginaAtual = paginaAtual <= 0 ? 0 : paginaAtual;

                //Busca o objeto solicitado
                using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
                {
                    DbSet<T> tabela = db.Set<T>();
					var query = tabela.Where(filtro);

					itensTotal = query.Count();
					if (itensTotal <= 0)
						return retorno;

                    if (ordenacao == null)
							retorno = query.Skip((paginaAtual - 1) * itensPagina)
													   .Take(itensPagina).ToList();
                    else
                    {
                        //Efetua a busca dos itens
                        if (!descendente)
								retorno = query.OrderBy(ordenacao).Skip(paginaAtual)
														   .Take(itensPagina).ToList();
                        else
								retorno = query.OrderByDescending(ordenacao).Skip(paginaAtual)
														   .Take(itensPagina).ToList();
                    }
                }
				retorno.ForEach(_ => { _.TotalRecordCount = itensTotal; });
                //Retorna
                return retorno;
            }
        }

        /// <summary>Retorna a lista de objetos aplicando um filtro</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <param name="distinct">Expressão a ser usada no distinct</param>
        /// <returns>Lista de objetos encontrados</returns>
        public static List<TColumn> Consultar<T, TColumn>(T entidade, Expression<Func<T, Boolean>> filtro, Expression<Func<T, TColumn>> distinct) where T : Entidade, new()
        {
            
            List<TColumn> retorno = new List<TColumn>();

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                DbSet<T> tabela = db.Set<T>();

                if (filtro == null && distinct != null)
                    retorno = tabela.Select(distinct).Distinct().ToList();
                else if(filtro != null && distinct != null)
                    retorno = tabela.Where(filtro).Select(distinct).Distinct().ToList();
            }

            //Retorna
            return retorno;
        }

        /// <summary>Retorna a quantidade de objetos encontrados com o filtro informado</summary>
        /// <param name="filtro">Expressão a ser usada como filtro</param>
        /// <returns>Quantidade de objetos encontrados</returns>
        public static Int32 ConsultarQtd<T>(T entidade, Expression<Func<T, Boolean>> filtro) where T : Entidade, new()
        {
            Int32 retorno = 0;

            //Busca o objeto solicitado
            using (ContextoBanco db = RetornarContextoDB(entidade.Contexto))
            {
                DbSet<T> tabela = db.Set<T>();

                //Efetua a busca dos itens
                if (filtro != null)
                    retorno = tabela.Where(filtro).Count();
                else
                    retorno = tabela.Count();
            }

            return retorno;
        }
        
        #endregion

        private static String ObterEntitySetName(Type tipo)
        {
			// return tipo.Name;
            return tipo.Namespace.Contains("Core") ? "PortalDeFluxoEntities." + tipo.Name : "BancoDados." + tipo.Name;
        }
    }
}
