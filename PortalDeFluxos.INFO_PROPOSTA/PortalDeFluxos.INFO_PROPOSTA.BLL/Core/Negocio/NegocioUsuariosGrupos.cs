using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Iteris;
using PortalDeFluxos.Core.BLL.Dados;
using System.Text;
using PortalDeFluxos.Core.BLL.Utilitario;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioUsuariosGrupos
    {

        #region [Grupos]

        public static CustomPainel ConsultarGruposEUsuariosGridView(
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            Boolean? ativo = null,
            String nome = null,
            String tipo = null,
            Boolean? possuiTarefa = null)
        {
            List<UsuariosGruposSP> usuariosGrupos = DadosUsuariosGrupos.ConsultarGruposEUsuarios(indicePagina,
                registrosPorPagina, ordernarPor, ordernarDirecao, ativo, nome, tipo, possuiTarefa);

            Int32 totalRecordCount = usuariosGrupos != null ? usuariosGrupos.Count : 0;
            if (totalRecordCount > 0)
            {
                UsuariosGruposSP usuarioGrupo = usuariosGrupos.FirstOrDefault();
                if (usuarioGrupo != null)
                    totalRecordCount = usuarioGrupo.TotalRecordCount ?? totalRecordCount;
            }
            
            Collection<Object> colecao = new Collection<Object>();
            colecao.AddRange(usuariosGrupos);
            CustomPainel report = null;
            report = new CustomPainel()
            {
                Entries = colecao,
                TotalRecordCount = totalRecordCount
            };

            return report;
        } 
        
        public static void SincronizarGrupos()
        {
            List<Grupo> grupos = PortalWeb.ContextoWebAtual.BuscarGrupos();
            List<GrupoSP> gruposSP = new GrupoSP().Consultar();

            if (gruposSP == null)
                gruposSP = new List<GrupoSP>();

            gruposSP.Add(grupos);
            gruposSP.Where(_ => _.Inserido == true).ToList().Inserir();
            gruposSP.Where(_ => _.Atualizado == true).ToList().Atualizar();

            List<GrupoUsuariosSP> gruposUsuarioSP = new GrupoUsuariosSP().Consultar();
            gruposUsuarioSP.Add(grupos, gruposSP);
            gruposUsuarioSP.Where(_ => _.Inserido == true).ToList().Inserir();
            gruposUsuarioSP.Where(_ => _.Atualizado == true).ToList().Atualizar();
        }

        #endregion

        #region [Usuarios]
        public static List<Usuario> ConsultarUsuarios(
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            Int32? iUsuario= null)
        {
            List<Usuario> usuarios = PortalWeb.ContextoWebAtual.BuscarUsuarios(iUsuario);

            usuarios.ForEach(_ =>
            {
                _.TotalRecordCount = usuarios.Count;
            });

            return usuarios;
        }

        public static void SincronizarUsuarios()
        {
            List<Usuario> usuarios = PortalWeb.ContextoWebAtual.BuscarUsuarios();
            List<UsuarioSP> usuariosSP = new UsuarioSP().Consultar();
            
            if(usuariosSP == null) 
                usuariosSP = new List<UsuarioSP>();

            usuariosSP.Add(usuarios);
            usuariosSP.Where(_ => _.Inserido == true).ToList().Inserir();
            usuariosSP.Where(_ => _.Atualizado == true).ToList().Atualizar();
        }

        #endregion

        #region [Detalhes Usuário/Tarefas]

        public static KeyValuePair<String,String> ObterHtmlListaUsuarios(Int32 idGrupo)
        {
            KeyValuePair<String, String> retorno = new KeyValuePair<string, string>();
            StringBuilder mensagem = new StringBuilder();
            List<UsuarioSP> usuarios = DadosUsuariosGrupos.ConsultarUsuariosGrupo(idGrupo);
            GrupoSP grupo = new GrupoSP().Obter(idGrupo);

            if (usuarios != null && grupo != null)
            {
                foreach (var user in usuarios)
                {
                    mensagem.Append(user.Nome);
                    mensagem.Append(" - ");
                    mensagem.Append(user.Login.Replace(@"\",@"\\"));
                    mensagem.AppendLine(user.Ativo ? " - Ativo" : " - Desativado");
                }
                retorno = new KeyValuePair<string, string>(grupo.Nome, mensagem.ToString().ToJScriptString());
            }

            return retorno;
        }

        public static KeyValuePair<String, String> ObterHtmlListaTarefas(String loginUserIdGrupo)
        {
            KeyValuePair<String, String> retorno = new KeyValuePair<string, string>();
            StringBuilder mensagem = new StringBuilder();
            List<TarefasPendentes> tarefas = DadosUsuariosGrupos.ConsultarTarefasPendentesPorLoginOuGrupo(loginUserIdGrupo);

            if (tarefas != null && tarefas.Count > 0)
            {
                foreach (TarefasPendentes tarefa in tarefas)
                {
                    mensagem.Append(tarefa.NomeSolicitacao);
                    mensagem.Append(" (");
                    mensagem.Append(String.Format(@"<a id=""{0}"" href=""{1}"">{2}</a> ", "tarefa_" + tarefa.IdTarefa
                        , String.Format("{0}", new Uri(new Uri(PortalWeb.ContextoWebAtual.Url), tarefa.DescricaoUrlTarefa)), tarefa.NomeTarefa));
                    mensagem.AppendLine(")");
                }
                retorno = new KeyValuePair<string, string>(tarefas.FirstOrDefault().NomeResponsavel, mensagem.ToString().ToJScriptString());
            }

            return retorno;
        }
        
        #endregion

        #region [Métodos auxiliares]

        public static Boolean EntidadeModificada(this UsuarioSP usuarioSP, Usuario usuario,Boolean ativo,Boolean contaSistema)
        {
            return usuarioSP.Nome != usuario.Nome ||
                   usuarioSP.Login != usuario.Login ||
                   usuarioSP.LoginComClaims != usuario.LoginComClaims ||
                   usuarioSP.LookupValue != usuario.LookupValue ||
                   usuarioSP.Email != usuario.Email ||
                   usuarioSP.SiteAdmin != usuario.SiteAdmin ||
                   usuarioSP.Ativo != ativo ||
                   usuarioSP.ContaSistema != contaSistema;
        }

        public static void Add(this List<UsuarioSP> usuariosSP, List<Usuario> usuarios)
        {
            foreach (Usuario usuario in usuarios)
            {
                Boolean contaSistema = !usuario.Login.ToLower().Contains(System.Environment.UserDomainName.ToLower());
                Boolean ativo = contaSistema || NegocioUsuariosGrupos.UsuarioADAtivo(usuario.Login);
                if (usuariosSP.Any(_ => _.IdUsuarioSP == usuario.Id))
                {
                    UsuarioSP usuarioAtual = usuariosSP.First(_ => _.IdUsuarioSP == usuario.Id);
                    if (usuarioAtual.EntidadeModificada(usuario,ativo,contaSistema))
                    {
                        usuariosSP.Remove(usuarioAtual);
                        usuariosSP.Add(new UsuarioSP(usuario,ativo, atualizado: true));
                    }                    
                }else
                    usuariosSP.Add(new UsuarioSP(usuario,ativo, inserido: true));
            }
        }
        
        public static void Add(this List<GrupoSP> gruposSP, List<Grupo> grupos)
        {
            foreach (Grupo grupo in grupos)
            {
                if (gruposSP.Any(_ => _.IdGrupoSP == grupo.Id))
                {
                    GrupoSP grupoAtual = gruposSP.First(_ => _.IdGrupoSP == grupo.Id);
                    if (grupoAtual.Nome != grupo.Nome)
                    {
                        gruposSP.Remove(grupoAtual);
                        gruposSP.Add(new GrupoSP(grupo, atualizado: true));
                    }
                }
                else
                    gruposSP.Add(new GrupoSP(grupo, inserido: true));
            }

            if (grupos.Count != gruposSP.Count)//se for deletado do SP
                gruposSP.ForEach(_ =>
                {
                    _.Ativo = grupos.Any(u => u.Id == _.IdGrupoSP);
                    _.Atualizado = _.Atualizado == false ? !grupos.Any(u => u.Id == _.IdGrupoSP) : _.Atualizado;
                });
        }

        public static void Add(this List<GrupoUsuariosSP> gruposUsuarioSP, List<Grupo> grupos, List<GrupoSP> gruposSPAtual)
        {
            foreach (Grupo grupo in grupos)
            {
                foreach (Usuario usuario in grupo.Usuarios)
                    if (!gruposUsuarioSP.Any(_ => _.IdGrupoSP == grupo.Id && _.IdUsuarioSP == usuario.Id))
                        gruposUsuarioSP.Add(new GrupoUsuariosSP(grupo, usuario.Id, inserido: true));
                if (grupo.Usuarios.Count != gruposUsuarioSP.Where(_ => _.IdGrupoSP == grupo.Id).ToList().Count)//se for deletado da lista de usuários
                    gruposUsuarioSP.Where(_ => _.IdGrupoSP == grupo.Id).ToList().ForEach(_ =>
                    {
                        _.Atualizado = _.Ativo != grupo.Usuarios.Any(u => u.Id == _.IdUsuarioSP);
                        _.Ativo = grupo.Usuarios.Any(u => u.Id == _.IdUsuarioSP);
                    });
            }

            //Apenas grupos atualizados
            gruposSPAtual.Where(_ => _.Atualizado).ToList().ForEach(_ =>
            {
                gruposUsuarioSP.Where(gu => gu.IdGrupoSP == _.IdGrupoSP).ToList().ForEach( u =>
                    {
                        u.Atualizado = u.IdGrupoUsuariosSP > 0 && u.Ativo != _.Ativo;
                        u.Ativo = _.Ativo;                        
                    });
            });
        }

        public static Boolean UsuarioADAtivo(String loginUsuario)
        {
            Boolean ativo = true;
            if (!loginUsuario.ToLower().Contains(System.Environment.UserDomainName.ToLower()))
                return false;
            try
            {
                DirectorySearcher filtro = new DirectorySearcher(PortalWeb.ContextoWebAtual.DiretorioADRaiz);
                filtro.Filter = String.Format("(&(objectCategory=User)(samAccountName={0}))", loginUsuario.RemoverClaimsAndDomain());
                SearchResult resultadoFiltro = filtro.FindOne();
                if(resultadoFiltro == null)
                    return false;

                DirectoryEntry resultado = resultadoFiltro.GetDirectoryEntry();
                if (resultado.NativeGuid == null)
                    return false;

                if (ativo && resultado.Properties["UserAccountControl"] == null)
                    return false;

                Int32 flag = Convert.ToInt32(resultado.Properties["UserAccountControl"].Value);
                ativo = !Convert.ToBoolean(flag & 0x0002);

            }catch(Exception ex)
            {
                new Log().Inserir("Serviço", String.Format("UsuarioADAtivo - {0}", loginUsuario.RemoverClaimsAndDomain()), ex);
            }

            return ativo;
            
        }

        #endregion
    }
}
