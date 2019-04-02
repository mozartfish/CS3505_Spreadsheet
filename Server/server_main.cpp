/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last revision: 3/28/19
 *
 * The entry point for the server spreadsheet application
 */

#include <iostream>
#include <vector>
#include <future>
#include <mutex>
#include <strings.h>
#include "../Connection/socket_connections.h"

int main(int argc, char ** argv)
{
  std::cout << "Launching server" << std::endl;
  
  // Initialize the struct of sockets
  socks connections;
  connections.sockets = new std::vector<int>();
  connections.size_before_update = 0;
  connections.new_socket_connected = false;
  connections.buffers = new std::vector<char*>();

  // Initialize a lock that gets passed around for when updating 
  // the socks struct or accessing its members
  std::mutex lock;

  //TODO Read in all information files for startup and have placed
  // in their proper objects

  //TODO listen for clients async
  auto connect = std::async(std::launch::async, socket_connections::WaitForClientConnections, &connections, &lock);
  
  //Infinite send and receive loop
   while(true)
    {

      // In the case there is at least 1 new client
      lock.lock();
       if (connections.new_socket_connected)
      	{

      	  std::cout << "A new client has connected" << std::endl;

	  // Iterate over each new client, and send required start data, and wait for data from them
	  for (int idx = connections.size_before_update; idx < connections.sockets->size(); idx++)
	    {
              connections.buffers->push_back(new char[BUF_SIZE]);
	      bzero((*connections.buffers)[idx - 1], BUF_SIZE);

	      socket_connections::SendData((*connections.sockets)[idx], "Hello\n", 6);

	      std::async(std::launch::async, socket_connections::WaitForData,
			 (*connections.sockets)[idx],  (*connections.buffers)[idx - 1], BUF_SIZE);
			 
	    }
	  connections.new_socket_connected = false;
	  connections.size_before_update = connections.sockets->size();
      	}
       lock.unlock();

       //TODO Send updates

       //TODO Process requests, wait for more data
    }
}
