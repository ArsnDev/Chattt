using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chattt
{
    public partial class ChatRoomForm : Form
    {
        private string _loggedInUserId;
        private TcpClient _client;
        private NetworkStream _stream;

        // UI 컨트롤들은 디자이너에서 지정한 이름과 일치해야 합니다.
        // 예를 들어 chatDisplayRichTextBox, messageInputTextBox, 
        // participantsListBox, sendButton 등이 .Designer.cs 파일에 정의되어 있어야 합니다.
        // private RichTextBox chatDisplayRichTextBox;
        // private TextBox messageInputTextBox;
        // private ListBox participantsListBox;
        // private Button sendButton;


        // MainForm에서 로그인된 사용자 ID와 연결된 TcpClient 객체를 전달받을 생성자
        public ChatRoomForm(string userId, TcpClient client)
        {
            InitializeComponent(); // UI 컨트롤 초기화

            _loggedInUserId = userId;
            _client = client;
            _stream = client.GetStream(); // 전달받은 TcpClient에서 NetworkStream 얻기

            this.Text = $"채팅방 - {_loggedInUserId}님"; // 폼 타이틀 설정

            // ==== UI 컨트롤 초기 설정 ====
            if (chatDisplayRichTextBox != null)
            {
                chatDisplayRichTextBox.ReadOnly = true;
                chatDisplayRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
                chatDisplayRichTextBox.WordWrap = true;
            }
            if (messageInputTextBox != null)
            {
                messageInputTextBox.Multiline = true;
                messageInputTextBox.ScrollBars = ScrollBars.Vertical;
                messageInputTextBox.AcceptsReturn = true; // Enter 키로 줄바꿈 허용 (Ctrl+Enter 또는 Shift+Enter로 전송)
                messageInputTextBox.KeyDown += MessageInputTextBox_KeyDown; // Enter 키 이벤트 처리
            }
            if (participantsListBox != null)
            {
                participantsListBox.Sorted = true; // 자동으로 알파벳 순 정렬
            }
            if (sendButton != null)
            {
                sendButton.Click += SendButton_Click;
            }

            // 폼 로드 이벤트 핸들러 연결
            this.Load += ChatRoomForm_Load;

            // 폼 클로징 이벤트 핸들러 연결 (리소스 정리용)
            this.FormClosing += ChatRoomForm_FormClosing;

            // 서버로부터 메시지를 계속 수신하는 루프 시작
            _ = ReceiveChatMessagesAsync(); // Fire-and-forget 패턴
        }

        // 폼이 로드된 후에 서버에 참가자 목록을 요청합니다.
        private async void ChatRoomForm_Load(object sender, EventArgs e)
        {
            AddMessageToChatDisplay($"시스템: {_loggedInUserId}님, 채팅방에 입장했습니다.", Color.Blue);

            // 서버에 현재 참가자 목록을 요청합니다.
            if (_client != null && _client.Connected)
            {
                await SendMessageAsync("REQUEST_PARTICIPANTS");
            }
            else
            {
                AddMessageToChatDisplay("시스템: 서버에 연결되어 있지 않습니다. 참가자 목록을 가져올 수 없습니다.", Color.Red);
            }
        }

        // =========================================================
        // 서버로부터 메시지 수신 및 파싱 (수정된 핵심 로직)
        // =========================================================
        private async Task ReceiveChatMessagesAsync()
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (_client != null && _client.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) // 서버가 정상적으로 연결을 끊음
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (this.Visible) // 폼이 보이는 상태일 때만 메시지 표시
                            {
                                AddMessageToChatDisplay("시스템: 서버 연결이 끊어졌습니다. 채팅방을 종료합니다.", Color.Red);
                                MessageBox.Show("서버 연결이 끊어졌습니다.", "연결 끊김", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Close(); // 폼을 닫습니다. FormClosing 이벤트가 리소스 정리를 처리합니다.
                            }
                        });
                        break; // 루프 종료
                    }
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    ProcessServerMessage(receivedMessage); // 수신 메시지 처리
                }
            }
            catch (ObjectDisposedException)
            {
                // 폼이 닫히면서 클라이언트/스트림이 이미 정리된 경우 정상적으로 발생하는 예외이므로 무시합니다.
                Console.WriteLine("[DEBUG] Receive loop aborted due to ObjectDisposedException (expected on form close).");
            }
            catch (Exception ex)
            {
                // 예상치 못한 네트워크 오류 발생
                if (!this.IsDisposed && _client.Connected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        AddMessageToChatDisplay($"시스템: 메시지 수신 오류 - {ex.Message}", Color.Red);
                        MessageBox.Show($"채팅 메시지 수신 오류: {ex.Message}", "수신 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close(); // 오류 발생 시 폼을 닫아 안전하게 종료합니다.
                    });
                }
            }
        }

        // 서버로부터 받은 메시지 종류에 따라 처리
        private void ProcessServerMessage(string message)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke((MethodInvoker)delegate // UI 업데이트를 위해 Invoke 사용
            {
                string[] parts = message.Split(new char[] { ':' }, 3); // 최대 3개로 분할 (CHAT_MESSAGE 대비)
                string command = parts[0];

                switch (command)
                {
                    case "CHAT_MESSAGE":
                        if (parts.Length >= 3)
                        {
                            string sender = parts[1];
                            string chatText = string.Join(":", parts.Skip(2)); // 메시지에 ':'가 포함될 경우를 대비
                            Color messageColor = (sender == _loggedInUserId) ? Color.DarkGreen : Color.Black;
                            AddMessageToChatDisplay($"{sender}: {chatText}", messageColor);
                        }
                        break;

                    case "PARTICIPANTS_LIST":
                        string participantListString = (parts.Length > 1) ? parts[1] : "";
                        UpdateParticipantsListBox(participantListString);
                        break;

                    case "USER_JOINED":
                        if (parts.Length > 1)
                        {
                            string joinedUser = parts[1];
                            AddMessageToChatDisplay($"시스템: {joinedUser}님이 입장했습니다.", Color.Blue);
                            AddParticipantToListBox(joinedUser);
                        }
                        break;

                    case "USER_LEFT":
                        if (parts.Length > 1)
                        {
                            string leftUser = parts[1];
                            AddMessageToChatDisplay($"시스템: {leftUser}님이 나갔습니다.", Color.Gray);
                            RemoveParticipantFromListBox(leftUser);
                        }
                        break;

                    case "ERROR":
                        if (parts.Length > 1)
                        {
                            string errorMessage = parts[1];
                            AddMessageToChatDisplay($"시스템 오류: {errorMessage}", Color.Red);
                        }
                        break;

                    default:
                        Console.WriteLine($"[DEBUG] Unknown Server Message: {message}");
                        break;
                }
            });
        }

        // 채팅창에 메시지를 추가하고 스크롤하는 헬퍼 메서드
        private void AddMessageToChatDisplay(string message, Color color)
        {
            if (chatDisplayRichTextBox.IsDisposed) return;

            chatDisplayRichTextBox.SelectionStart = chatDisplayRichTextBox.TextLength;
            chatDisplayRichTextBox.SelectionLength = 0;
            chatDisplayRichTextBox.SelectionColor = color;
            chatDisplayRichTextBox.AppendText(message + Environment.NewLine);
            chatDisplayRichTextBox.SelectionColor = chatDisplayRichTextBox.ForeColor;
            chatDisplayRichTextBox.ScrollToCaret();
        }

        // =========================================================
        // 참가자 ListBox 업데이트
        // =========================================================

        private void UpdateParticipantsListBox(string participantListString)
        {
            if (participantsListBox.IsDisposed) return;
            participantsListBox.Items.Clear();
            if (!string.IsNullOrEmpty(participantListString))
            {
                string[] users = participantListString.Split(',');
                foreach (string user in users.OrderBy(u => u.Trim()))
                {
                    if (!string.IsNullOrWhiteSpace(user))
                    {
                        participantsListBox.Items.Add(user.Trim());
                    }
                }
            }
            UpdateParticipantCountDisplay();
        }

        private void AddParticipantToListBox(string user)
        {
            if (participantsListBox.IsDisposed) return;
            if (!string.IsNullOrWhiteSpace(user) && !participantsListBox.Items.Contains(user))
            {
                participantsListBox.Items.Add(user);
                UpdateParticipantCountDisplay();
            }
        }

        private void RemoveParticipantFromListBox(string user)
        {
            if (participantsListBox.IsDisposed) return;
            if (participantsListBox.Items.Contains(user))
            {
                participantsListBox.Items.Remove(user);
                UpdateParticipantCountDisplay();
            }
        }

        private void UpdateParticipantCountDisplay()
        {
            if (this.IsDisposed) return;
            this.Text = $"채팅방 - {_loggedInUserId}님 ({participantsListBox.Items.Count}명 접속 중)";
        }

        // =========================================================
        // 메시지 전송 로직
        // =========================================================
        private async void SendButton_Click(object sender, EventArgs e)
        {
            await SendChatMessage();
        }

        private async void MessageInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendChatMessage();
            }
        }

        private async Task SendChatMessage()
        {
            string message = messageInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            try
            {
                await SendMessageAsync($"CHAT_MESSAGE:{_loggedInUserId}:{message}");
                messageInputTextBox.Clear();
                messageInputTextBox.Focus();
            }
            catch (Exception ex)
            {
                AddMessageToChatDisplay($"시스템: 메시지 전송 실패 - {ex.Message}", Color.Red);
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (_client == null || !_client.Connected || _stream == null || !_stream.CanWrite)
            {
                AddMessageToChatDisplay("시스템: 서버에 연결되어 있지 않습니다.", Color.Red);
                return;
            }
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                AddMessageToChatDisplay($"시스템: 메시지 전송 중 네트워크 오류 - {ex.Message}", Color.Red);
                this.Close(); // 전송 오류 시 연결이 끊어진 것으로 간주하고 폼을 닫음
            }
        }

        // =========================================================
        // 폼 종료 시 연결 해제 (유일한 리소스 정리 지점)
        // =========================================================
        private async void ChatRoomForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 이 이벤트 핸들러가 연결 종료를 위한 유일한 장소입니다.
            if (_client != null && _client.Connected && _loggedInUserId != null)
            {
                try
                {
                    // USER_LEFT 메시지를 보내는 것은 좋은 관행이지만,
                    // 이 시점에는 이미 연결이 불안정할 수 있으므로 예외 처리가 중요합니다.
                    await SendMessageAsync($"USER_LEFT:{_loggedInUserId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] 퇴장 메시지 전송 실패 (연결이 이미 닫혔을 수 있음): {ex.Message}");
                }
            }

            // 스트림과 클라이언트를 안전하게 닫습니다.
            _stream?.Close();
            _client?.Close();
            Console.WriteLine($"[DEBUG] 클라이언트 연결 및 스트림 리소스 정리 완료: {_loggedInUserId ?? "Guest"}");
        }
    }
}