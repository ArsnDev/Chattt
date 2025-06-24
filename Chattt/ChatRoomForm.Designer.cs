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
            chatDisplayRichTextBox = new RichTextBox();
            messageInputTextBox = new TextBox();
            participantsListBox = new ListBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // chatDisplayRichTextBox
            // 
            chatDisplayRichTextBox.Location = new Point(-3, 1);
            chatDisplayRichTextBox.Name = "chatDisplayRichTextBox";
            chatDisplayRichTextBox.ReadOnly = true;
            chatDisplayRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            chatDisplayRichTextBox.Size = new Size(609, 360);
            chatDisplayRichTextBox.TabIndex = 0;
            chatDisplayRichTextBox.Text = "";
            // 
            // messageInputTextBox
            // 
            messageInputTextBox.Location = new Point(0, 393);
            messageInputTextBox.Name = "messageInputTextBox";
            messageInputTextBox.Size = new Size(606, 23);
            messageInputTextBox.TabIndex = 1;
            // 
            // participantsListBox
            // 
            participantsListBox.FormattingEnabled = true;
            participantsListBox.ItemHeight = 15;
            participantsListBox.Location = new Point(612, 1);
            participantsListBox.Name = "participantsListBox";
            participantsListBox.Size = new Size(176, 364);
            participantsListBox.TabIndex = 2;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(645, 394);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(75, 23);
            sendButton.TabIndex = 3;
            sendButton.Text = "입력";
            sendButton.UseVisualStyleBackColor = true;
            // 
            // ChatRoomForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(sendButton);
            Controls.Add(participantsListBox);
            Controls.Add(messageInputTextBox);
            Controls.Add(chatDisplayRichTextBox);
            Name = "ChatRoomForm";
            Text = "ChatRoomForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatDisplayRichTextBox;
        private TextBox messageInputTextBox;
        private ListBox participantsListBox;
        private Button sendButton;
    }
}