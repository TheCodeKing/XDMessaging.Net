'=============================================================================
'*
'*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
'*
'*   http://www.TheCodeKing.co.uk
'*  
'*	All rights reserved.
'*	The code and information is provided "as-is" without waranty of any kind,
'*	either expresed or implied. Please do not use commerically without permission.
'*
'*-----------------------------------------------------------------------------
'*	History:
'*		11/02/2007	Michael Carlisle				Version 1.0
'*		12/12/2009	Michael Carlisle				Version 2.0
'*=============================================================================
'

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports TheCodeKing.Net.Messaging

Namespace TheCodeKing.Demo
    ''' <summary>
    ''' A demo messaging application which demostrates the cross AppDomain Messaging API.
    ''' This independent instances of the application to receive and send messages between
    ''' each other.
    ''' </summary>
    Partial Public Class Messenger
        Inherits Form
        ''' <summary>
        ''' The instance used to listen to broadcast messages.
        ''' </summary>
        Private listener As IXDListener

        ''' <summary>
        ''' The instance used to broadcast messages on a particular channel.
        ''' </summary>
        Private broadcast As IXDBroadcast

        ''' <summary>
        ''' Delegate used for invoke callback.
        ''' </summary>
        ''' <param name="dataGram"></param>
        ''' <remarks></remarks>
        Private Delegate Sub UpdateDisplay(ByVal dataGram As DataGram)

        ''' <summary>
        ''' Default constructor.
        ''' </summary>
        Public Sub New()
            InitializeComponent()
        End Sub
        ''' <summary>
        ''' The onload event which initializes the messaging API by registering
        ''' for the Status and Message channels. This also assigns a delegate for
        ''' processing messages received. 
        ''' </summary>
        ''' <param name="e"></param>
        Protected Overloads Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)

            UpdateDisplayText("Launch multiple instances of this application to demo interprocess communication." & vbCr & vbLf, Color.Gray)

            ' set the handle id in the form title
            Me.Text += String.Format(" - Window {0}", Me.Handle)

            InitializeMode(XDTransportMode.WindowsMessaging)

            ' broadcast on the status channel that we have loaded
            broadcast.SendToChannel("Status", String.Format("Window {0} created!", Me.Handle))
        End Sub

        ''' <summary>
        ''' The closing overrride used to broadcast on the status channel that the window is
        ''' closing.
        ''' </summary>
        ''' <param name="e"></param>
        Protected Overloads Overrides Sub OnClosing(ByVal e As CancelEventArgs)
            MyBase.OnClosing(e)
            broadcast.SendToChannel("Status", String.Format("Window {0} closing!", Me.Handle))
        End Sub
        ''' <summary>
        ''' The delegate which processes all cross AppDomain messages and writes them to screen.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
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

        ''' <summary>
        ''' A helper method used to update the Windows Form.
        ''' </summary>
        ''' <param name="dataGram">dataGram</param>
        Private Sub UpdateDisplayText(ByVal dataGram As DataGram)
            Dim textColor As Color
            Select Case dataGram.Channel.ToLower()
                Case "status"
                    textColor = Color.Green
                    Exit Select
                Case Else
                    textColor = Color.Blue
                    Exit Select
            End Select
            Dim msg As String = String.Format("{0}: {1}" & vbCr & vbLf, dataGram.Channel, dataGram.Message)
            UpdateDisplayText(msg, textColor)
        End Sub

        ''' <summary>
        ''' A helper method used to update the Windows Form.
        ''' </summary>
        ''' <param name="message">The message to write to the display</param>
        ''' <param name="textColor">The colour of the text to display</param>
        Private Sub UpdateDisplayText(ByVal message As String, ByVal textColor As Color)
            Me.displayTextBox.AppendText(message)
            Me.displayTextBox.[Select](Me.displayTextBox.Text.Length - message.Length + 1, Me.displayTextBox.Text.Length)
            Me.displayTextBox.SelectionColor = textColor
            Me.displayTextBox.[Select](Me.displayTextBox.Text.Length, Me.displayTextBox.Text.Length)
            Me.displayTextBox.ScrollToCaret()
        End Sub

        ''' <summary>
        ''' Sends a user input string on the Message channel. A message is not sent if
        ''' the string is empty.
        ''' </summary>
        ''' <param name="sender">The event sender.</param>
        ''' <param name="e">The event args.</param>
        Private Sub sendBtn_Click(ByVal sender As Object, ByVal e As EventArgs)
            SendMessage()
        End Sub

        ''' <summary>
        ''' Wire up the enter key to submit a message.
        ''' </summary>
        ''' <param name="m"></param>
        ''' <param name="k"></param>
        ''' <returns></returns>
        Protected Overloads Overrides Function ProcessCmdKey(ByRef m As Message, ByVal k As Keys) As Boolean
            ' allow enter to send message
            If m.Msg = 256 AndAlso k = Keys.Enter Then
                SendMessage()
                Return True
            End If
            Return MyBase.ProcessCmdKey(m, k)
        End Function

        ''' <summary>
        ''' Helper method for sending message.
        ''' </summary>
        Private Sub SendMessage()
            If Me.inputTextBox.Text.Length > 0 Then
                broadcast.SendToChannel("UserMessage", String.Format("{0}: {1}", Me.Handle, Me.inputTextBox.Text))
                Me.inputTextBox.Text = ""
            End If
        End Sub

        ''' <summary>
        ''' Adds or removes the Message channel from the messaging API. This effects whether messages 
        ''' sent on this channel will be received by the application. Status messages are broadcast 
        ''' on the Status channel whenever this setting is changed. 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub msgCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If msgCheckBox.Checked Then
                listener.RegisterChannel("UserMessage")
                broadcast.SendToChannel("Status", String.Format("{0}: Registering for UserMessage.", Me.Handle))
            Else
                listener.UnRegisterChannel("UserMessage")
                broadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for UserMessage.", Me.Handle))
            End If
        End Sub

        ''' <summary>
        ''' Adds or removes the Status channel from the messaging API. This effects whether messages 
        ''' sent on this channel will be received by the application. Status messages are broadcast 
        ''' on the Status channel whenever this setting is changed. 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub statusCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If statusCheckBox.Checked Then
                listener.RegisterChannel("Status")
                broadcast.SendToChannel("Status", String.Format("{0}: Registering for Status.", Me.Handle))
            Else
                listener.UnRegisterChannel("Status")
                broadcast.SendToChannel("Status", String.Format("{0}: UnRegistering for Status.", Me.Handle))
            End If
        End Sub
        ''' <summary>
        ''' Initialize the broadcast and listener mode.
        ''' </summary>
        ''' <param name="mode">The new mode.</param>
        Private Sub InitializeMode(ByVal mode As XDTransportMode)
            InitializeMode(mode, True)
        End Sub

        Private Sub InitializeMode(ByVal mode As XDTransportMode, ByVal notify As Boolean)
            If listener IsNot Nothing Then
                ' ensure we dispose any previous listeners, dispose should aways be
                ' called on IDisposable objects when we are done with it to avoid leaks
                listener.Dispose()
            End If

            ' creates an instance of the IXDListener object using the given implementation  
            listener = XDListener.CreateListener(mode)

            ' attach the message handler
            AddHandler listener.MessageReceived, AddressOf OnMessageReceived

            ' register the channels we want to listen on
            If statusCheckBox.Checked Then
                listener.RegisterChannel("Status")
            End If
            If msgCheckBox.Checked Then
                listener.RegisterChannel("UserMessage")
            End If

            If broadcast IsNot Nothing Then
                broadcast.SendToChannel("Status", String.Format("{0}: Mode changing to {1}", Me.Handle, mode))
            End If

            ' create an instance of IXDBroadcast using the given mode, 
            ' note IXDBroadcast does not implement IDisposable
            broadcast = XDBroadcast.CreateBroadcast(mode)
        End Sub
        ''' <summary>
        ''' On form changed mode.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub mode_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If wmRadio.Checked Then
                InitializeMode(XDTransportMode.WindowsMessaging)
            Else
                InitializeMode(XDTransportMode.IOStream)
            End If
        End Sub
    End Class
End Namespace
