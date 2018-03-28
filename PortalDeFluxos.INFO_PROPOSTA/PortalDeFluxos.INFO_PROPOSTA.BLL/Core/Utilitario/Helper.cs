using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalDeFluxos.Core.BLL.Negocio;

namespace PortalDeFluxos.Core.BLL.Utilitario
{
    public static class DataHelper
    {
        public static DateTime CalcularSLA(ConfiguracaoExpediente configuracaoExpediente, DateTime data, Double tempoTotal, Double tempoUtilUtilizado = 0)
        {
            //Busca as parametrizações de horário útil
            Int32 contadorMinuto = 0;
            TimeSpan HorarioExpedienteEntrada = configuracaoExpediente.HorarioExpedienteEntrada;
            TimeSpan HorarioExpedienteSaida = configuracaoExpediente.HorarioExpedienteSaida;
            List<DayOfWeek> diasUteisSemana = configuracaoExpediente.DiasUteisSemana;
            List<DateTime> feriados = configuracaoExpediente.Feriados;
            DateTime dataCalculada = data;

            Double tempoUtil = CalcularTempoUtil(configuracaoExpediente, data, tempoTotal);
            tempoUtil = tempoUtilUtilizado > tempoUtil ? 0 : tempoUtil - tempoUtilUtilizado;

            // Caso não exista dias úteis configurado, retorna a própria data
            if (diasUteisSemana == null)
                return data;

            // Caso não exista horário de início de expediente configurado, retorna meia noite como default
            if (HorarioExpedienteEntrada == null)
                HorarioExpedienteEntrada = new TimeSpan(0, 0, 0);

            // Caso não exista horário de fim de expediente configurado, 23:59 como default
            if (HorarioExpedienteSaida == null)
                HorarioExpedienteSaida = new TimeSpan(23, 59, 59);

            //Obter minutos uteis

            // a variável date prime contém o valor calculada da data
            // a ideia é percorrer minuto a minuto a data, validando dias uteis e feriados
            while (contadorMinuto != tempoUtil)
            {
                // // Adiciona o minuto atual a data
                dataCalculada = dataCalculada.AddMinutes(1);

                // Primeiro valida se "NÃO" é um dia útil, em seguida valida se é feriado, em qualquer um dos casos, obtém o próximo dia
                while (
                    !diasUteisSemana.Exists(item => item.Equals(dataCalculada.DayOfWeek)) /* Validação de dia útil */ ||
                    feriados.Any(item => item.Date.Equals(dataCalculada.Date)) /* Verifica se é um feriado */
                )
                    dataCalculada = dataCalculada.AddDays(1);// Vai para o próximo dia.

                // Calcula a hora de inicio/fim do expediente para data calculada
                DateTime dataCalculadaExpedienteInicio = dataCalculada.Date.Add(HorarioExpedienteEntrada);
                DateTime dataCalculadaExpedienteFim = dataCalculada.Date.Add(HorarioExpedienteSaida);
                // Verifica se a data está dentro dos limites de horário [HorarioExpedienteEntrada] e [HorarioExpedienteSaida].
                // Primeiro cenário: [HorarioExpedienteEntrada] < [HorarioExpedienteSaida].
                if (!(
                    HorarioExpedienteEntrada < HorarioExpedienteSaida && dataCalculada >= dataCalculadaExpedienteInicio && dataCalculada <= dataCalculadaExpedienteFim ||
                    // Segundo cenário: [HorarioExpedienteEntrada] > [HorarioExpedienteSaida]
                    HorarioExpedienteEntrada > HorarioExpedienteSaida && dataCalculada <= dataCalculadaExpedienteInicio && dataCalculada >= dataCalculadaExpedienteFim
                ))
                    dataCalculada = dataCalculada.AddDays(1).Date.Add(HorarioExpedienteEntrada).AddMinutes(1); // Vai para o próximo dia útil

                contadorMinuto++;
            }
            
            if (dataCalculada == dataCalculada.Date.Add(HorarioExpedienteSaida))//Verifica se a data encontrada não é a data de termino do expediente
			{
				dataCalculada = dataCalculada.AddDays(1).Date.Add(HorarioExpedienteEntrada);// Vai para o próximo dia.
				while (
					!diasUteisSemana.Exists(item => item.Equals(dataCalculada.DayOfWeek)) /* Validação de dia útil */ ||
					feriados.Any(item => item.Date.Equals(dataCalculada.Date)) /* Verifica se é um feriado */
				)
					dataCalculada = dataCalculada.AddDays(1);// Vai para o próximo dia.
			}

            return dataCalculada;
        }

        public static TimeSpan ConverterTextoHoras(String texto, TimeSpan valorDefault)
        {
            TimeSpan valor = new TimeSpan();
            if (!TimeSpan.TryParse(texto, out valor))
            {
                valor = valorDefault;
            }

            return valor;
        }

        public static Double CalcularTempoUtil(ConfiguracaoExpediente configuracaoExpediente, DateTime dataInicio, DateTime dataFim, bool fgSlaUtilizado = false)
        {
            if (dataFim <= dataInicio)
                return 0;

            //Intervalo total entre as duas datas
            Double tempoTotal = (new TimeSpan(dataFim.Day
                                        , dataFim.Hour
                                        , dataFim.Minute
                                        , dataFim.Second) -
                                    new TimeSpan(dataInicio.Day
                                        , dataInicio.Hour
                                        , dataInicio.Minute
                                        , dataInicio.Second)).TotalMinutes;

            return CalcularTempoUtil(configuracaoExpediente, dataInicio, tempoTotal, fgSlaUtilizado);
        }

        public static Double CalcularTempoUtil(ConfiguracaoExpediente configuracaoExpediente, DateTime dataInicio, Double tempoTotal, bool fgSlaUtilizado = false)
        {
            //Busca as parametrizações de horário útil
            Int32 slaUtilizado = 0;
            TimeSpan HorarioExpedienteEntrada = configuracaoExpediente.HorarioExpedienteEntrada;
            TimeSpan HorarioExpedienteSaida = configuracaoExpediente.HorarioExpedienteSaida;
            List<DayOfWeek> diasUteisSemana = configuracaoExpediente.DiasUteisSemana;
            List<DateTime> feriados = configuracaoExpediente.Feriados;
            DateTime dataCalculada = dataInicio;

            // Caso não exista dias úteis configurado, retorna o intervalo entre as datas
            if (diasUteisSemana == null)
                return 0;

            // Caso não exista horário de início de expediente configurado, retorna meia noite como default
            if (HorarioExpedienteEntrada == null)
                HorarioExpedienteEntrada = new TimeSpan(0, 0, 0);
            
            // Caso não exista horário de fim de expediente configurado, 23:59 como default
            if (HorarioExpedienteSaida == null)
                HorarioExpedienteSaida = new TimeSpan(23, 59, 59);


            // a ideia é percorrer minuto a minuto a data, validando dias uteis e feriados
            for (int i = 0; i < tempoTotal; i++)
            {
                // Vai para o próximo dia útil.
                dataCalculada = dataCalculada.AddMinutes(1);

                // Primeiro valida se "NÃO" é um dia útil, em seguida valida se é feriado, em qualquer um dos casos, obtém o próximo dia
                while (
                    !diasUteisSemana.Exists(item => item.Equals(dataCalculada.DayOfWeek)) /* Validação de dia útil */ ||
                    feriados.Any(item => item.Date.Equals(dataCalculada.Date)) /* Verifica se é um feriado */
                )
                {
                    dataCalculada = dataCalculada.AddDays(1);// Vai para o próximo dia.
                    if (fgSlaUtilizado)
                        i = i + 1440;
                }

                // Calcula a hora de inicio/fim do expediente para data calculada
                DateTime dataCalculadaExpedienteInicio = dataCalculada.Date.Add(HorarioExpedienteEntrada);
                DateTime dataCalculadaExpedienteFim = dataCalculada.Date.Add(HorarioExpedienteSaida);

                // Verifica se a data está dentro dos limites de horário [HorarioExpedienteEntrada] e [HorarioExpedienteSaida].
                // Primeiro cenário: [HorarioExpedienteEntrada] < [HorarioExpedienteSaida].
                if ((
                    HorarioExpedienteEntrada < HorarioExpedienteSaida && dataCalculada >= dataCalculadaExpedienteInicio && dataCalculada <= dataCalculadaExpedienteFim ||
                    // Segundo cenário: [HorarioExpedienteEntrada] > [HorarioExpedienteSaida]
                    HorarioExpedienteEntrada > HorarioExpedienteSaida && dataCalculada <= dataCalculadaExpedienteInicio && dataCalculada >= dataCalculadaExpedienteFim
                ))
                    slaUtilizado++;

                //if (dataCalculada.Day - dataCalculada.AddMinutes(-1).Day > 0)
                //    slaUtilizado--;//Virada de dia (contabilizar apenas uma vez)
            }

            return slaUtilizado;
        }

    }

    public static class ClaimsExtension
    {
        public static string RemoverClaims(this string loginComClaims)
        {
            if (String.IsNullOrEmpty(loginComClaims))
            {
                return loginComClaims;
            }

            if (loginComClaims.Length < 6)
            {
                return loginComClaims;
            }

            // remover o identitit claims
            loginComClaims = loginComClaims.Substring(loginComClaims.IndexOf(':') + 1);

            if (loginComClaims[4] != '|')
            {
                return loginComClaims;
            }
            String text = loginComClaims.Substring(5);
            if (String.IsNullOrEmpty(text))
            {
                return loginComClaims;
            }
            int num = text.IndexOf('|');
            String originalIssuerEncoded = null;
            String loginComClaims2 = text;
            if (num != -1)
            {
                originalIssuerEncoded = text.Substring(0, num);
                loginComClaims2 = text.Substring(num + 1);
            }
            else if (loginComClaims[3] != 'w' && loginComClaims[3] != 's')
            {
                return loginComClaims;
            }

            String loginSemClaims = RemoverClaimsPorSeguranca(loginComClaims2);

            return loginSemClaims;
        }

        public static string RemoverClaimsAndDomain(this string loginComClaims)
        {
            String login = loginComClaims.RemoverClaims();
            
            if (login.Split('\\').Length >1)
                login = login.Split('\\')[1];

            return login;
        }

        public static string ObterDomain(this string loginComClaims)
        {
            String login = loginComClaims.RemoverClaims();
            String dominio = "";
            if (login.Split('\\').Length > 1)
                dominio = login.Split('\\')[0];

            return dominio;
        }

        private static string RemoverClaimsPorSeguranca(this string loginComClaims)
        {
            String loginSemClaims = loginComClaims.Replace("%2C", ",");
            loginSemClaims = loginSemClaims.Replace("%2c", ",");
            loginSemClaims = loginSemClaims.Replace("%3A", ":");
            loginSemClaims = loginSemClaims.Replace("%3a", ":");
            loginSemClaims = loginSemClaims.Replace("%3B", ";");
            loginSemClaims = loginSemClaims.Replace("%3b", ";");
            loginSemClaims = loginSemClaims.Replace("%0A", "\n");
            loginSemClaims = loginSemClaims.Replace("%0a", "\n");
            loginSemClaims = loginSemClaims.Replace("%0D", "\r");
            loginSemClaims = loginSemClaims.Replace("%0d", "\r");
            loginSemClaims = loginSemClaims.Replace("%7C", new String(new char[]
	                                                        {
		                                                        '|'
	                                                        }));
            loginSemClaims = loginSemClaims.Replace("%7c", new String(new char[]
	                                                        {
		                                                        '|'
	                                                        }));
            return loginSemClaims.Replace("%25", "%");
        }
    }
}
