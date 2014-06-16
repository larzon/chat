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

      Console.Write("Korisničko ime: ");
      string username = Console.ReadLine();
      while (server.UserExist(username)) {
        Console.Write("Korisnik " + username + " već postoji, izaberite neko drugo korisničko ime: ");
        username = Console.ReadLine();
      }
      ChatClient me = new ChatClient(username);
      server.AddClient(me);

      while (true) {
        string message = Console.ReadLine();
        if (message == "") break;
        server.SendToAll(me, message);
      }
    }
  }
}