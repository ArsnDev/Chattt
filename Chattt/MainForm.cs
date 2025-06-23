using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chattt
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// '로그인' 버튼 클릭 시 호출될 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트를 발생시킨 컨트롤 (여기서는 loginBtn)</param>
        /// <param name="e">이벤트 인자 (클릭 이벤트에서는 보통 사용되지 않음)</param>
        private void loginBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID와 비밀번호를 모두 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 로그인 시도

            // 성공시
            this.Hide();
            ChatRoomForm chatRoom = new ChatRoomForm(inputId);
            chatRoom.ShowDialog();
            this.Close();
        }
        /// <summary>
        /// '회원가입' 버튼 클릭 시 호출될 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트를 발생시킨 컨트롤 (여기서는 registerBtn)</param>
        /// <param name="e">이벤트 인자 (클릭 이벤트에서는 보통 사용되지 않음)</param>
        private void registerBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID와 비밀번호를 모두 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 회원가입 시도
            MessageBox.Show($"회원가입 시도: ID={inputId}, PW={inputPw}");
        }
    }
}
