namespace Chattt
{
    partial class ChatRoomForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chatRichTextBox = new RichTextBox();
            chatTextBox = new TextBox();
            chatListBox = new ListBox();
            chatButton = new Button();
            SuspendLayout();
            // 
            // chatDisplayRichTextBox
            // 
            chatRichTextBox.Location = new Point(-3, 1);
            chatRichTextBox.Name = "chatDisplayRichTextBox";
            chatRichTextBox.ReadOnly = true;
            chatRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            chatRichTextBox.Size = new Size(609, 360);
            chatRichTextBox.TabIndex = 0;
            chatRichTextBox.Text = "";
            // 
            // chatTextBox
            // 
            chatTextBox.Location = new Point(0, 393);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.Size = new Size(606, 23);
            chatTextBox.TabIndex = 1;
            // 
            // chatListBox
            // 
            chatListBox.FormattingEnabled = true;
            chatListBox.ItemHeight = 15;
            chatListBox.Location = new Point(612, 1);
            chatListBox.Name = "chatListBox";
            chatListBox.Size = new Size(176, 364);
            chatListBox.TabIndex = 2;
            // 
            // chatButton
            // 
            chatButton.Location = new Point(645, 394);
            chatButton.Name = "chatButton";
            chatButton.Size = new Size(75, 23);
            chatButton.TabIndex = 3;
            chatButton.Text = "입력";
            chatButton.UseVisualStyleBackColor = true;
            // 
            // ChatRoomForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(chatButton);
            Controls.Add(chatListBox);
            Controls.Add(chatTextBox);
            Controls.Add(chatRichTextBox);
            Name = "ChatRoomForm";
            Text = "ChatRoomForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatRichTextBox;
        private TextBox chatTextBox;
        private ListBox chatListBox;
        private Button chatButton;
    }
}