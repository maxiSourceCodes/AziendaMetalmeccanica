using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AziendaMetalmeccanica
{
       public class Cliente
        {
        //definisco i parametri di configurazione dell'utente
        public String Nome { get; set; }
        public String Cognome { get; set; }
        public String Email { get; set; }
        public String PW { get; set; }
        public String Iban { get; set; }
    }
}
