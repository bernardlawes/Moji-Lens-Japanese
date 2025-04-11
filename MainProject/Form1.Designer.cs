namespace MainProject;

partial class Form1
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
        button_RunInConsole = new Button();
        button_RunInConsoleClipboard = new Button();
        tableLayoutPanel1 = new TableLayoutPanel();
        button_api = new Button();
        button_flashcards = new Button();
        button_reader = new Button();
        button4 = new Button();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        // 
        // button_RunInConsole
        // 
        button_RunInConsole.AccessibleRole = AccessibleRole.WhiteSpace;
        button_RunInConsole.Dock = DockStyle.Fill;
        button_RunInConsole.FlatAppearance.BorderSize = 0;
        button_RunInConsole.FlatStyle = FlatStyle.Flat;
        button_RunInConsole.Location = new Point(3, 48);
        button_RunInConsole.Name = "button_RunInConsole";
        button_RunInConsole.Size = new Size(394, 129);
        button_RunInConsole.TabIndex = 0;
        button_RunInConsole.Text = "Direct Console";
        button_RunInConsole.UseVisualStyleBackColor = true;
        button_RunInConsole.Click += button_RunInConsole_Click;
        // 
        // button_RunInConsoleClipboard
        // 
        button_RunInConsoleClipboard.AccessibleRole = AccessibleRole.WhiteSpace;
        button_RunInConsoleClipboard.Dock = DockStyle.Fill;
        button_RunInConsoleClipboard.FlatAppearance.BorderSize = 0;
        button_RunInConsoleClipboard.FlatStyle = FlatStyle.Flat;
        button_RunInConsoleClipboard.Location = new Point(403, 48);
        button_RunInConsoleClipboard.Name = "button_RunInConsoleClipboard";
        button_RunInConsoleClipboard.Size = new Size(394, 129);
        button_RunInConsoleClipboard.TabIndex = 1;
        button_RunInConsoleClipboard.Text = "Clipboard Console";
        button_RunInConsoleClipboard.UseVisualStyleBackColor = true;
        button_RunInConsoleClipboard.Click += button_RunInConsoleClipboard_Click;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.Controls.Add(button_api, 0, 2);
        tableLayoutPanel1.Controls.Add(button_RunInConsole, 0, 1);
        tableLayoutPanel1.Controls.Add(button_RunInConsoleClipboard, 1, 1);
        tableLayoutPanel1.Controls.Add(button_flashcards, 1, 2);
        tableLayoutPanel1.Controls.Add(button_reader, 0, 3);
        tableLayoutPanel1.Controls.Add(button4, 1, 3);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 4;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        tableLayoutPanel1.Size = new Size(800, 450);
        tableLayoutPanel1.TabIndex = 2;
        // 
        // button_api
        // 
        button_api.AccessibleRole = AccessibleRole.WhiteSpace;
        button_api.Dock = DockStyle.Fill;
        button_api.FlatAppearance.BorderSize = 0;
        button_api.FlatStyle = FlatStyle.Flat;
        button_api.Location = new Point(3, 183);
        button_api.Name = "button_api";
        button_api.Size = new Size(394, 129);
        button_api.TabIndex = 2;
        button_api.Text = "API";
        button_api.UseVisualStyleBackColor = true;
        button_api.Click += button_api_Click;
        // 
        // button_flashcards
        // 
        button_flashcards.AccessibleRole = AccessibleRole.WhiteSpace;
        button_flashcards.Dock = DockStyle.Fill;
        button_flashcards.FlatAppearance.BorderSize = 0;
        button_flashcards.FlatStyle = FlatStyle.Flat;
        button_flashcards.Location = new Point(403, 183);
        button_flashcards.Name = "button_flashcards";
        button_flashcards.Size = new Size(394, 129);
        button_flashcards.TabIndex = 3;
        button_flashcards.Text = "Flash Cards";
        button_flashcards.UseVisualStyleBackColor = true;
        // 
        // button_reader
        // 
        button_reader.AccessibleRole = AccessibleRole.WhiteSpace;
        button_reader.Dock = DockStyle.Fill;
        button_reader.FlatAppearance.BorderSize = 0;
        button_reader.FlatStyle = FlatStyle.Flat;
        button_reader.Location = new Point(3, 318);
        button_reader.Name = "button_reader";
        button_reader.Size = new Size(394, 129);
        button_reader.TabIndex = 4;
        button_reader.Text = "Reader";
        button_reader.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        button4.AccessibleRole = AccessibleRole.WhiteSpace;
        button4.Dock = DockStyle.Fill;
        button4.FlatAppearance.BorderSize = 0;
        button4.FlatStyle = FlatStyle.Flat;
        button4.Location = new Point(403, 318);
        button4.Name = "button4";
        button4.Size = new Size(394, 129);
        button4.TabIndex = 5;
        button4.Text = "button4";
        button4.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(tableLayoutPanel1);
        Name = "Form1";
        Text = "Form1";
        tableLayoutPanel1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Button button_RunInConsole;
    private Button button_RunInConsoleClipboard;
    private TableLayoutPanel tableLayoutPanel1;
    private Button button_api;
    private Button button_flashcards;
    private Button button_reader;
    private Button button4;
}
