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
        SuspendLayout();
        // 
        // button_RunInConsole
        // 
        button_RunInConsole.Location = new Point(127, 87);
        button_RunInConsole.Name = "button_RunInConsole";
        button_RunInConsole.Size = new Size(209, 65);
        button_RunInConsole.TabIndex = 0;
        button_RunInConsole.Text = "Direct Console";
        button_RunInConsole.UseVisualStyleBackColor = true;
        button_RunInConsole.Click += button_RunInConsole_Click;
        // 
        // button_RunInConsoleClipboard
        // 
        button_RunInConsoleClipboard.Location = new Point(411, 87);
        button_RunInConsoleClipboard.Name = "button_RunInConsoleClipboard";
        button_RunInConsoleClipboard.Size = new Size(209, 65);
        button_RunInConsoleClipboard.TabIndex = 1;
        button_RunInConsoleClipboard.Text = "Clipboard Console";
        button_RunInConsoleClipboard.UseVisualStyleBackColor = true;
        button_RunInConsoleClipboard.Click += button_RunInConsoleClipboard_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(button_RunInConsoleClipboard);
        Controls.Add(button_RunInConsole);
        Name = "Form1";
        Text = "Form1";
        ResumeLayout(false);
    }

    #endregion

    private Button button_RunInConsole;
    private Button button_RunInConsoleClipboard;
}
