/*
 * Authors: Thomas Ady, Pranav Rajan
 * Last revision: 4/15/19
 *
 * Contains all function definitions for networking code
 * to be used with server client connections
 */

#include <strings.h>
#include <vector>
#include <mutex>
#include <sys/types.h> 
#include <sys/socket.h>
#include <netinet/in.h>
#include <iostream>
#include <unistd.h>
#include <chrono>
#include "socket_connections.h"

  /*
   * Takes in an array pointer of socket IDs and waits for incoming connection 
   * to the server on default port 2112, where index 0 is assumed to be the
   * ID for the socket that waits on client connections. overwrites any
   * numbers currently in the list.
   */
void socket_connections::WaitForClientConnections(volatile socks * sock_list, std::mutex* lock)
  {

    // Make the socket listening for clients
    std::vector<int>* socket_list = sock_list->sockets;
    socket_list->push_back(socket(AF_INET, SOCK_STREAM, 0));
    
    if ((*socket_list)[0] < 0)
      {
	std::cout << "ERR: Client Connection socket failed" << std::endl;
	return;
      }

    // Initial increments for the waiting socket
    ++(sock_list->size_before_update);

    // Setting up the sockaddr from: http://www.linuxhowtos.org/C_C++/socket.htm
    struct sockaddr_in serv_addr;
    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(PORT_NUM);

    // Bind the socket to an address
    if (bind((*socket_list)[0], (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0)
      {
	std::cout << "Failed to bind socket" << std::endl;
	return;
	// TODO End program
      }

    // Listen for incoming connections
    if (listen((*socket_list)[0], 100) < 0)
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
      int fd = accept((*socket_list)[0], (struct sockaddr *) &cli_addr, &clilen);
      std::cout << "connected" << std::endl;

      lock->lock();
      socket_list->push_back(fd);

      // In case could not connect
      if ((*socket_list)[socket_list->size()] < 0)
	{
	  std::cout << "ERR: Failed connection to client, continuing waiting for connections" << std::endl;
	  continue;
	}

      std::cout << "new socket connected" << std::endl;

      // set new connection
      sock_list->new_socket_connected = true;
      lock->unlock();

      std::cout << "unlocked" << std::endl;
    }
  }
  

  
  /*
   * Reads in data from the socket fd provided, into the given buffer
   * of most bytes amount of data
   */
  void socket_connections::WaitForData(int socket_fd, char* buf, int bytes)
  {
    if (read(socket_fd, buf, bytes) < 0)
      std::cout << "Error reading data from socket" << std::endl;

    std::cout << buf << std::endl;
  }

/*
 * Counts the time passed when waiting for data to decide whether to
 * disconnect the client being waited on
 */
void socket_connections::WaitForDataTimer(char* buf, int vec_idx, std::mutex* lock, std::vector<bool> * disconnect_list)
{
  auto begin = std::chrono::steady_clock::now();
  while (std::chrono::duration_cast<std::chrono::minutes>(std::chrono::steady_clock::now() - begin).count() < 5);

  // If there is data after the time, return
  if(buf[0] || buf[0] < 0)
    return;

  // Set disconnect to true if no data has been found yet
  (*lock).lock();
  (*disconnect_list)[vec_idx] = true;
  (*lock).unlock();
}

  /*
   * Sends the provided data to the socket associated with the socket_fd parameter
   * where bytes is the amount of data from data to send
   */
void socket_connections::SendData(int socket_fd, const char *data, int bytes)
  {

    int bytes_written = write(socket_fd, data, bytes);
      
      // If not all data was sent, try to send the rest
      if (bytes_written < bytes)
	{
	  data += bytes_written;
	  SendData(socket_fd, data, bytes - bytes_written);
	}
  }

/*
 * Safely closes a socket of the specified file descriptor
 */
void socket_connections::CloseSocket(int socket_fd)
{
  if (close(socket_fd) < 0)
    std::cout << "Error closing socket of file descriptor " << socket_fd << std::endl;
}
