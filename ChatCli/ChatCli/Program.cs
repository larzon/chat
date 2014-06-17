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

      ChatServer server = (ChatServer)Activator.GetObject(typeof(ChatServer), @"tcp://localhost:12345/Chat.ChatServer");
      if (server == null) {
        System.Console.WriteLine("Nema servera");
        return;
      }

      Console.WriteLine("Dobrodošli na chat! :)");
      string clients = "";
      for (int i = 0; i < server.clientList.Count; ++i) {
        try {
          clients += server.clientList[i].username + ", ";
        }
        catch {
          server.RemoveClientAt(i);
          --i;
        }
      }
      Console.WriteLine("Trenutno u razgovoru sudjeluje " + server.clientList.Count() + " korisnika.");
      if (clients != "") Console.WriteLine("Trenutni korisnici: " + clients.Substring(0, clients.Length - 2));
      Console.WriteLine();

      Console.Write("Korisničko ime: ");
      string username = Console.ReadLine();
      while (username == "" || server.UserExists(username)) {
        if (username == "") Console.Write("Morate unijeti korisničko ime: ");
        else Console.Write("Korisnik " + username + " već postoji, izaberite neko drugo korisničko ime: ");
        username = Console.ReadLine();
      }
      ChatClient me = new ChatClient(username);
      server.SendNotification("Korisnik " + username + " se pridružio razgovoru.", me);
      server.AddClient(me);
      Console.WriteLine();

      while (true) {
        string message = Console.ReadLine();
        if (message == "") {
          server.SendNotification("Korisnik " + username + " je napustio razgovor.", me);
          break;
        }
        server.SendToAll(me, message);
      }
    }
  }
}