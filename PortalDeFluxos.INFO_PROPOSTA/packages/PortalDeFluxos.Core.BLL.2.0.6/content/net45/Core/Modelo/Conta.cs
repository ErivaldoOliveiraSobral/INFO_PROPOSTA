using Microsoft.SharePoint.Client;
using System;
using PortalDeFluxos.Core.BLL.Utilitario;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class Conta
    {
        public String UserName { get; set; }

        public String Password { get; set; }

        public String Domain { get; set; }

        public System.Net.NetworkCredential NetWorkCredential
        {
            get
            {
                return new System.Net.NetworkCredential(this.UserName, this.Password, this.Domain);
            }
        }

        public Conta(Dictionary<String, Object> contaDictionay)
        {
            this.Domain = contaDictionay.ContainsKey("Domain") ? (String)contaDictionay["Domain"] : String.Empty;
            this.UserName = contaDictionay.ContainsKey("UserName") ? (String)contaDictionay["UserName"] : String.Empty;
            this.Password = contaDictionay.ContainsKey("Password") ? (String)contaDictionay["Password"] : String.Empty;
        }
    }
}
