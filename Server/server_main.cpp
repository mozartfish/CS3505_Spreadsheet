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
#include <cstring>
#include <strings.h>
#include <fstream>
#include "spreadsheet.h"
#include "message.h"
#include "helpers.h"
#include "../Connection/socket_connections.h"

using namespace std;

unordered_map<string, spreadsheet*> * sheets;
vector<string> * sheet_names;
unordered_map<int, string> * socket_usermap;
unordered_map<int, string> * socket_sprdmap;
queue<string> * updates;
const string * SHEET_FILEPATH = new string ("./Settings/sheets.txt");

/*
 * Closes the server, sending all necessary goodbyes, processes the rest of
 * the incoming requests, closes all socket connections, deletes all
 * pointer objects, and writes all spreadsheets back to the file in Settings
 */
void close(volatile socks & socket_info)
{

  //Clear queue and process messages
  

  //Close sockets
  for (int socket : *socket_info.sockets)
    socket_connections::CloseSocket(socket);

  //Delete all pointers, and all spreadsheets in the list
  for (unordered_map<string, spreadsheet*>::iterator it = sheets->begin(); it != sheets->end(); it++)
    delete it->second;
  delete sheets;
  delete sheet_names;
  delete socket_usermap;
  delete socket_sprdmap;
  delete updates;
  delete SHEET_FILEPATH;
}

/*
 * Processes all spreadsheets from the Settings directory that should be
 * adjacent to this file, for use on server startup, assumes histories are well formed
 * Returns a non-zero number on an error
 */
int process_spreadsheets_from_file()
{
  ifstream file((*SHEET_FILEPATH));
  string line;

  vector<string> read_partitions;
  read_partitions.push_back("Name:");
  read_partitions.push_back("Usermap:");
  read_partitions.push_back("Spreadsheet_History:");
  read_partitions.push_back("Cell_Information:");

  // Read in spreadsheets
  if (file.is_open())
      while (getline(file, line))
	{
	  spreadsheet* curr_sheet = NULL;

	  // Pull out the first token
	  char * line_pointer = &line[0];
	  char * token = strtok(line_pointer, "\t");

	  // If no data after line
	  if (!token)
	    continue;

	  string * token_str = new string(token);

	  int part_idx = 0;

       /**********************************************************************/
       /*                       READ SHEET NAME                              */
       /**********************************************************************/

	  // Bad name error
	  if (*token_str != read_partitions[part_idx])
	    return -1;
	  

	  // Try to get name, make spreadsheet if valid, otherwise return err
	  token = strtok(NULL, "\t");
	  if (token == NULL)
	    return -1;
	  else
	    {
	      delete token_str;
	      token_str = new string (token);
	      curr_sheet = new spreadsheet(*token_str);
	    }
	  
	  ++part_idx;
	  token = strtok(NULL, "\t");

       /**********************************************************************/
       /*                       USERMAP READ BLOCK                           */
       /**********************************************************************/


	  // Try to parse Usermap (Can be empty)
	  if (token == NULL)
	    return -1;
	  else
	    {
	      token_str = new string (token);

	      // Bad partition
	      if (*token_str != read_partitions[part_idx])
		return -1;

	      part_idx++;

	      // Iterate over usermap until no tokens (error) or until next partition
	      while (token != NULL )
		{
		  // Get user
		  token = strtok(NULL, "\t");
		  if (token == NULL)
		    return -1;

		  string user(token);

		  // If 'user' is actually the next partition in the spreadsheet, break loop
		  if (user == read_partitions[part_idx])
		      break;

		  // Get pass
		  token = strtok(NULL, "\t");
		  if (token == NULL)
		    return -1;

		  string pass(token);

		  //Add user password pair to sheet
		  curr_sheet->add_user(user, pass);
		}
	    }

       /**********************************************************************/
       /*                       SHEET HISTORY READ                           */
       /**********************************************************************/

	  // Never reached next partition (aka Spreadsheet history)
	  if(token == NULL)
	    return -1;

	  part_idx++;

	  vector<string> * spd_hist = new vector<string>();

	  // Iterate over spreadsheet history
	  token = strtok(NULL, "\t");
	  while (token != NULL)
	    {
	      
	      token_str = new string (token);

	      // Make sure we have not reached next partition
	      if (*token_str == read_partitions[part_idx])
		break;

	      // add directly to spread history
	      spd_hist->push_back(*token_str);

	      token = strtok(NULL, "\t");
	    }

	  curr_sheet->add_direct_sheet_history(*spd_hist);

       /**********************************************************************/
       /*                       CELL HISTORY READ                            */
       /**********************************************************************/
	  
	  // Never reached next partition (aka Cell history)
	  if (token == NULL)
	    return -1;

	  token = strtok(NULL, "\t");

	  // Iterate over each token
	  while (token)
	    {
	      token_str = new string (token);
	     
	      //Parse cell num
	      int cell_num = stoi(token_str->substr(1, token_str->length() - 1));
	      vector<string> * cell_hist = new vector<string>();

	      token = strtok(NULL, "\t");

	      // Iterate over full history of the cell
	      while (token)
		{
		  token_str = new string(token);

		  // If found next cell
		  if ((*token_str)[0] == ':' && (*token_str)[token_str->length() - 1] == ':')
		    break;

		  // Add histories to a vector
		  cell_hist->push_back(*token_str);

		  token = strtok(NULL, "\t");
		}
	      
	      // Add vector of histories to spreadsheet
	      curr_sheet->add_direct_cell_history(cell_num, *cell_hist);
	      
	      } 

	  // Add finished sheet 
	  (*sheets)[curr_sheet->get_name()] = curr_sheet;
	}

  std::cout << "Finishing file read" << std::endl;
  
  // If could not open assumes there was no sheet to read
  file.close();
  return 0;
}

/*
 * Writes the state of all of the current spreadsheets to the specified SHEET_FILEPATH under a .txt file
 */
int write_sheets_to_file()
{
  ofstream file;
  file.open((*SHEET_FILEPATH), ofstream::out | ofstream::trunc);

  // Write all values, separated with tab characters (similar to tsv)
  if (file.is_open())
    {

      cout << "Writing spreadsheets back to file" << endl;

      unordered_map<string, spreadsheet*>::iterator sheet_it = sheets->begin();

      for (sheet_it; sheet_it != sheets->end(); sheet_it++)
	{
	  //Write name
	  file << "Name:" << '\t';
	  file << sheet_it->first << '\t';
	  
	  //Write users and passwords
	  file << "Usermap:" << '\t';
	  for (auto usermap : sheet_it->second->get_users())
	    file << usermap.first << '\t' << usermap.second << '\t';
	  
	  file << "Spreadsheet_History:" << '\t';
	  //Write spreadsheet history
	  for (auto sheet_hist : sheet_it->second->get_sheet_history())
	    {
	      file << sheet_hist << '\t';
	    }
	  
	  
	  //Write cells, and their dependencies, and their history
	  file << "Cell_Information:" << '\t';
	  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
	    {
	      // Write individual cell history
	      string cell = ":" + to_string(i) + ":";
	      file << cell;
	      vector<string> cell_hist = sheet_it->second->get_cell_history(i);
	      for (auto cell_contents : cell_hist)
		file  << '\t' << cell_contents;
	    }

	  //Separate spreadsheets by newline
	  file << '\n';
	}
    }
  //error opening file
  else
    return -1;

  file.close();
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
  //Create new spreadsheet if nonexistant
  if (sheets->find(spread_name) == sheets->end())
    {
      sheet_names->push_back(spread_name);
      (*sheets)[spread_name] = new spreadsheet(spread_name);
      (*sheets)[spread_name]->add_user(user, pass);
      return true;
    }
  
  // Access spreadsheet
  else
    {
      unordered_map<string,string> sprd_users = (*sheets)[spread_name]->get_users();
      
      // User didn't exist
      if (sprd_users.find(user) == sprd_users.end())
	{
	  (*sheets)[spread_name]->add_user(user, pass);
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
  volatile  socks connections;
  connections.sockets = new std::vector<int>();
  connections.size_before_update = 0;
  connections.new_socket_connected = false;
  connections.buffers = new std::vector<char*>();
  connections.partial_data = new vector<string*>();

  // Initialize a lock that gets passed around for when updating 
  // the socks struct or accessing its members
  std::mutex lock;

  // Initialize all fields needed for running the server
  sheets = new unordered_map<string, spreadsheet*> ();
  sheet_names = new vector<string>();
  socket_usermap = new unordered_map<int, string>();
  socket_sprdmap = new unordered_map<int, string>();
  updates = new queue<string>();

  //Start by reading the file of spreadsheets into the program
  if (process_spreadsheets_from_file() < 0)
    {
      cout << "Error reading spreadsheet file. Check for syntax errors." << endl;
      return -1;
    }

  //Listen for clients async
  std::async(std::launch::async, socket_connections::WaitForClientConnections, &connections, &lock);
  
  //Infinite send and receive loop
   while(true)
    {

      /**********************************************************************/
      /*                         NEW CLIENT BLOCK                           */
      /**********************************************************************/
      lock.lock();
       if (connections.new_socket_connected)
      	{

      	  std::cout << "A new client has connected" << std::endl;
	  
	  // Make message of all spreadsheet names
	  message name_mess("list");
	  name_mess.spreadsheets = sheet_names;
	  string json_message = server_helpers::message_to_json(name_mess);
	  char * json_primitive = strcpy(new char[json_message.size() + 1], json_message.c_str());

	  // Iterate over each new client, and send required start data, and wait for data from them
	  for (int idx = connections.size_before_update; idx < connections.sockets->size(); idx++)
	    {
	      // Add new buffers for getting data
              connections.buffers->push_back(new char[BUF_SIZE]);
	      bzero((*connections.buffers)[idx - 1], BUF_SIZE);
	      connections.partial_data->push_back(new string());

	      
	      // Send list of spreadsheet names
	      socket_connections::SendData((*connections.sockets)[idx], json_primitive, (int)json_message.size());

	      std::async(std::launch::async, socket_connections::WaitForData,
			 (*connections.sockets)[idx],  (*connections.buffers)[idx - 1], BUF_SIZE);
			 
	    }
	  connections.new_socket_connected = false;
	  connections.size_before_update = connections.sockets->size();
      	}
       lock.unlock();


       /**********************************************************************/
       /*                       DATA RECEIVE BLOCK                           */
       /**********************************************************************/
       lock.lock();
       
       // Get data from buffers
       for (int idx = 0; idx < connections.buffers->size(); idx++)
	 {
	   // If data exists, grab and reset buffer (If not chars are null)
	   if ((*connections.buffers)[idx][0] > 0)
	     {
	       (*(*connections.partial_data)[idx]) += (*connections.buffers)[idx];
	       (*connections.buffers)[idx] = new char[BUF_SIZE];
	       bzero((*connections.buffers)[idx], BUF_SIZE);
	     }

	   // Resume getting data
	   std::async(std::launch::async, socket_connections::WaitForData,
			 (*connections.sockets)[idx + 1],  (*connections.buffers)[idx], BUF_SIZE);

	   // Process data
	   string delimiter = "\n\n";
	   int limit = (*connections.partial_data)[idx]->find(delimiter);

	   // If delemiter doesn't exist, data is partial
	   if (limit < 0)
	     continue;

	   //Get new data, erase data
	   string  data = (*connections.partial_data)[idx]->substr(0, limit);
	   (*connections.partial_data)[idx]->erase(0, limit + 1);

	   // Format an update as socket_num, sheet_name, and then the data, separated with \t
	   string * update = new string();
	   *update += to_string((*connections.sockets)[idx + 1]) + '\t';
	   *update += (*socket_sprdmap)[(*connections.sockets)[idx + 1]] + '\t';
	   *update += data;
	   
	   // Add data to the queue
	   updates->push(*update);
	   
	 }

       lock.unlock();


       /**********************************************************************/
       /*                         UPDATE PROCESSING                          */
       /**********************************************************************/

       lock.lock();

       while (updates->size() != 0)
	 {
	   // Get individual message
	   string update = updates->front();
	   updates->pop();

	   // get tokens, convert from JSON
	   

	   // Send updates to all that should be notified if successful
	 }

       lock.unlock();
    }

   close(connections);
   return 0;
}
