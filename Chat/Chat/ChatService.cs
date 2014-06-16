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
      TcpChannel chan = new TcpChannel(props, null, provider); 
      ChannelServices.RegisterChannel(chan); 
      RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatServer), "Chat.ChatServer", WellKnownObjectMode.Singleton);
    }

    protected override void OnStop() {
    }
  }

  public class ChatClient : MarshalByRefObject { 
    public void Reci(string s) { 
      Console.WriteLine(s); 
    } 
  }

  public class ChatServer : MarshalByRefObject { 
    List<ChatClient> lc = new List<ChatClient>(); 
    
    public void DodajKlijenta(ChatClient c) { 
      lc.Add(c); 
    } 
    
    public void ReciSvima(ChatClient c, string s) { 
      for (int i = 0; i < lc.Count; ++i) { 
        try { 
          if (lc[i] != c) lc[i].Reci(s); 
        } 
        catch { 
          lc.RemoveAt(i); 
          --i; 
        } 
      } 
    } 
  }
}
