// AddToTopicForm.Designer.cs
namespace WordVaultAppMVC.Views
{
    partial class AddToTopicForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.ComboBox cboTopics;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblNewTopic;
        private System.Windows.Forms.TextBox txtNewTopic;
        private System.Windows.Forms.Button btnCreateTopic;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblWord = new System.Windows.Forms.Label();
            this.cboTopics = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblNewTopic = new System.Windows.Forms.Label();
            this.txtNewTopic = new System.Windows.Forms.TextBox();
            this.btnCreateTopic = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblWord
            this.lblWord.AutoSize = true;
            this.lblWord.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblWord.Location = new System.Drawing.Point(20, 20);
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(100, 28);
            this.lblWord.TabIndex = 0;
            this.lblWord.Text = "Từ: (word)";

            // cboTopics
            this.cboTopics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTopics.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboTopics.Location = new System.Drawing.Point(20, 60);
            this.cboTopics.Name = "cboTopics";
            this.cboTopics.Size = new System.Drawing.Size(300, 33);
            this.cboTopics.TabIndex = 1;

            // btnAdd
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnAdd.Location = new System.Drawing.Point(20, 110);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 40);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Thêm vào chủ đề";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // lblNewTopic
            this.lblNewTopic.AutoSize = true;
            this.lblNewTopic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNewTopic.Location = new System.Drawing.Point(20, 170);
            this.lblNewTopic.Name = "lblNewTopic";
            this.lblNewTopic.Size = new System.Drawing.Size(134, 23);
            this.lblNewTopic.TabIndex = 3;
            this.lblNewTopic.Text = "Tạo chủ đề mới:";

            // txtNewTopic
            this.txtNewTopic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewTopic.Location = new System.Drawing.Point(20, 200);
            this.txtNewTopic.Name = "txtNewTopic";
            this.txtNewTopic.Size = new System.Drawing.Size(200, 30);
            this.txtNewTopic.TabIndex = 4;

            // btnCreateTopic
            this.btnCreateTopic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCreateTopic.Location = new System.Drawing.Point(230, 198);
            this.btnCreateTopic.Name = "btnCreateTopic";
            this.btnCreateTopic.Size = new System.Drawing.Size(90, 32);
            this.btnCreateTopic.TabIndex = 5;
            this.btnCreateTopic.Text = "Tạo";
            this.btnCreateTopic.UseVisualStyleBackColor = true;
            this.btnCreateTopic.Click += new System.EventHandler(this.btnCreateTopic_Click);

            // AddToTopicForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 260);
            this.Controls.Add(this.lblWord);
            this.Controls.Add(this.cboTopics);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblNewTopic);
            this.Controls.Add(this.txtNewTopic);
            this.Controls.Add(this.btnCreateTopic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddToTopicForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm vào Chủ đề";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
