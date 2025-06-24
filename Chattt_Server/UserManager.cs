using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chattt_Server
{
    internal class UserManager
    {
        private static readonly UserManager _instance = new UserManager();
        private ConcurrentDictionary<string, string> _usersInMemory = new ConcurrentDictionary<string, string>();
        private UserManager()
        {
            _usersInMemory.TryAdd("testuser", "1234");
            _usersInMemory.TryAdd("user1", "pass1");
            Console.WriteLine("[UserManager] 초기 테스트 계정 로드 완료.");
        }
        public static UserManager Instance
        {
            get { return _instance; }
        }
        /// <summary>
        /// 회원가입
        /// </summary>
        /// <param name="id">회원가입 할 사용자 ID</param>
        /// <param name="password">회원가입 할 비밀번호</param>
        /// <returns>등록 성공 시 true, ID가 이미 존재하면 false</returns>
        public bool RegisterUser(string id, string password)
        {
            if (_usersInMemory.TryAdd(id, password))
            {
                Console.WriteLine($"[UserManager] Registered: '{id}'");
                return true;
            }
            Console.WriteLine($"[UserManager] Register Failed: ID '{id}' already Exists");
            return false;
        }
        /// <summary>
        /// 사용자 ID와 비밀번호를 비교하여 로그인 인증
        /// </summary>
        /// <param name="id">로그인할 사용자 ID</param>
        /// <param name="password">입력된 비밀번호</param>
        /// <returns>인증 성공 시 true, 실패 시 false</returns>
        public bool AuthenticateUser(string id, string password)
        {
            if (_usersInMemory.TryGetValue(id, out string storedPassword))
            {
                if (storedPassword == password)
                {
                    Console.WriteLine($"[UserManager] Logined: '{id}'");
                    return true;
                }
            }
            Console.WriteLine($"[UserManager] Login Failed: ID '{id}' or PW Wrong");
            return false;
        }
    }
}
