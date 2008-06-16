<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Messenger
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.statusCheckBox = New System.Windows.Forms.CheckBox
        Me.msgCheckBox = New System.Windows.Forms.CheckBox
        Me.inputTextBox = New System.Windows.Forms.TextBox
        Me.panel1 = New System.Windows.Forms.Panel
        Me.sendBtn = New System.Windows.Forms.Button
        Me.displayTextBox = New System.Windows.Forms.RichTextBox
        Me.panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'statusCheckBox
        '
        Me.statusCheckBox.Checked = True
        Me.statusCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.statusCheckBox.Location = New System.Drawing.Point(113, 31)
        Me.statusCheckBox.Name = "statusCheckBox"
        Me.statusCheckBox.Size = New System.Drawing.Size(61, 18)
        Me.statusCheckBox.TabIndex = 3
        Me.statusCheckBox.Text = "Status"
        Me.statusCheckBox.UseVisualStyleBackColor = True
        '
        'msgCheckBox
        '
        Me.msgCheckBox.Checked = True
        Me.msgCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.msgCheckBox.Location = New System.Drawing.Point(13, 32)
        Me.msgCheckBox.Name = "msgCheckBox"
        Me.msgCheckBox.Size = New System.Drawing.Size(94, 17)
        Me.msgCheckBox.TabIndex = 2
        Me.msgCheckBox.Text = "UserMessage"
        Me.msgCheckBox.UseVisualStyleBackColor = True
        '
        'inputTextBox
        '
        Me.inputTextBox.Location = New System.Drawing.Point(13, 6)
        Me.inputTextBox.Name = "inputTextBox"
        Me.inputTextBox.Size = New System.Drawing.Size(178, 20)
        Me.inputTextBox.TabIndex = 1
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.statusCheckBox)
        Me.panel1.Controls.Add(Me.msgCheckBox)
        Me.panel1.Controls.Add(Me.inputTextBox)
        Me.panel1.Controls.Add(Me.sendBtn)
        Me.panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panel1.Location = New System.Drawing.Point(0, 267)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(289, 56)
        Me.panel1.TabIndex = 5
        '
        'sendBtn
        '
        Me.sendBtn.Location = New System.Drawing.Point(197, 6)
        Me.sendBtn.Name = "sendBtn"
        Me.sendBtn.Size = New System.Drawing.Size(75, 23)
        Me.sendBtn.TabIndex = 0
        Me.sendBtn.Text = "Send"
        Me.sendBtn.UseVisualStyleBackColor = True
        '
        'displayTextBox
        '
        Me.displayTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.displayTextBox.Location = New System.Drawing.Point(0, 0)
        Me.displayTextBox.Name = "displayTextBox"
        Me.displayTextBox.Size = New System.Drawing.Size(289, 323)
        Me.displayTextBox.TabIndex = 6
        Me.displayTextBox.Text = ""
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(289, 323)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.displayTextBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Form1"
        Me.Text = "XDMessaging Demo"
        Me.TopMost = True
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private WithEvents statusCheckBox As System.Windows.Forms.CheckBox
    Private WithEvents msgCheckBox As System.Windows.Forms.CheckBox
    Private WithEvents inputTextBox As System.Windows.Forms.TextBox
    Private WithEvents panel1 As System.Windows.Forms.Panel
    Private WithEvents sendBtn As System.Windows.Forms.Button
    Private WithEvents displayTextBox As System.Windows.Forms.RichTextBox
End Class
