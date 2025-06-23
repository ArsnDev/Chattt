using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chattt
{
    public partial class ChatRoomForm : Form
    {
        private string _loggedInUserId;
        
        public ChatRoomForm(string loggedInUserId)
        {
            InitializeComponent();
            _loggedInUserId = loggedInUserId;
            this.Text = $"User - {_loggedInUserId}";
            chatTextBox.AcceptsReturn = true;
            chatTextBox.KeyDown += ChatTextBox_KeyDown;

            chatListBox.Sorted = true;

            chatButton.Click += SendButton_Click;
            this.Load += ChatRoomForm_Load;
        }

        private void ChatRoomForm_Load(object? sender, EventArgs e)
        {
            AddMessageToChatDisplay($"시스템: {_loggedInUserId}님, 채팅방에 입장하셨습니다.", Color.Blue);

            AddParticipantToListBox(_loggedInUserId);
            AddParticipantToListBox("Alice");
            AddParticipantToListBox("Bob");
            AddParticipantToListBox("Charlie");
        }
        /// <summary>
        /// 참가자를 ListBox에 추가합니다.
        /// </summary>
        /// <param name="loggedInUserId"></param>
        private void AddParticipantToListBox(string loggedInUserId)
        {
            if(!chatListBox.Items.Contains(loggedInUserId))
            {
                chatListBox.Items.Add(loggedInUserId);
            }
        }
        /// <summary>
        /// 참가자를 ListBox에서 제거합니다.
        /// </summary>
        private void RemoveParticipantFromListBox(string loggedInUserId)
        {
            if(chatListBox.Items.Contains(loggedInUserId))
            {
                chatListBox.Items.Remove(loggedInUserId);
                AddMessageToChatDisplay($"시스템: {_loggedInUserId}님, 채팅방에서 나갔습니다.", Color.Crimson);
            }
        }
        /// <summary>
        /// 채팅창에 메시지를 추가하고 스크롤하는 헬퍼 메서드
        /// </summary>
        /// <param name="message">표시할 메시지 내용</param>
        /// <param name="color">메시지 텍스트 색상 (기본값: 검정)</param>
        private void AddMessageToChatDisplay(string message, Color color = default(Color))
        {
            if (color == default(Color))
            {
                color = Color.Black;
            }
            chatRichTextBox.SelectionStart = chatRichTextBox.TextLength;
            chatRichTextBox.SelectionLength = 0;
            chatRichTextBox.SelectionColor = color;
            chatRichTextBox.AppendText(message + Environment.NewLine);
            chatRichTextBox.SelectionColor = chatRichTextBox.ForeColor;

            chatRichTextBox.ScrollToCaret();
        }
        /// <summary>
        /// '전송' 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void SendButton_Click(object? sender, EventArgs e)
        {
            SendLocalChatMessage();
        }
        /// <summary>
        /// 메시지 입력창에서 키 눌렀을 때 처리 (Enter 키 전송)
        /// </summary>
        private void ChatTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                SendLocalChatMessage();
            }
        }
        /// <summary>
        /// 로컬에서 채팅 메시지를 처리하고 표시하는 메서드 (서버 없음)
        /// </summary>
        private void SendLocalChatMessage()
        {
            string message = chatTextBox.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            AddMessageToChatDisplay($"{_loggedInUserId}: {message}", Color.DarkGreen);

            chatTextBox.Clear();
            chatTextBox.Focus();

            // (TODO: 나중에 서버 연동 시, 여기에 SendMessageAsync 호출 로직이 들어갑니다.)
            // await SendMessageAsync($"CHAT_MESSAGE:{_loggedInUserId}:{message}");
        }
    }
}
