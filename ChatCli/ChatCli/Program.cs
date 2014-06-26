using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Chat;

namespace ChatCli {
  class Program {
    static void Main(string[] args) {
      TcpChannel channel = new TcpChannel(0);
      ChannelServices.RegisterChannel(channel);

      ChatServer server = (ChatServer)Activator.GetObject(typeof(ChatServer), @"tcp://localhost:12345/Chat.ChatServer");//Server slusa klijenta na portu 12345, ime serverat: Chat.Chatserver
      if (server == null) {//server ne postoji - nullreferenciran objekt
        System.Console.WriteLine("Nema servera");
        return;
      }

      Console.WriteLine("Dobrodošli na chat! :)");//pozdravna poruka klijentu kad se istancira
      string clients = "";
      for (int i = 0; i < server.clientList.Count; ++i) {//ovdje hvatamo popis klijenata u razgovoru...
        try {
          clients += server.clientList[i].username + ", ";
        }
        catch {
          server.RemoveClientAt(i);
          --i;
        }
      }
      Console.WriteLine("Trenutno u razgovoru sudjeluje " + server.clientList.Count() + " korisnika.");
      if (clients != "") Console.WriteLine("Trenutni korisnici: " + clients.Substring(0, clients.Length - 2));//popis klijenata koji sudjeluju u gurpi
      Console.WriteLine();

      Console.Write("Korisničko ime: ");//Klijent mora unijeti svoje korisničko ime -> ako ne unese ništa ili unese već postojeće korisničko ime, traži ponovni unos
      string username = Console.ReadLine();
      while (username == "" || server.UserExists(username)) {
        if (username == "") Console.Write("Morate unijeti korisničko ime: ");
        else Console.Write("Korisnik " + username + " već postoji, izaberite neko drugo korisničko ime: ");
        username = Console.ReadLine();
      }
      ChatClient me = new ChatClient(username);//kreiranje novog klijenta
      server.SendNotification("Korisnik " + username + " se pridružio razgovoru.", me);//Serveru šaljemo notifikaciju poruku da se pridružio razgovoru novi klijent - > o tome će obavijestiti sve ostale klijente
      server.AddClient(me);//dodajemo novog klijenta serveru
      Console.WriteLine();

      while (true) {
        string message = Console.ReadLine();//učitavanje nove poruke
        if (message == "") {//ništa nismo učitali -> napuštanje razgovora
          server.SendNotification("Korisnik " + username + " je napustio razgovor.", me);//serveru šaljemo notifikaciju o tome da smo napustili razgovor -> server obaviještava klijente
          break;//izalzimo iz razgovora
        }
        server.SendToAll(me, message);//server svima šalje poruku od mene 
      }
    }
  }
}