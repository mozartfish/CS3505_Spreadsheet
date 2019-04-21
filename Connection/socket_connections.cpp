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
#include <future>
#include <unordered_map>
#include "socket_connections.h"

  /*
   * Takes in an array pointer of socket IDs and waits for incoming connection 
   * to the server on default port 2112, where index 0 is assumed to be the
   * ID for the socket that waits on client connections. overwrites any
   * numbers currently in the list.
   */
void socket_connections::WaitForClientConnections(volatile socks * sock_list, std::mutex* lock, bool * continue_to_run)
  {

    // Make the socket listening for clients
    std::vector<int>* socket_list = sock_list->sockets;
    socket_list->push_back(socket(AF_INET, SOCK_STREAM, 0));
    
    if ((*socket_list)[0] < 0)
      {
	*continue_to_run = false;
	std::cout << "ERR: Client Connection socket failed" << std::endl;
	return;
      }

    int i = 1;
    // From https://stackoverflow.com/questions/24194961/how-do-i-use-setsockoptso-reuseaddr
    if (setsockopt((*socket_list)[0], SOL_SOCKET, SO_REUSEADDR, &i, sizeof(int)) < 0)
      {
	*continue_to_run = false;
	std::cout << "ERR: could not set socket options" << std::endl;
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
	*continue_to_run = false;
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
    while(*continue_to_run)
    {
      
      // Setting up the sockaddr and socklen from http://www.linuxhowtos.org/data/6/server.c
      struct sockaddr_in cli_addr;
      socklen_t clilen = sizeof(cli_addr);
      int fd = accept((*socket_list)[0], (struct sockaddr *) &cli_addr, &clilen);

      lock->lock();

      // In case could not connect
      if (fd < 0)
	{
	  std::cout << "ERR: Failed connection to client, continuing waiting for connections" << std::endl;
	  continue;
	}

      socket_list->push_back(fd);

      // set new connection
      sock_list->new_socket_connected = true;
      lock->unlock();
    }
  }
  

  
  /*
   * Reads in data from the socket fd provided, into the given buffer
   * of most bytes amount of data
   */
void socket_connections::WaitForData(int socket_fd, char* buf, int bytes, std::mutex* lock, std::unordered_map<int, bool> * should_disc)
  {
    bool has_mod_val = false;

    // WE want this to block when the destructor happens
    //  auto launch = std::async(std::launch::async, socket_connections::WaitForDataTimer, &buf[0], 
    //			     &(*lock), socket_fd, should_disc, &has_mod_val);

    if (read(socket_fd, buf, bytes) < 0)
      {
	std::cout << "Error reading data from socket" << std::endl;
        
        if (should_disc->find(socket_fd) != should_disc->end() && !has_mod_val) 
	  (*should_disc)[socket_fd] = true;
	has_mod_val = true;

      }

    std::cout << buf << std::endl;
  
  }

/*
 * Counts the time passed when waiting for data to decide whether to
 * disconnect the client being waited on
 */
void socket_connections::WaitForDataTimer(char* buf, std::mutex* lock, int socket_fd, std::unordered_map<int, bool> * should_disc,  bool * has_mod_val)
{

  auto begin = std::chrono::steady_clock::now();
  while (std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - begin).count() < 10);

  std::cout << "disconnect time" << std::endl;

  // If there is data after the time, return
  if(buf[0] || buf[0] < 0)
    return;

  // Set disconnect to true if no data has been found yet
  std::cout << "good disc" << std::endl;
  if (should_disc->find(socket_fd) != should_disc->end() && !(*has_mod_val)) 
    (*should_disc)[socket_fd] = true;
  (*has_mod_val) = true;
}

  /*
   * Sends the provided data to the socket associated with the socket_fd parameter
   * where bytes is the amount of data from data to send
   */
void socket_connections::SendData(int socket_fd, const char *data, int bytes)
  {

    int bytes_written = write(socket_fd, data, bytes);

    // On error
    if (bytes_written < 0)
      {
	std::cout << "Error sending data to socket " << socket_fd << std::endl;
	return;
      }
      
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
