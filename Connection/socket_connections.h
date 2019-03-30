/*
 * Authors: Thomas Ady, Pranav Rajan
 * Last Revision: 3/27/19
 *
 * This header encompasses all declarations for socket based connection code
 * and static functions for connecting, sending, and receiving on TCP based
 * connections
 */

#include <sys/socket.h>

#ifndef CONNECT_H
#define CONNECT_H

// A struct containing a list of sockets and a size of the list
struct socks {
    int* sockets;
    int size;
  };

class socket_connections {

 private:
  socket_connections(){};

 public:
  // The port both clients and servers connect with
  const static int PORT_NUM = 2112;
  
  // Server functions to allow client connection
  static void WaitForClientConnections(socks * sock_list);
  static void ClientConnected();

  // Client functions to allow server connections
  static void AttemptServerConnection();
  static void OnServerConnect();
  
  // Functions for both Server and CLient to get and process data
  static void WaitForData();
  static void OnDataReceived();
  static void SendData();

};

#endif
