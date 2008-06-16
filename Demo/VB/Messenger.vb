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
'=============================================================================
'
Imports System.ComponentModel
Imports TheCodeKing.Net.Messaging

Public Class Messenger
    Private listener As XDListener

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        Me.Text = (Me.Text & String.Format("Window Id: {0}", MyBase.Handle))
        Me.listener = New XDListener
        AddHandler Me.listener.MessageReceived, New XDListener.XDMessageHandler(AddressOf Me.listener_MessageReceived)
        Me.listener.RegisterChannel("Status")
        Me.listener.RegisterChannel("UserMessage")
        XDBroadcast.SendToChannel("Status", String.Format("Window {0} created!", MyBase.Handle))
    End Sub

    Private Sub listener_MessageReceived(ByVal sender As Object, ByVal e As XDMessageEventArgs)
        Dim green As Color
        Dim text2 As String
        text2 = e.DataGram.Channel.ToLower
        If (text2 = "status") Then
            green = Color.Green
        Else
            green = Color.Blue
        End If
        Dim text As String = String.Format("{0}: {1}" & ChrW(13) & ChrW(10), e.DataGram.Channel, e.DataGram.Message)
        Me.displayTextBox.AppendText([text])
        Me.displayTextBox.Select(((Me.displayTextBox.Text.Length - [text].Length) + 1), Me.displayTextBox.Text.Length)
        Me.displayTextBox.SelectionColor = green
        Me.displayTextBox.Select(Me.displayTextBox.Text.Length, Me.displayTextBox.Text.Length)
        Me.displayTextBox.ScrollToCaret()
    End Sub

    Private Sub sendBtn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles sendBtn.Click
        If (Me.inputTextBox.Text.Length > 0) Then
            XDBroadcast.SendToChannel("UserMessage", String.Format("{0}: {1}", MyBase.Handle, Me.inputTextBox.Text))
            Me.inputTextBox.Text = ""
        End If
    End Sub

    Private Sub statusCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles statusCheckBox.Click
        If Me.statusCheckBox.Checked Then
            Me.listener.RegisterChannel("Status")
            XDBroadcast.SendToChannel("Status", String.Format("{0}: Registering for Status.", MyBase.Handle))
        Else
            Me.listener.UnRegisterChannel("Status")
            XDBroadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for Status.", MyBase.Handle))
        End If
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        MyBase.OnClosing(e)
        XDBroadcast.SendToChannel("Status", String.Format("Window {0} closing!", MyBase.Handle))
    End Sub

    Private Sub msgCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles msgCheckBox.Click
        If Me.msgCheckBox.Checked Then
            Me.listener.RegisterChannel("UserMessage")
            XDBroadcast.SendToChannel("Status", String.Format("{0}: Registering for UserMessage.", MyBase.Handle))
        Else
            Me.listener.UnRegisterChannel("UserMessage")
            XDBroadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for UserMessage.", MyBase.Handle))
        End If
    End Sub

End Class
