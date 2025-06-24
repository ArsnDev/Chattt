using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chattt_Server
{
    internal class ClientSession
    {
        public TcpClient _client {  get; private set; }
        public NetworkStream _stream { get; private set; }
        public string _userId { get; set; }

        public ClientSession(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _userId = null;
        }
        public async Task StartHandling()
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (_client.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        Console.WriteLine($"[Disconnected] Client: {_client.Client.RemoteEndPoint}");
                        break;
                    }

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"[Received] {_client.Client.RemoteEndPoint}: {receivedData}");

                    await ProcessClientRequestAsync(receivedData);
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine($"[Error] I/O Error ({_client.Client.RemoteEndPoint}): {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Error] Socket Error ({_client.Client.RemoteEndPoint}): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Exception Error ({_client.Client.RemoteEndPoint}): {ex.Message}");
            }
            finally
            {
                if (_userId != null)
                {
                    if (ClientManager.Instance.RemoveClient(_userId))
                    {
                        Console.WriteLine($"[Logout] User '{_userId}' Left");
                        await ClientManager.Instance.BroadcastMessageAsync($"USER_LEFT:{_userId}");
                    }
                }
                _stream?.Close();
                _client?.Close();
            }
        }
        private async Task ProcessClientRequestAsync(string requestMessage)
        {
            string[] parts = requestMessage.Split(':');
            string command = parts[0];

            switch (command)
            {
                case "LOGIN":
                    if(parts.Length == 3)
                    {
                        string id = parts[1];
                        string pw = parts[2];

                        bool isValid = UserManager.Instance.AuthenticateUser(id, pw);
                        if (isValid)
                        {
                            if (ClientManager.Instance.AddClient(id, _client))
                            {
                                this._userId = id;
                                await SendMessageToClientAsync($"LOGIN_SUCCESS:{id}");
                                Console.WriteLine($"[Login Success] UserId: {id}");
                                await ClientManager.Instance.BroadcastMessageAsync($"USER_JOINED:{this._userId}");
                                await SendParticipantsListToClientAsync();
                            }
                            else
                            {
                                await SendMessageToClientAsync("LOGIN_FAIL:AlreadyLoggedIn");
                                Console.WriteLine($"[Login Failed] Already Logged In: {id}");
                                _client.Close();
                            }
                        }
                        else
                        {
                            await SendMessageToClientAsync("LOGIN_FAIL:InvalidUser");
                            Console.WriteLine($"[Login Failed] Invalid UserId: {id}");
                        }
                    }
                    else
                    {
                        await SendMessageToClientAsync("LOGIN_FAIL:InvalidFormat");
                    }
                    break;
                case "REGISTER":
                    if (parts.Length == 3)
                    {
                        string id = parts[1];
                        string pw = parts[2];

                        bool isRegistered = UserManager.Instance.RegisterUser(id, pw);

                        if (isRegistered)
                        {
                            await SendMessageToClientAsync($"REGISTER_SUCCESS:{id}");
                            Console.WriteLine($"[Register Success] User: {id}");
                        }
                        else
                        {
                            await SendMessageToClientAsync("REGISTER_FAIL:IDExists");
                        }
                    }
                    else
                    {
                        await SendMessageToClientAsync("REGISTER_FAIL:InvalidFormat");
                    }
                    break;
                case "CHAT_MESSAGE":
                    if (this._userId != null && parts.Length >= 3)
                    {
                        string sender = parts[1];
                        string messageContent = parts[2];

                        if (sender == this._userId)
                        {
                            Console.WriteLine($"[Chat] {sender}: {messageContent}");
                            await ClientManager.Instance.BroadcastMessageAsync($"CHAT_MESSAGE:{sender}:{messageContent}");
                        }
                        else
                        {
                            await SendMessageToClientAsync("ERROR:InvalidSenderID");
                            Console.WriteLine($"[Warning] Invalid Sender ID: Client ({this._userId}) Sent as {sender}");
                        }
                    }
                    else if (this._userId == null)
                    {
                        await SendMessageToClientAsync("ERROR:NotLoggedIn");
                    }
                    else
                    {
                        await SendMessageToClientAsync("ERROR:InvalidChatMessageFormat");
                    }
                    break;
                case "REQUEST_PARTICIPANTS":
                    if (this._userId != null)
                    {
                        await SendParticipantsListToClientAsync();
                    }
                    else
                    {
                        await SendMessageToClientAsync("ERROR:NotLoggedIn");
                    }
                    break;

                default:
                    await SendMessageToClientAsync("ERROR:UnknownCommand");
                    break;
            }
        }
        private async Task SendMessageToClientAsync(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to Send Message ({this._userId ?? "Guest"}): {ex.Message}");
            }
        }
        private async Task SendParticipantsListToClientAsync()
        {
            string participantList = string.Join(",", ClientManager.Instance.GetParticipantIds());
            await SendMessageToClientAsync($"PARTICIPANTS_LIST:{participantList}");
        }
    }
}
