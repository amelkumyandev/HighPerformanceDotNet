using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HighPerformanceServer
{
    class Program
    {
        private const int MaxConnections = 10000;
        private const int BufferSize = 1024;
        private static Socket listenSocket;
        private static int totalClients = 0;

        // Resource pools
        private static BufferManager bufferManager = new BufferManager(BufferSize * MaxConnections, BufferSize);
        private static SocketAsyncEventArgsPool readWritePool = new SocketAsyncEventArgsPool(MaxConnections);
        private static ConcurrentDictionary<Socket, AsyncUserToken> connectedClients = new ConcurrentDictionary<Socket, AsyncUserToken>();

        static void Main(string[] args)
        {
            bufferManager.InitBuffer();

            // Pre-allocate SocketAsyncEventArgs objects for pooling
            for (int i = 0; i < MaxConnections; i++)
            {
                var eventArg = new SocketAsyncEventArgs();
                eventArg.Completed += IO_Completed;
                bufferManager.SetBuffer(eventArg);
                eventArg.UserToken = new AsyncUserToken();
                readWritePool.Push(eventArg);
            }

            StartServer();
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        private static void StartServer()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, 11000));
            listenSocket.Listen(100);
            Console.WriteLine("Server started. Listening on port 11000.");

            // Start accepting connections
            StartAccept(null);
        }

        private static void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            // Check if the argument is already in use; if so, create a new instance
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += AcceptEventArg_Completed;
            }
            else if (acceptEventArg.AcceptSocket != null)
            {
                // If AcceptSocket is still set, it means the operation hasn't completed yet
                acceptEventArg.AcceptSocket = null;
            }

            try
            {
                bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    // If the operation completed synchronously, process the accept directly
                    ProcessAccept(acceptEventArg);
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[Server] InvalidOperationException in StartAccept: {ex.Message}");
                // Optionally, reinitialize acceptEventArg here or handle any specific cleanup
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Exception in StartAccept: {ex.Message}");
            }
        }


        private static void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e) => ProcessAccept(e);

        private static void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket clientSocket = e.AcceptSocket;
            e.AcceptSocket = null; // Reset AcceptSocket for the next use

            try
            {
                Interlocked.Increment(ref totalClients);
                Console.WriteLine($"[Server] Client connected. Total clients: {totalClients}");

                // Assign a new or recycled SocketAsyncEventArgs for receiving data
                SocketAsyncEventArgs readEventArgs = readWritePool.Pop();
                if (readEventArgs == null)
                {
                    Console.WriteLine("No more connections can be accepted.");
                    clientSocket.Close();
                    return;
                }

                AsyncUserToken userToken = (AsyncUserToken)readEventArgs.UserToken;
                userToken.Socket = clientSocket;

                bool willRaiseEvent = clientSocket.ReceiveAsync(readEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(readEventArgs);
                }

                // Start accepting the next connection
                StartAccept(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Exception in ProcessAccept: {ex.Message}");
                CloseClientSocket(e);
            }
        }


        private static void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(e);
            }
            else if (e.LastOperation == SocketAsyncOperation.Send)
            {
                ProcessSend(e);
            }
            else
            {
                throw new ArgumentException("Unknown last operation.");
            }
        }

        private static void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = (AsyncUserToken)e.UserToken;

            try
            {
                // Check if we received any data and there were no socket errors
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    // Process the received data
                    string receivedText = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                    Console.WriteLine($"[Server] Received: {receivedText} from {userToken.Socket.RemoteEndPoint}");

                    // (Optional) Echo the message back to the client, or process it as needed
                    byte[] data = Encoding.UTF8.GetBytes("Acknowledged: " + receivedText);
                    e.SetBuffer(data, 0, data.Length);
                    bool willRaiseEvent = userToken.Socket.SendAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessSend(e); // If send completes synchronously, process it directly
                    }

                    // Reset the buffer to receive more data from the client
                    e.SetBuffer(0, e.Buffer.Length);
                    willRaiseEvent = userToken.Socket.ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(e); // If receive completes synchronously, process it directly
                    }
                }
                else
                {
                    CloseClientSocket(e); // Close socket if there's an issue
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error in ProcessReceive: {ex.Message}");
                CloseClientSocket(e);
            }
        }



        private static void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                AsyncUserToken userToken = (AsyncUserToken)e.UserToken;
                Console.WriteLine($"[Server] Send operation failed: {e.SocketError}");
                CloseClientSocket(userToken.ReceiveEventArgs);
            }
        }

        private static void BroadcastMessage(Socket senderSocket, string message)
        {
            string senderInfo = senderSocket.RemoteEndPoint.ToString();
            string formattedMessage = $"{senderInfo}: {message}";
            byte[] data = Encoding.UTF8.GetBytes(formattedMessage);

            foreach (var client in connectedClients.Values)
            {
                if (client.Socket.Connected)
                {
                    try
                    {
                        SocketAsyncEventArgs sendEventArgs = client.SendEventArgs;
                        sendEventArgs.SetBuffer(data, 0, data.Length);

                        bool willRaiseEvent = client.Socket.SendAsync(sendEventArgs);
                        if (!willRaiseEvent)
                        {
                            ProcessSend(sendEventArgs);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting to client: {ex.Message}");
                        CloseClientSocket(client.ReceiveEventArgs);
                    }
                }
            }
        }

        private static void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = (AsyncUserToken)e.UserToken;

            if (userToken.Socket == null || !userToken.Socket.Connected)
                return;

            connectedClients.TryRemove(userToken.Socket, out _);

            try
            {
                if (userToken.Socket.Connected)
                {
                    userToken.Socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Server] Exception during shutdown: {ex.Message}");
            }
            finally
            {
                userToken.Socket.Close();
                readWritePool.Push(userToken.ReceiveEventArgs); // Reuse the SocketAsyncEventArgs
                Interlocked.Decrement(ref totalClients);
                Console.WriteLine($"Client disconnected. Total clients: {totalClients}");
            }
        }
    }

    class BufferManager
    {
        private byte[] buffer;
        private int bufferSize;
        private int numBytes;
        private int currentIndex;
        private Stack<int> freeIndexPool;

        public BufferManager(int totalBytes, int bufferSize)
        {
            numBytes = totalBytes;
            this.bufferSize = bufferSize;
            currentIndex = 0;
            freeIndexPool = new Stack<int>();
        }

        public void InitBuffer()
        {
            buffer = new byte[numBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (freeIndexPool.Count > 0)
            {
                args.SetBuffer(buffer, freeIndexPool.Pop(), bufferSize);
            }
            else
            {
                if ((numBytes - bufferSize) < currentIndex)
                {
                    return false;
                }
                args.SetBuffer(buffer, currentIndex, bufferSize);
                currentIndex += bufferSize;
            }
            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }

    class SocketAsyncEventArgsPool
    {
        private ConcurrentStack<SocketAsyncEventArgs> pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            pool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                Console.WriteLine("Warning: Attempted to add a null SocketAsyncEventArgs to the pool.");
                return; // Ignore null entries
            }
            pool.Push(item);
        }


        public SocketAsyncEventArgs Pop()
        {
            pool.TryPop(out SocketAsyncEventArgs args);
            return args;
        }
    }

    class AsyncUserToken
    {
        public Socket Socket { get; set; }
        public SocketAsyncEventArgs ReceiveEventArgs { get; set; }
        public SocketAsyncEventArgs SendEventArgs { get; set; }
    }
}
