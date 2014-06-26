using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Chat {
  public partial class ChatService : ServiceBase {
    public ChatService() {//ostvarivanje windows servisa
      InitializeComponent();
    }

    protected override void OnStart(string[] args) {
      BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider(); 
      provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full; 
      IDictionary props = new Hashtable(); props["port"] = 12345; //server će slušati a portu 12345
      TcpChannel channel = new TcpChannel(props, null, provider); 
      ChannelServices.RegisterChannel(channel); //registriranje na kanal
      RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatServer), "Chat.ChatServer", WellKnownObjectMode.Singleton);//ime servera je "Chat.Chatserver"
    }

    protected override void OnStop() {
    }
  }

  public class ChatClient : MarshalByRefObject {
    public string username;

    public ChatClient(string username) {//referenca na klijenta
      this.username = username;
    }

    public void Send(string message, string senderUsername = null) {//klijent šalje poruku, server je ispisuje na ostale klijente
      if (senderUsername == null) Console.WriteLine(message);
      else Console.WriteLine(senderUsername + ": " + message); 
    } 
  }

  public class ChatServer : MarshalByRefObject { 
    public List<ChatClient> clientList = new List<ChatClient>();

    public bool UserExists(string username) {
      try {
        if (clientList.Exists(client => client.username == username)) return true;//provjeravanje postoji li klijent u grupi za chat, ako da, sve ok, ako ne, znači da je klijent napustio chat
        return false;
      }
      catch {
        return false;
      }
    }
    
    public void AddClient(ChatClient client) {
      clientList.Add(client); //dodavanje nekog klijenta u listu klijenata (za chat)
    }

    public void RemoveClientAt(int i) {
      clientList.RemoveAt(i);//brisanje klijenta iz liste klijenata
    }
    
    public void SendToAll(ChatClient client, string message) {
      for (int i = 0; i < clientList.Count; ++i) { 
        try {
          if (clientList[i] != client) clientList[i].Send(message, client.username);//slanje poruke klijentu j od klijenta i ako j!=i 
        } 
        catch {
          clientList.RemoveAt(i); //ovaj klijent je odstupio od chatanja 
          --i; 
        } 
      } 
    }

    public void SendNotification(string message, ChatClient client) {
      for (int i = 0; i < clientList.Count; ++i) {
        try {
          if (clientList[i] != client) clientList[i].Send(message);//slanje notifikacija klijentima -> da se neki drugi klijent pridružio ili izišao iz razgovora
        }
        catch {
          clientList.RemoveAt(i);
          --i;
        }
      }
    }
  }
}
