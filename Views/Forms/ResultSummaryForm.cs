using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views
{
    public class ResultSummaryForm : Form
    {
        private DataGridView dgvResults;
        private Label lblTitle;
        private Button btnClose;

        public ResultSummaryForm(List<(Vocabulary vocab, string userAnswer, bool isCorrect)> results)
        {
            InitializeComponent();
            LoadResults(results);
        }

        private void InitializeComponent()
        {
            this.Text = "Tổng kết kết quả Quiz";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            lblTitle = new Label
            {
                Text = "📋 Tổng kết kết quả",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnClose = new Button
            {
                Text = "Đóng",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(100, 40),
                Location = new Point(290, 360)
            };
            btnClose.Click += (s, e) => this.Close();

            Controls.Add(dgvResults);
            Controls.Add(lblTitle);
            Controls.Add(btnClose);
        }

        private void LoadResults(List<(Vocabulary vocab, string userAnswer, bool isCorrect)> results)
        {
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add("Word", "Từ vựng");
            dgvResults.Columns.Add("CorrectAnswer", "Đáp án đúng");
            dgvResults.Columns.Add("UserAnswer", "Bạn chọn");
            dgvResults.Columns.Add("Result", "Kết quả");

            foreach (var result in results)
            {
                int row = dgvResults.Rows.Add();
                dgvResults.Rows[row].Cells["Word"].Value = result.vocab.Word;
                dgvResults.Rows[row].Cells["CorrectAnswer"].Value = result.vocab.Meaning;
                dgvResults.Rows[row].Cells["UserAnswer"].Value = result.userAnswer;
                dgvResults.Rows[row].Cells["Result"].Value = result.isCorrect ? "✅ Đúng" : "❌ Sai";

                dgvResults.Rows[row].DefaultCellStyle.BackColor = result.isCorrect ? Color.LightGreen : Color.MistyRose;
            }
        }
    }
}
