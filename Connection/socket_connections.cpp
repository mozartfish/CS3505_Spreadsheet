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

    // Initial increments for the waiting socket
    ++(sock_list->size);
    ++(sock_list->size_before_update);

    // Setting up the sockaddr from: http://www.linuxhowtos.org/C_C++/socket.htm
    struct sockaddr_in serv_addr;
    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(PORT_NUM);

    // Bind the socket to an address
    if (bind(socket_list[0], (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0)
      {
	std::cout << "Failed to bind socket" << std::endl;
	return;
      }

    // Listen for incoming connections
    if (listen(socket_list[0], 5000) < 0)
      {
	std::cout << "ERR: Could not listen for incoming connections" << std::endl;
	return;
      }

    
    // Infinitely listen for incoming connections
    while(true)
    {
      
      // Setting up the sockaddr and socklen from http://www.linuxhowtos.org/data/6/server.c
      struct sockaddr_in cli_addr;
      socklen_t clilen = sizeof(cli_addr);
      socket_list[sock_list->size] = accept(socket_list[0], 
					    (struct sockaddr *) &cli_addr, &clilen);
      // In case could not connect
      if (socket_list[sock_list->size] < 0)
	{
	  std::cout << "ERR: Failed connection to client, continuing waiting for connections" << std::endl;
	  continue;
	}

      //Should lock here
      // Increment size for new socket, set new connection
      ++(sock_list->size);
      sock_list->new_socket_connected = true;
    }
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
   * Sends the provided data to the socket associated with the socket_fd parameter
   */
  void socket_connections::SendData(int socket_fd)
  {
  }
