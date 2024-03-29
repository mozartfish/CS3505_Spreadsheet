/*
 * Authors: Thomas Ady, Pranav Rajan
 * Last Revision: 3/30/19
 *
 * This header encompasses all declarations for socket based connection code
 * and static functions for connecting, sending, and receiving on TCP based
 * connections
 */

#include <sys/socket.h>
#include <vector>
#include <mutex>
#include <string>
#include <chrono>
#include <unordered_map>

#ifndef CONNECT_H
#define CONNECT_H

#define BUF_SIZE 1024 //Buffer size default of 1kB

// A struct containing a list of sockets and a size of the list
struct socks {
  std::vector<int>* sockets;

  bool continue_listening;
  
  // Fields to let any user of this struct know it has been modified
  int size_before_update;
  bool new_socket_connected;

  // Buffer that gets data and buffer that holds data
  std::vector<char*>* buffers;
  std::vector<std::string *>* partial_data;

  std::unordered_map<int, bool>* needs_removed;
  };

class socket_connections {

 private:
  socket_connections(){};

 public:
  // The port both clients and servers connect with
  const static int PORT_NUM = 2112;
  
  // Server functions to allow client connection
  static void WaitForClientConnections(volatile socks * sock_list, std::mutex* lock, bool * continue_to_run);
  
  // Functions to get and process data
  static void WaitForData(int socket_fd, char* buf, int bytes, std::mutex* lock, std::unordered_map<int, bool> * should_disc);
  static void SendData(int socket_fd, const char * data, int bytes);

  // Simple wrapper for closing socket
  static void CloseSocket(int fd);

  // Helper function for waiting on data
  static void WaitForDataTimer(char* buf, std::mutex* lock, int socket_fd, std::unordered_map<int, bool> * should_disc, bool * has_mod_val);

};

#endif
