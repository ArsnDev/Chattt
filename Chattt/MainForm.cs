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
        /// '�α���' ��ư Ŭ�� �� ȣ��� �̺�Ʈ �ڵ鷯
        /// </summary>
        /// <param name="sender">�̺�Ʈ�� �߻���Ų ��Ʈ�� (���⼭�� loginBtn)</param>
        /// <param name="e">�̺�Ʈ ���� (Ŭ�� �̺�Ʈ������ ���� ������ ����)</param>
        private void loginBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID�� ��й�ȣ�� ��� �Է����ּ���.", "�Է� ����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // �α��� �õ�

            // ������
            this.Hide();
            ChatRoomForm chatRoom = new ChatRoomForm(inputId);
            chatRoom.ShowDialog();
            this.Close();
        }
        /// <summary>
        /// 'ȸ������' ��ư Ŭ�� �� ȣ��� �̺�Ʈ �ڵ鷯
        /// </summary>
        /// <param name="sender">�̺�Ʈ�� �߻���Ų ��Ʈ�� (���⼭�� registerBtn)</param>
        /// <param name="e">�̺�Ʈ ���� (Ŭ�� �̺�Ʈ������ ���� ������ ����)</param>
        private void registerBtn_Click(object sender, EventArgs e)
        {
            string inputId = idTextBox.Text.Trim();
            string inputPw = pwTextBox.Text.Trim();

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPw))
            {
                MessageBox.Show("ID�� ��й�ȣ�� ��� �Է����ּ���.", "�Է� ����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ȸ������ �õ�
            MessageBox.Show($"ȸ������ �õ�: ID={inputId}, PW={inputPw}");
        }
    }
}
