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
#include <string>
#include <unordered_map>
#include <queue>
#include <strings.h>
#include "spreadsheet.h"
#include "../Connection/socket_connections.h"

using namespace std;

unordered_map<string, spreadsheet*> * sheets;
unordered_map<int, string> * socket_usermap;
unordered_map<int, string> * socket_sprdmap;
queue<string> * updates;

/*
 * Closes the server, sending all necessary goodbyes, processes the rest of
 * the incoming requests, closes all socket connections, deletes all
 * pointer objects, and writes all spreadsheets back to the file in Settings
 */
void close()
{

}

/*
 * Processes all spreadsheets from the Settings directory that should be
 * adjacent to this file, for use on server startup
 * Returns a non-zero number on an error
 */
int process_spreadsheets_from_file()
{
  return 0;
}

/*
 * Opens/creates a spreadsheet with the given spreadsheet name
 * username and password, returns true if the spreadsheet did not
 * exist or if the user password combo was correct, or if there
 * was no user of the provided name associated with the spreadsheet
 */
bool check_sprd(string spread_name, string user, string pass)
{
  //Create new spreadsheet
  if (sheets->find(spread_name) == sheets->end())
    {
      sheets[spread_name] = new spreadsheet(spread_name);
      sheets[spread_name]->add_user(user, pass);
      return true;
    }
  // Access spreadsheet
  else
    {
      unordered_map<string,string> sprd_users = get_users();
      
      
      if (sprd_users.find(user) == sprd_users.end())
	{
	  sheets[spread_name]->add_user(user, pass);
	  return true;
	}
      else
	return (sprd_users[user] == pass);
    }
}

/*
 * The main entry point for the program, launches the Server
 */
int main(int argc, char ** argv)
{
  std::cout << "Launching server" << std::endl;
  
  // Initialize the struct of sockets
  volatile socks connections;
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

   close();
   return 0;
}
