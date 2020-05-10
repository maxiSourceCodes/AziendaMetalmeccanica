using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AziendaMetalmeccanica
{
    //questa classe l'insieme degli ordini effettuati in un singolo momento
    //un ordine è una tripla Cod.Prod. + Quantità + Descrizione
    class Carrello
    {
        private DateTime dataAcquisto;

        private int codUtente;

        private bool pagato = false;

        private int codTipologia;

        private double totImporto;

        private string stringTipo;

        public List<Ordine> Ordini { get; set; }

        public double TotImporto
        {
            get { return totImporto; }
            set { totImporto = value; }
        }

        public bool Pagato
        {
            get { return pagato; }
            set { pagato = value; }
        }

        public int CodTipologia
        {
            get { return codTipologia; }
            set { codTipologia = value;}
        }

        public int CodUtente
        {
            get { return codUtente; }
            set { codUtente = value; }
        }

        public string StringTipo
        {
            get { return stringTipo; }
            set { stringTipo = value; }
        }

        public DateTime DataAcquisto
        {
            get { return dataAcquisto; }
            set { dataAcquisto = value; }
        }


        public Carrello(int utente, int codiceLetto, List<Ordine> ordini, double totale)
        {
            //imposto le info riguardanti l'ordine appena eseguito
            Ordini = ordini;

            //aggiungo al carrello le informazioni provenienti dalla creazione
            CodTipologia = codiceLetto;

            CodUtente = utente;

            TotImporto = totale;


        }
    }
}

