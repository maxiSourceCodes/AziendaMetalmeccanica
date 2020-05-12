using System;
using System.Collections.Generic;
using System.IO;



namespace AziendaMetalmeccanica
{
    class Program
    {
        //dichiaro i file di riferimento al programma
        const string fileListino = "Database/ListinoPrezzi.csv";
        const string fileUtenti = "Database/ElencoClienti.csv";
        const string fileOrdini = "Database/ElencoOrdini.csv";
        const string filePagati = "Database/ElencoPagati.csv";

        //dichiaro un file temporaneo utilizzabile a tutti gli scopi
        const string fileTemporaneo = "Database/temp.csv";

        static void Main(string[] args)
        {

            string[] sceltaUtente = new string[] { "Nuovo utente", "Login utente", "Esci" };

            int clienteNuovo = 0; //questa variabile definirà la scelta utente (Registrazione, Login, etc)

            bool concediAccesso = false,
                erroreFile = false;

            //dichiaro la stringa globale che indica l'utente attualmente in carico
            string Loggato = "";

            //imposto un messaggio per l'utente appena entrato nel programma
            string MessBenvenuto = "Benvenuto su AziendaMetalmeccanica";

            //se uno dei file utilizzati non dovesse esistere, segnalo immediatamente il problema
            if ((!File.Exists(fileListino)) || 
                (!File.Exists(fileUtenti)) || 
                (!File.Exists(fileOrdini)) || 
                (!File.Exists(filePagati)))

            {
                //notifico al cliente un problema in lettura dei file
                Console.WriteLine(MessBenvenuto +
                                  "\n\nAttenzione! Qualche file risulta mancante\n" +
                                  "\nContattare l'amministratore del programma" +
                                  "\nPremi Invio per Uscire");
                Console.ReadLine();
                erroreFile = true;
            }
            else
            {
                do

                {
                    //cancella lo schermo
                    Console.Clear();

                    //mostro un menù per sapere se è un nuovo cliente o un cliente già registrato
                    clienteNuovo = Menu(MessBenvenuto, sceltaUtente);

                    //controllo la scelta dell'utente(Registrato o loggato)
                    if (clienteNuovo == 0)
                        concediAccesso = RegistraCliente();

                    else if ((clienteNuovo == 1) || (concediAccesso))
                        Loggato = AccessoCliente();

                    if (Loggato != "")
                        concediAccesso = true;
                    //se l'accesso è stato consenstito (registrazione o login andate a buon fine)
                    //consento l'accesso al programma originale
                }
                //rimango nel ciclo finché il cliente non sceglie di uscire oppure non viene autenticato
                while (!((concediAccesso) || (clienteNuovo == (sceltaUtente.Length - 1)) || erroreFile)); }




            //se concedo l'accesso al cliente e quest'ultimo non ha scelto di uscire
            //mostro il menu principale del programma
            if ((concediAccesso) && (clienteNuovo != (sceltaUtente.Length - 1) && (!erroreFile)))
                MainMenu(Loggato);
        }
        //menu che consente all'utente connesso di poter selezionare un'azione 
        static void MainMenu(string Loggato)
        {
            int uscita = 0;
            string messaggiomenu;


            do
            {
                //cancella lo schermo
                Console.Clear();

                //messaggio mostrato nel menu principale
                messaggiomenu = "Scegli l'operazione che vuoi eseguire e premi Invio \n\n\n";

                //elenco operazioni disponibili
                string[] modalità = { "Effettua un nuovo ordine",
                                      "Esegui il pagamento di un ordine",
                                      "Visualizza il tuo indice di gradimento",
                                      "Cambia IBAN di riferimento" ,
                                      "Esci" };
                int operazione = Menu(messaggiomenu, modalità);

                switch (operazione)
                {
                    case 0:
                        Ordina(Loggato);
                        break;
                    case 1:
                        Pagamenti(Loggato);
                        break;
                    case 2:
                        Gradimento(Loggato);
                        break;
                    case 3:
                        CambiaIban(Loggato);
                        break;
                    case 4:
                        uscita = ConfermaUscita();
                        break;
                    default:
                        Console.WriteLine("Selezione non prevista.");
                        break;
                }
            }
            while (uscita != 1);
        }


        //funzione che permette il cambio dell'IBAN, previa validazione del nuovo codice
        static void CambiaIban(string utente)
        {
            int indiceRiga=0;
            string ibanAttuale="",
                   ibanNuovo;
            bool   rigaCorrotta=false;

            ConsoleKeyInfo tastoConferma;

            do
            {
                //cancella lo schermo
                Console.Clear();

                if (rigaCorrotta)
                    Console.WriteLine("ATTENZIONE: NON possono essere inseriti caratteri diversi"+
                                      "\n da numeri e lettere maiuscole, nemmeno spazi");


                //inserisco il messaggio di istruzione
                Console.WriteLine("\n Inserisci il nuovo Iban e premi Invio"+ 
                                  "\n altrimenti premi Invio+ESC per tornare al menu'");
                //leggo l'iban
                Console.Write("\nIBAN : ");
                ibanNuovo = Console.ReadLine();

                //se l'utente ha digitato qualcosa, allora viene considerato come tentativo
                if (!(ibanNuovo == ""))
                {
                        //primo caso: punti e virgola inseriti all'interno del form
                        if (ibanNuovo.Contains(";") || (ibanNuovo.Contains("#")))
                {
                    rigaCorrotta = true;
                }

                //secondo caso: riga correttamente digitata
                else
                {
                    //controllo la validità dell'iban
                    if (!ControlloIban(ibanNuovo))
                    {
                        //in questo caso il codice immesso è errato
                        rigaCorrotta = true;
                    }
                    else
                        rigaCorrotta = false;
                }

                    //1° caso: iban inserito correttamente

                    if (!rigaCorrotta)
                    {
                        Console.WriteLine("\nIBAN valido. Per conferma premi Invio");
                    }

                    //2° caso: iban errato

                    else
                    {
                    //avverto il cliente che l'iban digitato non è corretto
                    Console.Write("<IBAN ERRATO>. Premi Invio");
                    }
                }

            tastoConferma = Console.ReadKey();
            }
            //rimango nel ciclo solo se la linea è corrotta e premo invio
            while ((rigaCorrotta)&&(tastoConferma.Key == ConsoleKey.Enter));

            //se la riga immessa è valida provvedo alla scrittura del file
            if (!rigaCorrotta)
            {
                //dichiaro una lista di appoggio per gli utenti che hanno un Iban
                List<string> utenti = new List<string>();

                //controllo quali sono le impostazioni selezionate (codice utente)
                utenti = LeggiFile(fileUtenti, 0);

                //Calcolo la spesa media del cliente controllando tutti i suoi acquisti e sommandone gli importi
                for (int i = 1; i < utenti.Count; i++)
                {
                    //se l'utente è quello registrato, copio l'iban
                    if (utenti[i] == utente)
                    {
                        ibanAttuale = LeggiFile(fileUtenti, 5)[i];
                        indiceRiga = i;
                        Console.WriteLine(indiceRiga + "-" + ibanNuovo);
                    }
                }


                //provvedo ad eseguire la scrittura su file
                StreamReader letturaFile;
                StreamWriter scritturaFile;
                string rigaLetta;

                letturaFile = File.OpenText(fileUtenti);
                scritturaFile = File.CreateText(fileTemporaneo);

                //imposto un try-catch per salvaguardare possibili eccezioni
                try
                {
                    while (!letturaFile.EndOfStream)                             //lettura rubrica sulla stringa sr
                    {
                        //leggo la riga dal primo file 
                        rigaLetta = letturaFile.ReadLine();

                        if ((indiceRiga == 0))
                        {
                            //se la voce non è quella selezionata lo riscrivo
                            rigaLetta = rigaLetta.Replace(ibanAttuale, (ibanNuovo + '#'));
                            Console.WriteLine(rigaLetta);
                        }
                        //scrivo la riga del file, opportunamente modificata nel caso
                        scritturaFile.WriteLine(rigaLetta);
                        indiceRiga--;
                    }

                    Console.WriteLine("\nOperazione completata con successo"+ 
                                      "\nPremere Invio per tornare al menu' ");
                    Console.ReadLine();
                }
                //adesso eseguo la cascata dei catch
                catch (System.IO.FileLoadException)
                {
                    Console.WriteLine("\nAttenzione: operazione di caricamento file non riuscita."+
                                      "\nRipetere l'operazione o contattare l'assistenza"+
                                      "\nPremere invio per tornare al menu' ");
                    Console.ReadLine();
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("\nAttenzione: operazione di I/O non consentita." +
                                      "\nRipetere l'operazione o contattare l'assistenza" +
                                      "\nPremere invio per tornare al menu' ");
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nAttenzione: eccezione " + e.Message +
                                      "\nRipetere l'operazione o contattare l'assistenza" +
                                      "\nPremere invio per tornare al menu' ");
                    Console.ReadLine();
                }

                //adesso eseguo il rilascio delle risorse
                finally
                {
                    //chiudo entrambi i file

                    if (letturaFile!=null)
                        letturaFile.Close();

                    if (scritturaFile != null)
                        scritturaFile.Close();
                
                    File.Delete(fileUtenti);
                    File.Copy(fileTemporaneo, fileUtenti);
                    File.Delete(fileTemporaneo);
                }
            }
        }

        //funzione che comporta un'uscita dal main menu
        static int ConfermaUscita()
        {
            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo tasto;

            //cancello schermo e scrivo messaggio di conferma
            Console.Clear();
            Console.WriteLine("Conferma chiusura programma premendo Invio" +
                "\n\nAltrimenti premi qualsiasi altro pulsante per tornare al menu'");

            //leggo tasto digitato dall'utente
            tasto = Console.ReadKey();


            //se il tasto è "Invio" esco, altrimenti torno al menu' 
            if (tasto.Key == ConsoleKey.Enter)
                return 1;

            else
                return 0;
        }

        //funzione che registra il cliente appena entrato
        static bool RegistraCliente()
        {
            bool rigaCorrotta = false; //dichiaro il segnalatore che indica la riga corrotta

            int esito;                 //imposto qui il valore restituito dalla scrittura dell'utente

            string nome, //dichiaro le variabili necessarie nel form di compilazione
                cognome,
                email,
                password,
                iban,
                avviso="",
                lineaUtente;

            ConsoleKeyInfo tastoConferma;

            bool registrato = false;
            do
            {
                //cancella lo schermo
                Console.Clear();

                if (rigaCorrotta)
                    Console.WriteLine("ATTENZIONE: Form completato in maniera errata" +"\n"+avviso);


                //inserisco tutti i campi necessari
                Console.WriteLine("\n Completa la registrazione inserendo i tuoi dati.");
                Console.WriteLine("\n Non e' consentito utilizzare ';' o '#' nei dati");
                Console.WriteLine("\n Ad ogni dato inserito premi Invio");
                Console.Write("\n\nNome : ");
                nome = Console.ReadLine();
                Console.Write("\nCognome : ");
                cognome = Console.ReadLine();
                Console.Write("\nEmail : ");
                email = Console.ReadLine();
                Console.Write("\nPassword : ");
                password = Console.ReadLine();
                Console.Write("\nIBAN : ");
                iban = Console.ReadLine();

                string rigaCompleta = nome + cognome + email + password;

                //primo caso: punti e virgola inseriti all'interno del form
                if (rigaCompleta.Contains(";")||(rigaCompleta.Contains("#")))
                {
                    rigaCorrotta = true;
                }

                //secondo caso: riga correttamente digitata
                else
                {
                    //controllo la validità dell'iban
                    if (!ControlloIban(iban))
                    {
                        //in questo caso il codice immesso è errato
                        avviso = "\nIl codice IBAN che hai immesso e' errato";
                        rigaCorrotta = true;
                    }
                    else
                    {
                        //se il codice è corretto controllo la mail(se corrotta o già presente in archivio)
                        if (!ControlloMail(email) || (!email.Contains("@")) || (!email.Contains(".")))
                        {
                            //in questo caso la mail immessa è già registrata
                            avviso = "\nLa mail inserita e' gia' presente nel database oppure è corrotta";
                            rigaCorrotta = true;
                        }
                        else
                            rigaCorrotta = false;

                    }
                    if (rigaCorrotta)
                    {
                        Console.WriteLine("\nPremi Invio per riprovare ad accedere," +
                        "\n per tornare al menu' premi un qualunque altro tasto ");
                    }
                    else
                    {
                        Console.WriteLine("\nAccesso consentito. Premi Invio per tornare al menu'.");
                    }

                }
            tastoConferma = Console.ReadKey();
            }
            while ((rigaCorrotta) && (tastoConferma.Key == ConsoleKey.Enter)) ;


            //caso confermato: il cliente è registrato
            if (!rigaCorrotta)
            {
                    //ho consentito la registrazione, quindi creo un'istanza di Cliente
                    //a cui allegherò tutte le info racimolate
                    Cliente clienteCorrente = new Cliente
                    {
                        Nome = nome,
                        Cognome = cognome,
                        Email = email,
                        PW = password,
                        Iban = iban
                    };

                    //controllo esistenza del file
                    if (!(File.Exists(fileUtenti)))
                    registrato = false;

                    else
                    {
                        //qui verrà memorizzata la lista delle mail memorizzate sul file
                        List<string> elencomail = new List<string>();

                        //recupero una lista con l'elenco delle mail associate (scopo conteggio)
                        elencomail = LeggiFile(fileUtenti, 1);

                        //scrivo una linea con i dati dell'utente da importare nel file
                        lineaUtente =elencomail.Count + ";"
                                                    + email + ";"
                                                    + password + ";"
                                                    + nome + ";"
                                                    + cognome + ";"
                                                    + iban + "#";

                        //scrivo l'utente sul file
                        esito=ScriviSuFile(fileUtenti, lineaUtente);

                        if (esito == 1)
                        registrato = true;
                    }
            }
            return registrato;
        }

        //funzione che controlla se la mail è già inserita in archivio
        static bool ControlloMail(string mailDaControllare)
        {
            //dichiaro una variabile che funge da segnalatore
            bool esito = true;

        
        //qui verrà memorizzata la lista delle mail memorizzate sul file
        List<string> elencomail = new List<string>();

        //recupero una lista con l'elenco delle mail associate
        elencomail = LeggiFile(fileUtenti, 1);

        //costruisco un'altra lista per elencare le voci da visualizzare
        for (int i = 0; i < (elencomail.Count); i++)
        {

            //se la mail corrisponde ad un'altra, avverto l'utente di esse già registrato
            if ((elencomail[i].ToLower()) == mailDaControllare.ToLower())
            {
                //se l'email esiste già, provvedo ad evitare un'ulteriore registrazione
                Console.WriteLine("\n La tua mail è già registrata! Accedi al sito...");
                esito = false;
                Console.ReadLine();
            }
        }

        //restituisco esito della computazione
        return esito;
        }


        //funzione che controlla se l'iban inserito è corretto
        static bool ControlloIban (string stringaIban)
        {
            bool esito = false;
            //imposto un try-catch per evitare che i dati siano caricati in maniera incoerente
            try
            {

                //dichiaro le variabili locali
                string inizioIban = "",
                       restoStringa;

                int conteggioCaratteri = 0;

                //calcolo la lunghezza della stringa fornita
                conteggioCaratteri = stringaIban.Length;

                //se l'ordine non utilizza un iban italiano, non concedo l'immissione
                if (stringaIban.Substring(0, 2) != "IT")
                    conteggioCaratteri = 0;

                //controllo la stringa solo se la sua lunghezza è di 27 caratteri in Italia)
                //in caso contrario, l'IBAN sarà sicuramente errato
                if (conteggioCaratteri==27)
                {
                    //sviluppo i calcolo riguardanti la verifica dell'iban
                    inizioIban = stringaIban.Substring(0, 4);

                    //sostituisco 'IT' con il corrispondente codice carattere
                    inizioIban = inizioIban.Replace("IT", "1829");

                    //rimuovo dalla stringa i caratteri che ho letto in precedenza
                    stringaIban = stringaIban.Remove(0, 4);

                    //sostituisco le eventuali lettere con il valore
                    stringaIban = stringaIban.Replace("A","10");
                    stringaIban = stringaIban.Replace("B", "11");
                    stringaIban = stringaIban.Replace("C", "12");
                    stringaIban = stringaIban.Replace("D", "13");
                    stringaIban = stringaIban.Replace("E", "14");
                    stringaIban = stringaIban.Replace("F", "15");
                    stringaIban = stringaIban.Replace("G", "16");
                    stringaIban = stringaIban.Replace("H", "17");
                    stringaIban = stringaIban.Replace("I", "18");
                    stringaIban = stringaIban.Replace("J", "19");
                    stringaIban = stringaIban.Replace("K", "20");
                    stringaIban = stringaIban.Replace("L", "21");
                    stringaIban = stringaIban.Replace("M", "22");
                    stringaIban = stringaIban.Replace("N", "23");
                    stringaIban = stringaIban.Replace("O", "24");
                    stringaIban = stringaIban.Replace("P", "25");
                    stringaIban = stringaIban.Replace("Q", "26");
                    stringaIban = stringaIban.Replace("R", "27");
                    stringaIban = stringaIban.Replace("S", "28");
                    stringaIban = stringaIban.Replace("T", "29");
                    stringaIban = stringaIban.Replace("U", "30");
                    stringaIban = stringaIban.Replace("V", "31");
                    stringaIban = stringaIban.Replace("W", "32");
                    stringaIban = stringaIban.Replace("X", "33");
                    stringaIban = stringaIban.Replace("Y", "34");
                    stringaIban = stringaIban.Replace("Z", "35");


                    //aggiungo in coda al seriale i caratteri letti
                    stringaIban += inizioIban;

                    //divido la stringa in parti da 8 cifre fin quando non rimangono vuote
                    //ed aggiungo il resto alla sezione successiva
                    do
                    {
                        restoStringa = stringaIban.Substring(0, 8);
                        stringaIban=stringaIban.Remove(0, 8);
                        stringaIban = Convert.ToString(int.Parse(restoStringa) % 97) + stringaIban;
                    }
                    while (stringaIban.Length>8);

                    //se il conteggio dei caratteri è maggiore di 5 ed il resto corrisponde, l'iban può essere corretto
                    if (((int.Parse(stringaIban)) % 97 == 1))
                    {
                         esito = true;
                    }
                    else
                       esito = false;
                }

            }
            catch (FormatException a)
            {
                Console.WriteLine("Attenzione: formato Iban non riconosciuto\n" + a.Message);
                esito = false;

            }
            catch
            {
                Console.WriteLine("Attenzione, qualche dato non e' corretto.\nRiesegui la compilazione");
                esito = false;
            }
            return esito;
        }

        //funzione che gestisce il login dell'utente
        static string AccessoCliente()
        {
            //dichiaro un intero che mi servirà da indice
            int trovato = 0;

            //dichiaro le stringhe che serviranno per memorizzare il codice utente, mail e password
            string utente = "",
                password,
                email;

            //dichiaro una variabile per la scelta del pulsante
            ConsoleKeyInfo log;

            //qui verrà memorizzata la lista delle mail memorizzate sul file
            List<string> elenco = new List<string>();







            //imposto un ciclo per inserire mail e password
            do {
                //cancello lo schermo
                Console.Clear();

                //resetto la lista eventualmente ancora aperta
                elenco.Clear();

                Console.WriteLine("\nDigita email e password per l'autenticazione ");
                Console.Write("\nEmail : ");
                email = Console.ReadLine();
                Console.Write("\nPassword : ");
                password = Console.ReadLine();

                //recupero una lista con l'elenco delle mail associate
                elenco = LeggiFile(fileUtenti, 1);

                //costruisco un'altra lista per elencare le voci da visualizzare
                for (int i = 0; i < (elenco.Count); i++)
                {

                    //se la mail corrisponde ad un'altra, allora l'utente è registrato
                    //quindi recupero l'indice per poter verificare anche la password
                    if ((elenco[i]) == email)
                    {
                        trovato = i;
                    }
                }

                elenco.Clear();

                //recupero una lista con l'elenco delle password
                elenco = LeggiFile(fileUtenti, 2);

                //caso positivo: mail e password corrispondono -> l'utente è autenticato
                if (elenco[trovato] == password)
                {
                    Console.WriteLine("\n Login Validato. Premi Invio per tornare al menu'");

                    elenco.Clear();

                    //recupero una lista con l'elenco delle password
                    elenco = LeggiFile(fileUtenti, 0);
                    utente = elenco[trovato];

                }
                else
                {
                    Console.WriteLine("\n Accesso negato: premi Invio per ritentare," +
                                      "\noppure un qualsiasi tasto per tornare al menu");
                }

                log = Console.ReadKey();
            }
            while ((log.Key == ConsoleKey.Enter) && (utente == ""));

            return utente;
        }


        //funzione che scrive una riga su un fie selezionato e poi restituisce l'esito della scrittura
        static int ScriviSuFile(string fileDaScrivere, string rigaDaScrivere)
        {
            //dichiaro la variabile locale che conterrà l'esito
            int esito=0;

            //dichiaro un flusso in scrittura
            StreamWriter scrittura;
            scrittura = File.AppendText(fileDaScrivere);
            //controllo se la scrittura va a buon fine
            try
            {
                //controllo esistenza del file
                if (!(File.Exists(fileDaScrivere)))
                    esito = 0;

                //se il file esiste provvedo alla scrittura
                else
                {
                scrittura.WriteLine(rigaDaScrivere);
                esito = 1;
                }
            }
            //imposto un'eccezione generica
            catch (Exception e)
            {
                Console.WriteLine("\nAttenzione: errore in scrittura del file:" +
                    "\n Eccezione gestita" + e.Message);
            esito = 0;
            }
            finally
            {
                //chiudo il file
                if (scrittura!=null)
                    scrittura.Close();
            }
            //restituisco l'esito della scrittura
            return esito;

        }


        //funzione che pubblica i parametri di gradimento dell'utente 
        //in base alle stime sugli acquisti
        static void Gradimento(string utente)
        {

            //dichiarazione variabili
            double x = 1,
                   y,
                   TotaleUtente;

            //Cancello lo schermo
            Console.Clear();

            //ottengo l'importo totale speso dal cliente con i suoi ordini
            TotaleUtente = CalcolaSpesaMedia(utente);

            //stimo l'affidabilità media del cliente
            x = StimaAffidabilita(utente);

            //in questo caso non sono stati trovati acquisti, 
            //oppure l'ordine non è ancora considerato evaso
            if(x==0)
            {
                Console.WriteLine("\nValore x: " + x);

                //informo il cliente della situazione attuale
               Console.WriteLine("\nNon hai ancora un indice di gradimento.");
                Console.WriteLine("\n\nNon hai ancora effettuato un acquisto oppure " +
                                  "\nil tuo ultimo ordine non e' ancora stato evaso" +
                                  "\n\nPremi invio per tornare al menu");
                Console.ReadLine();
            }
            //in questo caso ho rilevato
            else
            {
                //in questo caso comunico al cliente i dati rilevati dall'archivio
                y = (1/x)*TotaleUtente +1;
                y = Math.Sqrt(y);

                //informo il cliente del suo punteggio totale
                Console.WriteLine("\n\nIl tuo punteggio attuale e': "+ Math.Round(y, 6));
                //a partire dal precedente valore posso impostare sconti/promozioni
                //ai clienti che ottengono punteggi più alti

                //indico al cliente la procedura da effettuare
                Console.WriteLine("\nPremi Invio per tornare al menu' principale.");

                //attendo la conferma di lettura da parte dell'utente
                Console.ReadLine();
            }
        }

        static void Pagamenti(string utente)
        {   //dichiaro le variabili locali
            int scelta,
                esito=0;

            bool nuovoUtente = true;

            string IBAN = "",
                stringaCodice,
                messaggio;

            //qui verrà memorizzata la lista degli ordini in sospeso      
            List<string> Commissionati = new List<string> (LeggiFile(fileOrdini, 0));
            List<string> Mittenti = new List<string>(LeggiFile(fileOrdini, 1));

            //qui verrà memorizzata la lista degli ordini pagati
            List<string> Pagati = new List<string>(LeggiFile(filePagati,2));

            //qui verrà memorizzata la lista degli ordini da pagare
            List<string> InSospeso = new List<string>();

            //costruisco una lista per controllare gli utenti (pagamenti in sospeso)
            for (int i = 0; i < (Mittenti.Count); i++)
            {
                //se l'utente corrisponde, effettuo un ulteriore controllo
                if (Mittenti[i]==utente)
                {

                    //in questo blocco vi saranno solo gli ordini commissionati dall'utente in questione
                    //quindi fino alla nuova smentita sono tutti in sospeso
                    InSospeso.Add(Commissionati[i] + " - " + LeggiFile(fileOrdini, 2)[i] + " - " + LeggiFile(fileOrdini, 4)[i]+" euro");

                    //costruisco una lista per controllare le voci da visualizzare (ordini pagati)
                    for (int j = 0; j < (Pagati.Count); j++)
                    {
                        //caso 1: l'ordine è stato pagato, quindi lo rimuovo
                        if (Pagati[j] == Commissionati[i])
                        {
                            InSospeso.Remove(Commissionati[i] + " - " + LeggiFile(fileOrdini, 2)[i] + " - " + LeggiFile(fileOrdini, 4)[i] + " euro");
                            IBAN = LeggiFile(filePagati, 6)[j];

                            //se trovo un pagamento, l'utente non è nuovo
                            nuovoUtente = false;
                        }

                    }
                }
            }

            //recupero l'iban dall'elenco clienti
            for(int k = 0; k < (LeggiFile(fileUtenti, 0)).Count;k++)
            {
                if (utente == LeggiFile(fileUtenti, 0)[k])
                    IBAN = LeggiFile(fileUtenti, 5)[k];

            }

            //se è il caso di un utente associato (quindi Iban caricato), 
            //allora mostro l'IBAN
            if (!nuovoUtente)
            {
                //tolgo all'IBAN il carattere di finestringa '#'
                IBAN = IBAN.Remove(IBAN.Length - 1);
            }

            //se non vi sono pagamenti in sospeso avverto subito l'utente
            if (InSospeso.Count==0)
            {
                //cancello lo schermo
                Console.Clear();
                if(nuovoUtente)
                { 
                    //info di status del cliente se non ha acquisti)
                    Console.WriteLine("Status: *Nuovo Cliente*");
                }
                else
                {
                    //se ho trovato almeno un pagamento, l'utente è già associato (ed ha già utilizzato l'IBAN)
                    Console.WriteLine("\nStatus: *Cliente Associato*   [IBAN: " + IBAN + "]");
                }

                //messaggio di info al cliente
                Console.WriteLine("\n Attualmente non hai pagamenti in sospeso...");
                Console.WriteLine("\n Premi Invio per tornare al menu' principale.");
                Console.ReadLine();
            }
            else
            {
                //aggiungo la voce di uscita al menu'
                InSospeso.Add("\tTorna al menu' principale.\t");

                do
                {//cancello lo schermo
                    Console.Clear();

                    if (nuovoUtente)
                    {
                        //info di status del cliente se non ha acquisti)
                        messaggio="\nStatus: *Nuovo Cliente*";
                    }
                    else
                    {
                        //se ho trovato almeno un pagamento, l'utente è già associato (ed ha già utilizzato l'IBAN)
                        messaggio="\nStatus: *Cliente Associato*   [IBAN: " + IBAN + "]";
                    }


                    //info di status del cliente
                    scelta = Menu(messaggio +
                                  "\n\nSeleziona l'ordine che vuoi pagare"+
                                  "\n\nCod.      Data        Importo", InSospeso);


                    Console.WriteLine(scelta);

                    //se l'utente non ha richiesto di uscire, provvedo a pagare il suo ordine
                    if((scelta+1)!=InSospeso.Count)
                    {
                        //prendo in carico la stringa selezionata
                        stringaCodice = InSospeso[scelta];

                        //Eseguo il pagamento come impostato dall'utente sul codice selezionato
                        esito = Pagamento(utente, int.Parse(stringaCodice.Split('-')[0]),IBAN);

                    }
                }
                //esco dal ciclo solo quando l'utente chiede di tornare al menu'
                while (((scelta + 1) != InSospeso.Count)&&(esito!=1));
            }
        }


        //funzione che permette di eseguire un ordine
        static void Ordina(string Loggato)
        {
            string messtopologia; //messaggio di intestazione del menu topologia

            //cancella lo schermo
            Console.Clear();

            //List<TipoProdotto> tipoProdotti = getTipoProdotti();
            messtopologia = "Seleziona la tipologia di ordine che vuoi effettuare e premi invio.";

            //la scelta del tipo è fornita dalle specifiche: MECCANICA PESANTE o COMPONENTISTICA
            string[] sceltaTipo = { "MECCANICA PESANTE", "COMPONENTISTICA", "Torna al menù principale" };


            //viene invocato il menu di scelta
            int tipoProdotto = Menu(messtopologia, sceltaTipo);

            //entro nel menu di scelta se non sono nell'opzione torna al menu principale
            if (tipoProdotto != (sceltaTipo.Length - 1))
            {
                //creazione di un carrello (oggetto che contiene l'elenco totale degli oggetti da acquistare)
                Carrello carrello = new Carrello(int.Parse(Loggato), tipoProdotto, new List<Ordine>(), 0);

                //seleziono i prodotti da mettere nel carrello
                SelezionaProdotti(carrello, tipoProdotto);

            }

        }

        static int Pagamento(string utentePagatore, int ordineDaPagare,string iban)
        {
            //dichiaro una variabile contenente l'esito dell'operazione
            int esito = 0;

            //dichiaro le variabili locali
            string cod_Tipo,
                importo,
                linea;
            DateTime DataOdierna = DateTime.Today;

            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo confermaAquisto;

            //cancello lo schermo
            Console.Clear();

            Console.WriteLine("\n\nPer confermare il pagamento dell'ordine premi Invio" +
                              "\nAltrimenti premi un tasto qualunque");
            //leggo il tasto scelto dall'utente
            confermaAquisto = Console.ReadKey(true);


            //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
            if (confermaAquisto.Key == ConsoleKey.Enter)
            {
                //recupero il codice tipologia e l'importo dell'ordine
                cod_Tipo = LeggiFile(fileOrdini, 3)[ordineDaPagare];
                importo = LeggiFile(fileOrdini, 4)[ordineDaPagare];
                
                //imposto la linea da scrivere sul file
                linea=LeggiFile(filePagati, 0).Count+
                                  ";" + utentePagatore +
                                  ";" + Convert.ToString(ordineDaPagare) +
                                  ";" + LeggiFile(fileOrdini, 3)[ordineDaPagare]+
                                  ";" + DataOdierna.ToString("d") +                            
                                  ";" + LeggiFile(fileOrdini, 4)[ordineDaPagare] +
                                  ";" + iban;

                //trascrivo il pagamento appena effettuato sul file
                esito = ScriviSuFile(filePagati, linea);

            }

            if (esito == 1)
            {
                Console.Write("\nPagamento completato!" +
                              "\nPremi Invio per tornare al menu'.");
                Console.ReadLine();
            }
            else
            {
                Console.Write("\nX Pagamento non confermato! X\nPremi Invio per tornare al menu'.");
                Console.ReadLine();
            }
            //questa variabile restituirà 1 se il carrello è stato acquistato
            //oppure 0 nel caso in cui l'utente abbia annullato la scelta
            return esito;
        }


        //funzione che seleziona i prodotti facenti parte di una stessa categoria oggetto
        static void SelezionaProdotti(Carrello carrello, int categoriaOggetto)
        {
            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo tasto;

            //cancello lo schermo
            Console.Clear();

            string messSceltaProdotto = "Scegli l'oggetto da acquistare";

            string codiceRilevato;

            int codProd = 10;

            //qui verrà memorizzata la lista delle opzioni acquistabili
            List<string> selezionate = new List<string>();

            //qui verrà memorizzata la lista delle opzioni che verranno mostrate
            List<string> daMostrare = new List<string>();

            //controllo quali sono le impostazioni selezionate
            selezionate = LeggiFile(fileListino, 0);

            //costruisco un'altra lista per elencare le voci da visualizzare
            for (int i = 1; i < (selezionate.Count); i++)
            {

                //se il codice prodotto corrisponde, aggiungo l'oggetto alla lista da mostrare
                if (int.Parse(selezionate[i]) == categoriaOggetto)
                {
                    daMostrare.Add(LeggiFile(fileListino, 3)[i]);
                }
            }
            //aggiungo l'a scelta di poter tornare al menu' principale
            daMostrare.Add("Torna al menu' principale");

            switch (carrello.CodTipologia)
            {
                case 0:
                    codProd = Menu(messSceltaProdotto, daMostrare); //ottengo il codice prodotto categoria 0
                    break;                                          
                case 1:
                    codProd = Menu(messSceltaProdotto, daMostrare); //ottengo il codice prodotto categoria 1
                    break;
                case 2:
                    MainMenu(Convert.ToString(carrello.CodUtente)); //cliente vuole tornare al menù precedente
                    break;
                default:
                    MainMenu(Convert.ToString(carrello.CodUtente)); //scelta non valida
                    break;
            }

            //se non sono nell'ultima opzione consento all'utente di uscire
            if(codProd!=(daMostrare.Count-1))
            {

                //ricava l'indice dell'ordine con cui l'oggetto si trova nel file
                int indiceOggetto = 0,
                    temp = codProd + 1,           //variabile temporanea
                    trovato = selezionate.Count + 1;



                //si ricava il prezzo dell'oggetto
                //costruisco un'altra lista per elencare le voci da visualizzare
                for (int i = 1; i < (selezionate.Count); i++)
                {

                    if ((int.Parse(LeggiFile(fileListino, 0)[i]) == carrello.CodTipologia))
                    {
                        temp--;
                    }
                    if ((temp == 0) && (trovato > selezionate.Count)) //se trovo il valore non entro più in questa condizione
                    {
                        trovato = int.Parse(LeggiFile(fileListino, 2)[i]);
                        indiceOggetto = i;
                    }
                }

                //controllo quali sono le impostazioni selezionate
                selezionate = LeggiFile(fileListino, 4);

                //trovo il prezzo unitario dell'oggetto (in formato stringa)
                string prezzoOggetto = selezionate[indiceOggetto];

                //controllo quali sono le impostazioni selezionate (lettura modalità)
                selezionate = LeggiFile(fileListino, 1);

                carrello.StringTipo = selezionate[indiceOggetto];


                //rilevo il codice identificativo dell'oggetto sul file
                selezionate = LeggiFile(fileListino, 2);

                codiceRilevato = selezionate[indiceOggetto];


                //su 'prod' avrò la tipologia di prodotto che intendo aquistare
                //adesso provvedo a ricavare la quantità di merce da acquistare
                string messQuanti = "Prezzo unitario: " + prezzoOggetto + " euro\n\nIndicare la quantità di " + daMostrare[codProd] + " che si desidera acquistare\n" +
                    "\n     Confermare premendo Invio.";
                Console.WriteLine(messQuanti);


                //inizializzo la quantità
                int quantità = 0;

                //ciclo di rilevazione di una quantità valida (formato Int, modulo > 0)
                while (quantità == 0)
                {
                    string stringQuantità = Console.ReadLine();
                    if (!int.TryParse(stringQuantità, out quantità) || quantità == 0)
                    {
                        //errore rilevato: impostato un numero non maggiore di zero o altro valore non valido
                        Console.Clear();
                        Console.WriteLine(messQuanti + "\n\nRiprovare!\n" +
                        "E' necessario inserere un formato numerico maggiore di '0' \n");
                    }
                }


                List<Ordine> ordini = carrello.Ordini;     //lista degli ordini attualmente aperta


                carrello.Ordini = ordini;    //qui si trovano tutti gli ordini della sessione attualmente aperta


                Console.Clear();

                //mostro al cliente la tabella riassuntiva dell'acquisto
                Console.WriteLine("         #Resoconto acquisto#\n");
                Console.WriteLine("Prodotto : " + daMostrare[codProd] + ", Quantità : " + quantità +
                                    "\nPrezzo unitario : " + Double.Parse(prezzoOggetto) + ", Importo totale: " + quantità * (Double.Parse(prezzoOggetto)) + " euro ");


                //indico al cliente le modalità di conferma o rigetto
                Console.WriteLine("\n Per confermare premi Invio, qualsiasi tasto per annullare l'operazione.");

                //leggo il tasto scelto dall'utente
                tasto = Console.ReadKey(true);

                //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
                if (tasto.Key == ConsoleKey.Enter)
                {
                    //verrà generato un nuovo ordine ()
                    Ordine ordineCorrente = new Ordine(int.Parse(codiceRilevato), quantità, daMostrare[codProd]);  // dichiaro il nuovo ordine corrente



                    //aggiungo al carrello l'importo parziale
                    carrello.TotImporto = carrello.TotImporto + (ordineCorrente.Quantità * (double.Parse(prezzoOggetto)));
                    Console.WriteLine("\nImporto totale del carrello: " + carrello.TotImporto + " euro");
                    Console.ReadLine();

                    //aggiungo l'ordine corrente all'elenco ordini
                    ordini.Add(ordineCorrente);
                    ScegliOperazioneProdotti(carrello);
                }
                else
                {
                    //informo il cliente sull'annullamento dell'operazione
                    Console.WriteLine("\n\n Operazione annullata.   Per continuare premi Invio");
                    Console.ReadLine();

                    //consento al cliente di selezionare qualche altro oggetto dopo l'annullamento dell'operazione
                    SelezionaProdotti(carrello, carrello.CodTipologia);
                }



            }
        }


        //aggiunge un prodotto ad un carrello pre-esistente
        static void AggiungiProdotto(Carrello carrello, int categoriaOggetto)
        {
            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo tasto;

            //cancello lo schermo
            Console.Clear();

            string messSceltaProdotto = "Scegli l'oggetto da acquistare";

            string codiceRilevato;

            int codProd = 10;

            //qui verrà memorizzata la lista delle opzioni acquistabili
            List<string> selezionate = new List<string>();

            //qui verrà memorizzata la lista delle opzioni che verranno mostrate
            List<string> daMostrare = new List<string>();

            //controllo quali sono le impostazioni selezionate
            selezionate = LeggiFile(fileListino, 0);

            //costruisco un'altra lista per elencare le voci da visualizzare
            for (int i = 1; i < (selezionate.Count); i++)
            {

                //se il codice prodotto corrisponde, aggiungo l'oggetto alla lista da mostrare
                if (int.Parse(selezionate[i]) == categoriaOggetto)
                {
                    daMostrare.Add(LeggiFile(fileListino, 3)[i]);
                }
            }

            switch (carrello.CodTipologia)
            {
                case 0:
                    codProd = Menu(messSceltaProdotto, daMostrare); //ottengo il codice prodotto 
                    break;                                          // della sua categoria: 0,1
                case 1:
                    codProd = Menu(messSceltaProdotto, daMostrare);
                    break;
                case 2:
                    MainMenu(Convert.ToString(carrello.CodUtente));
                    break;
                default:
                    MainMenu(Convert.ToString(carrello.CodUtente));
                    break;
            }



            //ricava l'indice dell'ordine con cui l'oggetto si trova nel file
            int indiceOggetto = 0,
                temp = codProd + 1,           //variabile temporanea
                trovato = selezionate.Count + 1;



            //si ricava il prezzo dell'oggetto
            //costruisco un'altra lista per elencare le voci da visualizzare
            for (int i = 1; i < (selezionate.Count); i++)
            {

                if ((int.Parse(LeggiFile(fileListino, 0)[i]) == carrello.CodTipologia))
                {
                    temp--;
                }
                if ((temp == 0) && (trovato > selezionate.Count)) //se trovo il valore non entro più in questa condizione
                {
                    trovato = int.Parse(LeggiFile(fileListino, 2)[i]);
                    indiceOggetto = i;
                }
            }

            //controllo quali sono le impostazioni selezionate
            selezionate = LeggiFile(fileListino, 4);

            //trovo il prezzo unitario dell'oggetto (in formato stringa)
            string prezzoOggetto = selezionate[indiceOggetto];

            //controllo quali sono le impostazioni selezionate (lettura modalità)
            selezionate = LeggiFile(fileListino, 1);

            carrello.StringTipo = selezionate[indiceOggetto];


            //rilevo il codice identificativo dell'oggetto sul file
            selezionate = LeggiFile(fileListino, 2);

            codiceRilevato = selezionate[indiceOggetto];


            //su 'prod' avrò la tipologia di prodotto che intendo aquistare
            //adesso provvedo a ricavare la quantità di merce da acquistare
            string messQuanti = "Prezzo unitario: " + prezzoOggetto + " euro\n\nIndicare la quantità di " + daMostrare[codProd] + " che si desidera acquistare\n" +
                "\n     Confermare premendo Invio.";
            Console.WriteLine(messQuanti);


            //inizializzo la quantità
            int quantità = 0;

            //ciclo di rilevazione di una quantità valida (formato Int, modulo > 0)
            while ((quantità == 0)||(quantità>1000))
            {
                string stringQuantità = Console.ReadLine();
                if (!int.TryParse(stringQuantità, out quantità) || quantità == 0)
                {
                    //errore rilevato: impostato un numero non maggiore di zero o altro valore non valido
                    Console.Clear();
                    Console.WriteLine(messQuanti + "\n\nRiprovare!\n" +
                    "E' necessario inserere un formato numerico intero maggiore di '0' \n"+
                    "e minore di '1.000' [limite massimo di consegna]");
                }
            }


            List<Ordine> ordini = carrello.Ordini;     //lista degli ordini attualmente aperta


            carrello.Ordini = ordini;    //qui si trovano tutti gli ordini della sessione attualmente aperta

            Console.Clear();

            //mostro al cliente la tabella riassuntiva dell'acquisto
            Console.WriteLine("         #Resoconto acquisto#\n");
            Console.WriteLine("Prodotto : " + daMostrare[codProd] + ", Quantità : " + quantità +
                                "\nPrezzo unitario : " + Double.Parse(prezzoOggetto) + ", Importo totale: " + quantità * (Double.Parse(prezzoOggetto)) + " euro ");


            //indico al cliente le modalità di conferma o rigetto
            Console.WriteLine("\n Per confermare premi Invio, qualsiasi tasto per annullare l'operazione.");

            //leggo il tasto scelto dall'utente
            tasto = Console.ReadKey(true);

            //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
            if (tasto.Key == ConsoleKey.Enter)
            {
                //verrà generato un nuovo ordine ()
                Ordine ordineCorrente = new Ordine(int.Parse(codiceRilevato), quantità, daMostrare[codProd]);  // dichiaro il nuovo ordine corrente



                //aggiungo al carrello l'importo parziale
                carrello.TotImporto = carrello.TotImporto + (ordineCorrente.Quantità * (double.Parse(prezzoOggetto)));
                Console.WriteLine("\nImporto totale del carrello: " + carrello.TotImporto + " euro");
                Console.ReadLine();

                //aggiungo l'ordine corrente all'elenco ordini
                ordini.Add(ordineCorrente);
            }
            else
            {
                //informo il cliente sull'annullamento dell'operazione
                Console.WriteLine("\n\n Operazione annullata.   Per continuare premi Invio");
                Console.ReadLine();

                //consento al cliente di selezionare qualche altro oggetto dopo l'annullamento dell'operazione
                SelezionaProdotti(carrello, carrello.CodTipologia);
            }
        }

        //funzione che restituisce il cumulativo degli acquisti di un utente specificato
        static double CalcolaSpesaMedia(String utente)
        {
            //dichiaro le variabili locali
            double totale=0;
            int numeroAcquisti=0;

            //dichiaro una lista di appoggio per gli importi
            List<string> importi = new List<string>();

            //controllo quali sono le impostazioni selezionate (lettura prezzi pagamenti)
            importi = LeggiFile(filePagati, 5);

            //dichiaro una lista di appoggio per gli utenti che hanno pagato
            List<string> utenti = new List<string>();

            //controllo quali sono le impostazioni selezionate (lettura modalità)
            utenti = LeggiFile(filePagati, 1);

            //Calcolo la spesa media del cliente controllando tutti i suoi acquisti e sommandone gli importi
            for(int i=1; i<importi.Count;i++)
            {
                if (utenti[i]==utente)
                {
                    //se il cliente è lo stesso aggiungo i suoi importi
                    totale += double.Parse(importi[i]);
                    numeroAcquisti++;
                }
            }

            //restituisco la spesa media eseguita dal cliente
            return totale/numeroAcquisti;
        }

        //funzione che restituisce il numero di giorni impiegati dall'utente al pagamento
        static double StimaAffidabilita (string utente)
        {
            //dichiaro le variabili locali
            double giorniAttesa=0,
                   numeroAcquisti=0,
                   affidabilita;
            TimeSpan divario;

            //dichiaro una lista in cui elenco le date in cui sono confermati gli ordini
            List<string> ordiniAcquistati = new List<string>();

            //dichiaro una lista in cui elenco le date in cui sono effettuati gli acquisti
            List<string> dateAcquistati = new List<string>();

            //dichiaro una lista per elencare gli utenti
            List<string> utenti = new List<string>();

            //metto in una lista gli ordini che fanno fede agli univoci pagamenti
            ordiniAcquistati = LeggiFile(filePagati, 2);

            //elenco le date in cui sono stati pagati tali ordini
            dateAcquistati = LeggiFile(filePagati, 4);

            //controllo quali sono le impostazioni selezionate (lettura modalità)
            utenti = LeggiFile(filePagati, 1);

            //eseguo un ciclo su tutti gli ordini acquistati
            for (int i = 1; i < dateAcquistati.Count; i++)
            {
                //se l'utente corrisponde, sommo il totale
                if ((utente == utenti[i]))
                {
                    divario = (DateTime.Parse(dateAcquistati[i]) - DateTime.Parse(LeggiFile(fileOrdini, 2)[int.Parse(ordiniAcquistati[i])]));
                    giorniAttesa += divario.Days;
                    numeroAcquisti++;

                }
            }

            //aggiungo un controllo nel caso in cui l'utente non abbia ancora effettuato acquisti
            if (numeroAcquisti == 0)
            {
                //se il cliente non ha ancora eseguito acquisti, imposto soglia 0
                affidabilita = 0;
            }
            else
            {
                //se il cliente ha effettuato acquisti, uso una formula per calcolarne l'affidabilità
                affidabilita = (1/(numeroAcquisti+1))+(giorniAttesa/numeroAcquisti);
            }

            //restituisco il numero del conteggio medio (oppure 0 in caso di nessun acquisto/segnalazione avaria)
            return affidabilita;
        }

        //funzione che mostra le scelte quando un oggetto è nel carrello
        static void ScegliOperazioneProdotti(Carrello carrello)
        {
            int esci = 0; //dichiaro una variabile di uscita
            do
            {
                //cancello lo schermo
                Console.Clear();

                //carico le possibili opzioni da far scegliere al cliente
                string[] checkScelta = { "Aggiungi altri prodotti all'ordine.",
                                         "Visualizza contenuto carrello.",
                                         "Conferma ordine (carrello)",
                                         "Cancella contenuto carrello",
                                         "Vai al menu principale" };
                int scelta = Menu("",checkScelta);

                        //offro al cliente la possibilità di scegliere tra varie opzioni...
                        //ed esco dal menu' solo quando una delle funzioni restituisce 1
                        switch (scelta)
                        {
                            case 0:
                            //utilizzo questa fase quando il cliente 
                            //ha ancora intenzione di acquistare un oggetto
                            AggiungiProdotto(carrello, carrello.CodTipologia);
                            break;
                        case 1:
                            //il cliente sceglie di visualizzare il carrello
                            VisualizzaCarrello(carrello);
                            break;
                        case 2:
                            //il cliente sceglie di acquistare il contenuto del carrello
                            esci = CompraCarrello(carrello);
                            break;
                        case 3:
                            //il cliente sceglie di cancellare il contenuto attuale del carrello
                            esci = CancellaCarrello(carrello, false);
                            break;
                        case 4:
                            //opzione di ritorno al menu principale
                            esci = SceltaCanc(carrello);
                            break;
                        default:
                            MainMenu(Convert.ToString(carrello.CodUtente));
                            break;
                        }
                
            }
            //finchè non viene selezionata dall'utente un'opzione che implica l'uscita
            //il cliente rimane in questo menu' di selezione
            while (esci != 1);
        }

        //funzione che visualizza il contenuto di un carrello
        static void VisualizzaCarrello(Carrello carrello)
        {
            int i = 0; //dichiaro un puntatore

            //cancello lo schermo
            Console.Clear();

            Console.WriteLine("Tipologia di ordine : " + carrello.StringTipo);
            Console.WriteLine("\n\nContenuto del carrello :");

            foreach (Ordine ord in carrello.Ordini)
            {
                //se i non è uguale a zero, stampo la riga di separazione per ordini
                if (i != 0)
                {
                    Console.WriteLine("_________________________________");
                }
                Console.WriteLine("\n" + ord.DescrizioneProdotto + "   Quantita': " + ord.Quantità);

                //incremento i
                i++;
            }

            Console.WriteLine("\n\n\nPremi invio per tornare al menu'");
            Console.ReadLine();

        }


        //funzione che gestisce l'acquisto del carrello
        static int CompraCarrello(Carrello carrello)
        {
            //dichiaro una variabile contenente l'esito dell'operazione
            int esito=0;

            //dichiaro una stringa in cui verrà inserita la data
            DateTime dataAcq= DateTime.Today;

            carrello.DataAcquisto = dataAcq;

            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo confermaAquisto;

            //cancello lo schermo
            Console.Clear();

            Console.WriteLine("Per confermare l'ordinazione del carrello premi Invio" +
                              "\nAltrimenti premi un tasto qualunque");
            //leggo il tasto scelto dall'utente
            confermaAquisto = Console.ReadKey(true);

            //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
            if (confermaAquisto.Key == ConsoleKey.Enter)
            {

                //se l'utente conferma l'uscita cancello il carrello
                esito = Comprato(carrello);
            }

            if(esito==1)
            {
                Console.Write("\nOrdine concluso con successo."+
                              "\n\nPotrai sin da subito effettuare il pagamento dal menu' principale"+
                              "\n\nPremi Invio per continuare.");
                Console.ReadLine();
            }
            else
            {
                Console.Write("\nX Ordine non confermato! X\nPremi Invio per tornare al menu'.");
                Console.ReadLine();
            }
            //questa variabile restituirà 1 se il carrello è stato acquistato
            //oppure 0 nel caso in cui l'utente abbia annullato la scelta
            return esito;

        }


        //cancellazione carrello (con annesso booleano di conferma già ricevuta)
        static int CancellaCarrello(Carrello carrelloCanc, bool confermato)
        {
            int esito = 0;

            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo cancellaConf;

            //caso cancellazione carrello esplicita
            if (!confermato)
            {
                //cancello lo schermo
                Console.Clear();

                Console.WriteLine("Se sei sicuro di cancellare il carrello premi Invio" +
                    "               \nAltrimenti premi un tasto qualunque");


                //leggo il tasto scelto dall'utente
                cancellaConf = Console.ReadKey(true);

                if (cancellaConf.Key == ConsoleKey.Enter)
                    confermato = true;
            }

            
            //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
            //(mediante "torna al menu" o mediante "cancellazione diretta")
            if (confermato)
            {
                //effettuo l'operazione di cancellazione
                carrelloCanc.Ordini.Clear();

                if (carrelloCanc.Ordini.Count == 0)
                {
                    esito = 1;
                }
                else
                {
                    esito = carrelloCanc.Ordini.Count;
                }

                return esito;
            }
            else
                return 0;
        }


        //cancellazione nel caso di ritorno al menu' principale
        static int SceltaCanc(Carrello carrello)
        {
            int esito = 0;

            //dichiaro la rilevazione di un tasto premuto
            ConsoleKeyInfo cancellaConf;

            //cancello lo schermo
            Console.Clear();

            Console.WriteLine("ATTENZIONE! Se vuoi tornare al menù principale perderai il carrello...\n");
            Console.WriteLine("Se sei sicuro di cancellare il carrello premi Invio" +
    "               \nAltrimenti premi un tasto qualunque");


            //leggo il tasto scelto dall'utente
            cancellaConf = Console.ReadKey(true);

            //caso CONFERMA: il cliente ha dato il suo consenso all'operazione
            if (cancellaConf.Key == ConsoleKey.Enter)
            {
                //se l'utente conferma l'uscita cancello il carrello
                esito = CancellaCarrello(carrello,true);

            }
                return esito;

        }

        //
        static int Comprato(Carrello carrelloAcquisto)
        {
            //dichiaro una variabile contenente l'esito dell'operazione
            int esito = 0;

            //dichiaro una stringa per contenere tutti gli acquisti
            string stringAcquisto = "",
                lineaOrdine="";

            //controllo esistenza del file
            if (!(File.Exists(fileOrdini)))
            {
                Console.WriteLine("\n\nIl file non e' raggiungibile al momento" +
                                  "\n\nContattare l'amministratore del programma");
                esito = 0;
            }

            else
            {
                //qui verrà memorizzata la lista degli ordini già memorizzati sul file
                List<string> elencoOrdini = new List<string>();

                //recupero una lista con l'elenco degli ordini già scritti
                elencoOrdini = LeggiFile(fileOrdini, 0);

                //implodo in una stringa tutte le info riguardanti gli ordini
                foreach (Ordine ord in carrelloAcquisto.Ordini)
                {
                    stringAcquisto += ";";
                    stringAcquisto += ord.CodeProdotto;
                    stringAcquisto += ",";
                    stringAcquisto += ord.Quantità;
                    stringAcquisto += ",";
                    stringAcquisto += ord.DescrizioneProdotto;

                }

                //scrivo una riga di file all'interno di una stringa
                //con tutte le info attuali del carrello
                lineaOrdine=elencoOrdini.Count + ";" + 
                                         carrelloAcquisto.CodUtente + ";" 
                                         + carrelloAcquisto.DataAcquisto.ToString("d") + ";"+
                                         +carrelloAcquisto.CodTipologia + ";" +
                                         +carrelloAcquisto.TotImporto +
                                         stringAcquisto + "#";

                //scrivo l'ordine sul file e
                //comunico che la scrittura è andata a buon fine
                esito = ScriviSuFile(fileOrdini, lineaOrdine);
            }

            return esito;
        }


        //1° menu overloading: messaggio di benvenuto e array di stringhe
        static int Menu(string messaggio, string[] modalità)
        {
            //dichiaro una lista per fargli confluire l'array di stringhe in ingresso
            List<string> arrayInList = new List<string>(modalità);
            
            //indico una variabile per far ottenere il risultato della funzione menù
            int attuale = 0; 

            //invoco la funzione che mostra il menù di una lista di valori
            attuale = Menu(messaggio, arrayInList);

            //restituisco l'esito
            return attuale;
        }


        //2° menu overloading: lista di voci per elenchi dinamici
        static int Menu(string messaggio, List<string> voci)
        {
            //Imposta la scelta del menu' iniziale
            ConsoleKeyInfo scelta; //identifica la scelta del cliente
            int i, attuale = 0; //indica la schermata attualmente visualizzata



            //menu a selezione per tutte le modalità
            for (i = 0; i < voci.Count; i++)
            {
                if (i == 0)
                {
                    Console.Write(messaggio + "\n\n");
                    Console.Write("> ");
                    Console.Write(voci[i] + " <\n\n");
                }
                    
                else
                {
                    Console.Write("  ");
                    Console.Write(voci[i] + "\n\n");
                }

            }

            do
            {
                //leggo la scelta effettuata dal cliente
                scelta = Console.ReadKey(true);

                //cancella lo schermo
                Console.Clear();

                //scrivo il messaggio
                Console.Write(messaggio + "\n\n");

                //scelta sù
                if (scelta.Key == ConsoleKey.UpArrow)
                    attuale--;
                if (attuale < 0)
                    attuale = voci.Count - 1;

                //scelta giù
                if (scelta.Key == ConsoleKey.DownArrow)
                    attuale++;
                if (attuale > voci.Count - 1)
                    attuale = 0;

                //Qui mostro l'etichetta attualmente selezionata
                for (i = 0; i < voci.Count; i++)
                {
                    if (i == attuale)
                    {
                        Console.Write("> ");
                        Console.Write(voci[i] + " <\n\n");
                    }
                        
                    else
                    {
                        Console.Write("  ");
                        Console.Write(voci[i] + "\n\n");
                    }

                }
            }
            while (!(scelta.Key == ConsoleKey.Enter)); // esco quando è selezionata l'ultima scelta e premo Invio
            return attuale;
        }


        //questa funzione estrae da un file .csv la lista dei campi di una stessa colonna
        static List<string> LeggiFile(string File,int estratto)
            {

            List<string> LettiDB= new List<string>(); //creo la lista (mediante costruttore)
            int i = 0;

            string lineaLetta; //dichiaro la linea letta dal file
                
            StreamReader sr = new StreamReader(File); //flusso di lettura

            try
            { 
                while (!sr.EndOfStream)
                {
                    //leggo una linea dal file
                    lineaLetta = sr.ReadLine();

                    //effettuo la lettura soltanto se la linea non è vuota
                    if(lineaLetta!="")
                    {
                    //estraggo dalla linea il campo che mi interessa
                    LettiDB.Add(lineaLetta.Split(';')[estratto]);      //imposto il carattere separatore ";"
                    }
                    //incremento il contatore
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAttenzione: errore in lettura del file:" + File +
                    "\n Eccezione gestita" + e.Message);
            }
            finally
            {
            //chiudo il file
            if (sr!=null)
            sr.Close();
            }
            return LettiDB;
        }
    }
}