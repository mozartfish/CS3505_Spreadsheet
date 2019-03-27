/*
 * Authors: Thomas Ady, Pranav Rajan
 * Last revision: 3/27/19
 *
 * Contains all function definitions for networking code
 * to be used with server client connections
 */

#include <sys/socket.h>
#include "socket_connections.h"

  /*
   * Initiates a socket that waits for incoming connections to the server,
   * on default port 2112
   */
  void socket_connections::WaitForClientConnections()
  {
  }

  /*
   * The response function for when a client connects to the server,
   * intitiates the waiting loop again once connection is finalized
   */
  void socket_connections::ClientConnected()
  {
  }
  
  /*
   *
   */
  void socket_connections::AttemptServerConnection()
  {
  }

  /*
   *
   */
  void socket_connections::OnServerConnect()
  {
  }
  
  /*
   *
   */
  void socket_connections::WaitForData()
  {
  }

  /*
   *
   */
  void socket_connections::OnDataReceived()
  {
  }

  /*
   *
   */
  void socket_connections::SendData()
  {
  }
