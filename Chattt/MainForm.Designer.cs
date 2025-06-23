namespace Chattt
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            registerBtn = new Button();
            loginBtn = new Button();
            label2 = new Label();
            label1 = new Label();
            pwTextBox = new TextBox();
            idTextBox = new TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(registerBtn);
            groupBox1.Controls.Add(loginBtn);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(pwTextBox);
            groupBox1.Controls.Add(idTextBox);
            groupBox1.Location = new Point(50, 28);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(639, 312);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // registerBtn
            // 
            registerBtn.Location = new Point(385, 166);
            registerBtn.Name = "registerBtn";
            registerBtn.Size = new Size(114, 47);
            registerBtn.TabIndex = 5;
            registerBtn.Text = "회원가입";
            registerBtn.UseVisualStyleBackColor = true;
            registerBtn.Click += registerBtn_Click;
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(385, 96);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(114, 48);
            loginBtn.TabIndex = 4;
            loginBtn.Text = "로그인";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 168);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 3;
            label2.Text = "PW :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 116);
            label1.Name = "label1";
            label1.Size = new Size(26, 15);
            label1.TabIndex = 2;
            label1.Text = "ID :";
            // 
            // pwTextBox
            // 
            pwTextBox.Location = new Point(87, 165);
            pwTextBox.Name = "pwTextBox";
            pwTextBox.Size = new Size(219, 23);
            pwTextBox.TabIndex = 1;
            pwTextBox.UseSystemPasswordChar = true;
            // 
            // idTextBox
            // 
            idTextBox.Location = new Point(87, 113);
            idTextBox.Name = "idTextBox";
            idTextBox.Size = new Size(219, 23);
            idTextBox.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Name = "MainForm";
            Text = "Chattt";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private TextBox pwTextBox;
        private TextBox idTextBox;
        private Button registerBtn;
        private Button loginBtn;
        private Label label2;
    }
}
