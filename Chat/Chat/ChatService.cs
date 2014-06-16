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
    public ChatService() {
      InitializeComponent();
    }

    protected override void OnStart(string[] args) {
      BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider(); 
      provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full; 
      IDictionary props = new Hashtable(); props["port"] = 12345; 
      TcpChannel channel = new TcpChannel(props, null, provider); 
      ChannelServices.RegisterChannel(channel); 
      RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatServer), "Chat.ChatServer", WellKnownObjectMode.Singleton);
    }

    protected override void OnStop() {
    }
  }

  public class ChatClient : MarshalByRefObject {
    public string username;

    public ChatClient(string username) {
      this.username = username;
    }

    public void Send(string senderUsername, string message) { 
      Console.WriteLine(senderUsername + ": " + message); 
    } 
  }

  public class ChatServer : MarshalByRefObject { 
    List<ChatClient> clientList = new List<ChatClient>();

    public bool UserExist(string username) {
      if (clientList.Find(client => client.username == username) != null) return true;
      
      return false;
    }
    
    public void AddClient(ChatClient client) {
      clientList.Add(client); 
    } 
    
    public void SendToAll(ChatClient client, string message) {
      for (int i = 0; i < clientList.Count; ++i) { 
        try {
          if (clientList[i] != client) clientList[i].Send(client.username, message); 
        } 
        catch {
          clientList.RemoveAt(i); 
          --i; 
        } 
      } 
    } 
  }
}
