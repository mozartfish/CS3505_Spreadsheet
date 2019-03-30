/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last revision: 3/28/19
 *
 * The entry point for the server spreadsheet application
 */

#include <iostream>
#include "../Connection/socket_connections.h"

int main(int argc, char ** argv)
{
  // Initialize the struct of sockets
  socks connections;
  connections.sockets = new int[200];
  connections.size = 0;
  connections.size_before_update = 0;
  connections.new_socket_connected = false;

  //TODO Read in all information files for startup and have placed
  // in their proper objects

  //TODO listen for clients async
  socket_connections::WaitForClientConnections(&connections);

  //Infinite send and receive loop
}
