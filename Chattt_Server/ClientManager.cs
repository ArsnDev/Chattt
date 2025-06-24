using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

// 접속중인 멤버를 관리하는 ClientManager
namespace Chattt_Server
{
    internal class ClientManager
    {
        private static readonly ClientManager _instance = new ClientManager();

        private ClientManager()
        {
            Console.WriteLine("[ClientManager] Initialized");
        }
        public static ClientManager Instance
        {
            get { return _instance; }
        }
        private static ConcurrentDictionary<string, TcpClient> _connectedClients = new ConcurrentDictionary<string, TcpClient>();

        /// <summary>
        /// 로그인 성공 시 클라이언트를 목록에 추가
        /// </summary>
        /// <param name="userId">로그인된 클라이언트 ID</param>
        /// <param name="tcpClient">연결된 tcpClient</param>
        /// <returns>추가되면 true, ID가 이미 존재하면 false</returns>
        public bool AddClient(string userId, TcpClient tcpClient)
        {
            if (_connectedClients.TryAdd(userId, tcpClient))
            {
                Console.WriteLine($"[ClientManager] Client Added: {userId})");
                return true;
            }
            Console.WriteLine($"[ClientManager] Client Failed to Add (Already in List): {userId}");
            return false;
        }
        /// <summary>
        /// 클라이언트 목록에서 제거
        /// </summary>
        /// <param name="userId">로그아웃 할 클라이언트 ID </param>
        /// <returns>제거 성공 시 true, ID가 없으면 false</returns>
        public bool RemoveClient(string userId)
        {
            if (_connectedClients.TryRemove(userId, out _))
            {
                Console.WriteLine($"[ClientManager] Client Removed: {userId}");
                return true;
            }
            Console.WriteLine($"[ClientManager] Client Failed to Remove (Not in List): {userId}");
            return false;
        }

        /// <summary>
        /// 현재 접속 중인 모든 클라이언트의 ID 목록을 반환
        /// </summary>
        public IEnumerable<string> GetParticipantIds()
        {
            return _connectedClients.Keys.OrderBy(id => id);
        }

        /// <summary>
        /// 접속 중인 모든 클라이언트에게 메세지 브로드캐스트
        /// </summary>
        /// <param name="message">보낼 내용</param>
        public async Task BroadcastMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var client in _connectedClients)
            {
                TcpClient targetClient = client.Value;
                if(targetClient.Connected)
                {
                    try
                    {
                        NetworkStream targetStream = targetClient.GetStream();
                        await targetStream.WriteAsync(data, 0, data.Length);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}
