'=============================================================================
'
'	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
'
'   http://www.TheCodeKing.co.uk
'  
'	All rights reserved.
'	The code and information is provided "as-is" without waranty of any kind,
'	either expresed or implied. Please do not use commerically without permission.
'
'-----------------------------------------------------------------------------
'	History:
'		25/02/2007	Michael Carlisle				Version 1.0 VB Port
'	    12/12/2009	Michael Carlisle				Version 2.0 VB Port
'=============================================================================
'
Imports System.ComponentModel
Imports TheCodeKing.Net.Messaging
Imports TheCodeKing.Net.Messaging.XDListener

Public Class Messenger
    Private listener As IXDListener
    Private broadcast As IXDBroadcast
    Private Delegate Sub UpdateDisplay(ByVal dataGram As DataGram)

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        Me.UpdateDisplayText("Launch multiple instances to demo inter-process communication." & ChrW(13) & ChrW(10), Color.Gray)
        Me.Text = (Me.Text & String.Format("Window Id: {0}", MyBase.Handle))
        Me.listener = XDListener.CreateListener(XDTransportMode.IOStream)
        AddHandler Me.listener.MessageReceived, New XDMessageHandler(AddressOf Me.OnMessageReceived)
        Me.listener.RegisterChannel("Status")
        Me.listener.RegisterChannel("UserMessage")
        Me.broadcast = XDBroadcast.CreateBroadcast(XDTransportMode.IOStream)
        Me.broadcast.SendToChannel("Status", String.Format("Window {0} created!", MyBase.Handle))
    End Sub
    Private Sub OnMessageReceived(ByVal sender As Object, ByVal e As XDMessageEventArgs)
        If MyBase.InvokeRequired Then
            Try
                Dim callback As New UpdateDisplay(AddressOf UpdateDisplayText)
                MyBase.Invoke(callback, e.DataGram)
            Catch
            End Try
        Else
            Me.UpdateDisplayText(e.DataGram)
        End If
    End Sub
    Private Sub UpdateDisplayText(ByVal dataGram As DataGram)
        Dim textColor As Color
        Dim channel As String = dataGram.Channel.ToLower
        If ((Not channel Is Nothing) AndAlso (channel = "status")) Then
            textColor = Color.Green
        Else
            textColor = Color.Blue
        End If
        Dim msg As String = String.Format("{0}: {1}" & ChrW(13) & ChrW(10), dataGram.Channel, dataGram.Message)
        Me.UpdateDisplayText(msg, textColor)
    End Sub
    Private Sub UpdateDisplayText(ByVal message As String, ByVal textColor As Color)
        Me.displayTextBox.AppendText(message)
        Me.displayTextBox.Select(((Me.displayTextBox.Text.Length - message.Length) + 1), Me.displayTextBox.Text.Length)
        Me.displayTextBox.SelectionColor = textColor
        Me.displayTextBox.Select(Me.displayTextBox.Text.Length, Me.displayTextBox.Text.Length)
        Me.displayTextBox.ScrollToCaret()
    End Sub
    Private Sub sendBtn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles sendBtn.Click
        If (Me.inputTextBox.Text.Length > 0) Then
            Me.broadcast.SendToChannel("UserMessage", String.Format("{0}: {1}", MyBase.Handle, Me.inputTextBox.Text))
            Me.inputTextBox.Text = ""
        End If
    End Sub

    Private Sub statusCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles statusCheckBox.Click
        If Me.statusCheckBox.Checked Then
            Me.listener.RegisterChannel("Status")
            Me.broadcast.SendToChannel("Status", String.Format("{0}: Registering for Status.", MyBase.Handle))
        Else
            Me.listener.UnRegisterChannel("Status")
            Me.broadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for Status.", MyBase.Handle))
        End If
    End Sub
    Private Sub msgCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles msgCheckBox.Click
        If Me.msgCheckBox.Checked Then
            Me.listener.RegisterChannel("UserMessage")
            Me.broadcast.SendToChannel("Status", String.Format("{0}: Registering for UserMessage.", MyBase.Handle))
        Else
            Me.listener.UnRegisterChannel("UserMessage")
            Me.broadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for UserMessage.", MyBase.Handle))
        End If
    End Sub
    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        MyBase.OnClosing(e)
        Me.broadcast.SendToChannel("Status", String.Format("Window {0} closing!", MyBase.Handle))
    End Sub
    Protected Overrides Function ProcessCmdKey(ByRef m As Message, ByVal k As Keys) As Boolean
        If ((m.Msg = &H100) AndAlso (k = Keys.Return)) Then
            If (Me.inputTextBox.Text.Length > 0) Then
                Me.broadcast.SendToChannel("UserMessage", String.Format("{0}: {1}", MyBase.Handle, Me.inputTextBox.Text))
                Me.inputTextBox.Text = ""
            End If
            Return True
        End If
        Return MyBase.ProcessCmdKey((m), k)
    End Function

End Class
