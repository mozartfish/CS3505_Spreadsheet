/*
 * Authors: Thomas Ady, Pranav Rajan
 * Last revision: 3/27/19
 *
 * Contains all function definitions for networking code
 * to be used with server client connections
 */

#include <strings.h>
#include <sys/types.h> 
#include <sys/socket.h>
#include <netinet/in.h>
#include "socket_connections.h"
#include <iostream>

  /*
   * Takes in an array pointer of socket IDs and waits for incoming connections 
   * to the server on default port 2112, where index 0 is assumed to be the
   * ID for the socket that waits on client connections. overwrites any
   * numbers currently in the list.
   */
  void socket_connections::WaitForClientConnections(socks * sock_list)
  {
    // Make the socket listening for clients
    int * socket_list = sock_list->sockets;
    socket_list[0] = socket(AF_INET, SOCK_STREAM, 0);
    if (socket_list < 0)
      {
	std::cout << "ERR: Client Connection socket failed" << std::endl;
	return;
      }

    // Setting up the sockaddr from: http://www.linuxhowtos.org/C_C++/socket.htm
    struct sockaddr_in serv_addr;
    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(PORT_NUM);

    // Bind the socket to an address
    if (!bind(socket_list[0], (struct sockaddr *) &serv_addr, sizeof(serv_addr)))
      {
	std::cout << "Failed to bind socket" << std::endl;
	return;
      }

    // Listen for incoming connections
    listen(socket_list[0], 5000);

    //Client found
    ClientConnected();
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
