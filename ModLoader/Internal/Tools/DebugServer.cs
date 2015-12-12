using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

#if DEV_BUILD
namespace spaar.ModLoader.Internal.Tools
{
  public class DebugServer : SingleInstance<DebugServer>
  {
    public override string Name { get; } = "Debug Server";

    private TcpListener listener;
    private bool started = false;

    private Queue<string> receivedMessages;
    private List<TcpClient> clients;

    private void Start()
    {
      ModLoader.MakeModule(this);
      receivedMessages = new Queue<string>();
      clients = new List<TcpClient>();
    }

    public void StartDebugServer(int port)
    {
      var address = IPAddress.Any;
      listener = new TcpListener(address, port);
      listener.Start();
      started = true;
    }

    public void SendLogEntry(string message)
    {
      if (clients == null) return;
      foreach (var client in new List<TcpClient>(clients))
      {
        if (!client.Connected)
        {
          clients.Remove(client);
          continue;
        }

        var buffer = Encoding.UTF8.GetBytes(message);
        var stream = client.GetStream();
        stream.BeginWrite(buffer, 0, buffer.Length, WriteCallback,
          new AsyncState
          {
            stream = stream,
            buffer = buffer,
          });
      }
    }

    private void Update()
    {
      if (started)
      {
        if (listener.Pending())
        {
          StartCoroutine(AcceptClient());
        }

        if (receivedMessages.Count > 0)
        {
          Commands.HandleCommand(receivedMessages.Dequeue().Trim());
        }
      }
    }

    private System.Collections.IEnumerator AcceptClient()
    {
      var client = listener.AcceptTcpClient();

      while (!client.Connected)
      {
        yield return new WaitForFixedUpdate();
      }

      clients.Add(client);

      var stream = client.GetStream();

      var buffer = new byte[1024];
      stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, new AsyncState
      {
        stream = stream,
        buffer = buffer
      });
    }

    private void ReadCallback(IAsyncResult result)
    {
      var state = (AsyncState)result.AsyncState;
      int bytesRead = state.stream.EndRead(result);
      receivedMessages.Enqueue(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

      state.stream.BeginRead(state.buffer, 0, state.buffer.Length, ReadCallback,
        state);
    }

    private void WriteCallback(IAsyncResult result)
    {
      var state = (AsyncState)result.AsyncState;
      state.stream.EndWrite(result);
    }

    private struct AsyncState
    {
      public NetworkStream stream;
      public byte[] buffer;
    }
  }
}
#endif
