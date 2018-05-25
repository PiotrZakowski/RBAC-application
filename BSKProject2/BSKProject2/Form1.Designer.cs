namespace BSKProject2
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hasloTextBox = new System.Windows.Forms.TextBox();
            this.zalogujButton = new System.Windows.Forms.Button();
            this.profilComboBox = new System.Windows.Forms.ComboBox();
            this.wybierzButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(132, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login";
            // 
            // loginTextBox
            // 
            this.loginTextBox.Location = new System.Drawing.Point(58, 41);
            this.loginTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(200, 20);
            this.loginTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(132, 72);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hasło";
            // 
            // hasloTextBox
            // 
            this.hasloTextBox.Location = new System.Drawing.Point(58, 94);
            this.hasloTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.hasloTextBox.Name = "hasloTextBox";
            this.hasloTextBox.PasswordChar = '*';
            this.hasloTextBox.Size = new System.Drawing.Size(200, 20);
            this.hasloTextBox.TabIndex = 3;
            // 
            // zalogujButton
            // 
            this.zalogujButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.zalogujButton.Location = new System.Drawing.Point(106, 130);
            this.zalogujButton.Margin = new System.Windows.Forms.Padding(2);
            this.zalogujButton.Name = "zalogujButton";
            this.zalogujButton.Size = new System.Drawing.Size(103, 31);
            this.zalogujButton.TabIndex = 4;
            this.zalogujButton.Text = "Zaloguj";
            this.zalogujButton.UseVisualStyleBackColor = true;
            this.zalogujButton.Click += new System.EventHandler(this.zalogujButton_Click);
            // 
            // profilComboBox
            // 
            this.profilComboBox.Enabled = false;
            this.profilComboBox.FormattingEnabled = true;
            this.profilComboBox.Location = new System.Drawing.Point(81, 185);
            this.profilComboBox.Name = "profilComboBox";
            this.profilComboBox.Size = new System.Drawing.Size(200, 21);
            this.profilComboBox.TabIndex = 5;
            // 
            // wybierzButton
            // 
            this.wybierzButton.Enabled = false;
            this.wybierzButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.wybierzButton.Location = new System.Drawing.Point(106, 218);
            this.wybierzButton.Name = "wybierzButton";
            this.wybierzButton.Size = new System.Drawing.Size(103, 31);
            this.wybierzButton.TabIndex = 6;
            this.wybierzButton.Text = "Wybierz";
            this.wybierzButton.UseVisualStyleBackColor = true;
            this.wybierzButton.Click += new System.EventHandler(this.wybierzButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(32, 186);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Profil";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 261);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.wybierzButton);
            this.Controls.Add(this.profilComboBox);
            this.Controls.Add(this.zalogujButton);
            this.Controls.Add(this.hasloTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox hasloTextBox;
        private System.Windows.Forms.Button zalogujButton;
        private System.Windows.Forms.ComboBox profilComboBox;
        private System.Windows.Forms.Button wybierzButton;
        private System.Windows.Forms.Label label3;
    }
}

