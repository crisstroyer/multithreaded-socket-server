# multithreaded-server-socket
[![MIT Licensed][license-image]][license-url]

Complete, compatible and well tested module to implement a low consumption ([GSM](https://es.wikipedia.org/wiki/Sistema_global_para_las_comunicaciones_m%C3%B3viles)) [multi-threaded](https://totalview.io/blog/multithreading-multithreaded-applications) [TCPIp socket](https://es.wikipedia.org/wiki/Socket_de_Internet) server for binary communication between a server and a [PLC](https://en.wikipedia.org/wiki/Programmable_logic_controller) device in a [SCADA](https://es.wikipedia.org/wiki/SCADA) system.

This module allows binary communication with telemetry systems

## Features

- Support for `BCD` data types
- Implementation of different data types such as `*hexadecimal*`, `*decimal*`, `*byte*`, `*float*`
- Low power Plot interpreter for `*gsm*` networks
- Dynamic definition of management frames for binary communication
- Low-level Socket `TCPIp server`
- `Multi-threaded` server as a service
- Server `multi-port` tcp ip
- Support for [AES](https://es.wikipedia.org/wiki/Advanced_Encryption_Standard) encryption
- `C#` implementation

## Some benefits

1. *Quick and Efficient*: Multithreaded server could respond efficiently and quickly to the increasing client queries quickly.
2. *Waiting time for users decreases*: In a single-threaded server, other users had to wait until the running process gets completed but in multithreaded servers, 
    all users can get a response at a single time so no user has to wait for other processes to finish.
3. *Threads are independent of each other*: There is no relation between any two threads. When a client is connected a new thread is generated every time.
4. *The issue in one thread does not affect other threads*: If any error occurs in any of the threads then no other thread is disturbed, all other processes keep running normally. 
    In a single-threaded server, every other client had to wait if any problem occurs in the thread.


[license-image]: https://img.shields.io/badge/license-MIT-blue.svg
[license-url]: https://raw.githubusercontent.com/crisstroyer/node-oauth-jwt-server/master/LICENSE