using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chattt
{
    public partial class MainForm : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 12345;

        public MainForm()
        {
            InitializeComponent();
        }

        private async Task ConnectToServer()
        {
            try
            {
                if (_client != null && _client.Connected)
                {
                    return;
                }
                _client = new TcpClient();
                await _client.ConnectAsync(SERVER_IP, SERVER_PORT);
                _stream = _client.GetStream();
            }
            catch (Exception ex)
            {
                _client?.Close();
                _client = null;
                _stream = null;
                throw new Exception($"서버 연결에 실패했습니다: {ex.Message}");
            }
        }

        private async Task<string> ReadServerResponseAsync()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    throw new Exception("서버로부터 응답을 받기 전에 연결이 끊어졌습니다.");
                }
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                _client?.Close();
                _client = null;
                _stream = null;
                throw new Exception($"서버 응답 수신 중 오류: {ex.Message}");
            }
        }


        private async Task SendMessageAsync(string message)
        {
            if (_client == null || !_client.Connected || _stream == null)
            {
                MessageBox.Show("서버에 연결되어 있지 않아 메시지를 보낼 수 없습니다.", "전송 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 전송 오류: {ex.Message}", "전송 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _client?.Close();
                _client = null;
                _stream = null;
            }
        }

        private void ProcessServerResponse(string response)
        {
            Invoke((MethodInvoker)delegate
            {
                string[] parts = response.Split(':');
                string command = parts[0];

                switch (command)
                {
                    case "LOGIN_SUCCESS":
                        string userId = parts[1];
                        MessageBox.Show($"{userId}님, 로그인 성공!", "로그인 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        ChatRoomForm chatRoomForm = new ChatRoomForm(userId, _client);
                        chatRoomForm.ShowDialog();

                        this.Close();
                        break;

                    case "LOGIN_FAIL":
                        string loginReason = parts[1];
                        MessageBox.Show($"로그인 실패: {loginReason}", "로그인 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _client?.Close();
                        break;

                    case "REGISTER_SUCCESS":
                        string registeredId = parts[1];
                        MessageBox.Show($"{registeredId}님, 회원가입 성공! 이제 로그인할 수 있습니다.", "회원가입 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        idTextBox.Text = "";
                        pwTextBox.Text = "";
                        _client?.Close();
                        break;
                    case "REGISTER_FAIL":
                        string registerReason = parts[1];
                        MessageBox.Show($"회원가입 실패: {registerReason}", "회원가입 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _client?.Close();
                        break;
                    default:
                        Console.WriteLine($"[MainForm] 알 수 없는 서버 응답: {response}");
                        _client?.Close();
                        break;
                }
            });
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID와 비밀번호를 모두 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await ConnectToServer();
                await SendMessageAsync($"LOGIN:{inputId}:{inputPw}");

                string response = await ReadServerResponseAsync();
                ProcessServerResponse(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "로그인 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void registerBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID와 비밀번호를 모두 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await ConnectToServer();
                await SendMessageAsync($"REGISTER:{inputId}:{inputPw}");

                string response = await ReadServerResponseAsync();
                ProcessServerResponse(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "회원가입 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Close();
        }
    }
}