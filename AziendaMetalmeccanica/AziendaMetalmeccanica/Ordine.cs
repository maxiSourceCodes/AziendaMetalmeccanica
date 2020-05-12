using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AziendaMetalmeccanica
{
    public class Ordine
    {
        //dichiaro gli attributi della classe Ordine

        public int CodeProdotto { get; set; }
        public int Quantità { get; set; }
        public string DescrizioneProdotto { get; set; }

        public Ordine(int codeProdotto, int quantità, string stringProdotto)
        {
            //invoco le proprietà per inserire i campi per inserimento il codice prodotto
            CodeProdotto = codeProdotto; 
            Quantità = quantità;
            DescrizioneProdotto = stringProdotto;
        }
    }
}
