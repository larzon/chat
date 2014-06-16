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
      ChatClient ja = new ChatClient(); 
      TcpChannel chan = new TcpChannel(0); 
      ChannelServices.RegisterChannel(chan); 
      ChatServer server = (ChatServer)Activator.GetObject(typeof(ChatServer), @"tcp://localhost:12345/Chat.ChatServer"); 
      if (server == null) { 
        System.Console.WriteLine("Nema servera"); 
        return; 
      } 
      server.DodajKlijenta(ja); 
      while (true) { 
        string s = Console.ReadLine(); 
        if (s == "") break; 
        server.ReciSvima(ja, s); 
      }
    }
  }
}
