using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    /// <summary>
    /// Configuracoes necessário para o cálculo do SLA (Hora útil, Dia da Semana útil, etc...)
    /// </summary>
    public class ConfiguracaoExpediente
    {
        /// <summary>
        /// Dias da semana considerado como dias úteis
        /// </summary>
        public List<DayOfWeek> DiasUteisSemana { get; set; }

        /// <summary>
        /// Horário de início do expediente
        /// </summary>
        public TimeSpan HorarioExpedienteEntrada { get; set; }
        /// <summary>
        /// Horário de fim do expediente
        /// </summary>
        public TimeSpan HorarioExpedienteSaida { get; set; }

        /// <summary>
        /// Lista contendo os feriados
        /// </summary>
        public List<DateTime> Feriados { get; set; }

        public static ConfiguracaoExpediente ConfiguracaoDefault
        {
            get
            {
                return new ConfiguracaoExpediente()
                {
                    DiasUteisSemana = new List<DayOfWeek>()
                    {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday
                    },
                    HorarioExpedienteEntrada = new TimeSpan(9, 0, 0),
                    HorarioExpedienteSaida = new TimeSpan(18, 0, 0),
                    Feriados = new List<DateTime>()
                };
            }
        }
    }
}
