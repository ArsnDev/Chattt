// MainForm.cs (������ ����)

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

        // ConnectToServer���� ��׶��� �޽��� ���� ������ �����մϴ�.
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
                // ���ܸ� �ٽ� ������ ȣ���� �ʿ��� ó���ϵ��� �մϴ�.
                throw new Exception($"���� ���ῡ �����߽��ϴ�: {ex.Message}");
            }
        }

        // �α���/ȸ������ �� '����' ���丸 �о���� �޼��带 ���� ����ϴ�.
        private async Task<string> ReadServerResponseAsync()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    // ������ ���� ���� ������ ���� ���
                    throw new Exception("�����κ��� ������ �ޱ� ���� ������ ���������ϴ�.");
                }
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                // �б� ���� �߻� �� ������ �ݰ� ���ܸ� �����ϴ�.
                _client?.Close();
                _client = null;
                _stream = null;
                throw new Exception($"���� ���� ���� �� ����: {ex.Message}");
            }
        }


        private async Task SendMessageAsync(string message)
        {
            if (_client == null || !_client.Connected || _stream == null)
            {
                MessageBox.Show("������ ����Ǿ� ���� �ʾ� �޽����� ���� �� �����ϴ�.", "���� ����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�޽��� ���� ����: {ex.Message}", "���� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _client?.Close();
                _client = null;
                _stream = null;
            }
        }

        private void ProcessServerResponse(string response)
        {
            // UI �����忡�� ����ǵ��� ����
            Invoke((MethodInvoker)delegate
            {
                string[] parts = response.Split(':');
                string command = parts[0];

                switch (command)
                {
                    case "LOGIN_SUCCESS":
                        string userId = parts[1];
                        MessageBox.Show($"{userId}��, �α��� ����!", "�α��� ����", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        // ChatRoomForm�� TcpClient �������� �Ѱ��ݴϴ�.
                        ChatRoomForm chatRoomForm = new ChatRoomForm(userId, _client);
                        chatRoomForm.ShowDialog();

                        // ChatRoomForm�� ������ MainForm�� �ݽ��ϴ�.
                        this.Close();
                        break;

                    case "LOGIN_FAIL":
                        string loginReason = parts[1];
                        MessageBox.Show($"�α��� ����: {loginReason}", "�α��� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // �α��� ���� �� ������ �ݾ��ݴϴ�.
                        _client?.Close();
                        break;

                    // (���� ȸ������ �� ��Ÿ ���� ó�� �ڵ�� ����)
                    case "REGISTER_SUCCESS":
                        string registeredId = parts[1];
                        MessageBox.Show($"{registeredId}��, ȸ������ ����! ���� �α����� �� �ֽ��ϴ�.", "ȸ������ ����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        idTextBox.Text = "";
                        pwTextBox.Text = "";
                        _client?.Close(); // ȸ������ �Ŀ��� ������ �ݰ� �ٽ� �α����ϵ��� ����
                        break;
                    case "REGISTER_FAIL":
                        string registerReason = parts[1];
                        MessageBox.Show($"ȸ������ ����: {registerReason}", "ȸ������ ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _client?.Close();
                        break;
                    default:
                        Console.WriteLine($"[MainForm] �� �� ���� ���� ����: {response}");
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
                MessageBox.Show("ID�� ��й�ȣ�� ��� �Է����ּ���.", "�Է� ����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await ConnectToServer();
                await SendMessageAsync($"LOGIN:{inputId}:{inputPw}");

                // ���� ��, ������ ��ٸ��ϴ�.
                string response = await ReadServerResponseAsync();
                ProcessServerResponse(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�α��� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void registerBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID�� ��й�ȣ�� ��� �Է����ּ���.", "�Է� ����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await ConnectToServer();
                await SendMessageAsync($"REGISTER:{inputId}:{inputPw}");

                // ���� ��, ������ ��ٸ��ϴ�.
                string response = await ReadServerResponseAsync();
                ProcessServerResponse(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ȸ������ ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ���� ���� �� �ڿ��� �����ϴ� ���� ���� �����Դϴ�.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client?.Close();
        }
    }
}