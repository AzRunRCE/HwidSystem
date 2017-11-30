Imports System.IO
Imports System.Windows.Forms

Public Class Downloader
    Dim FName As String
    Public DownloadLink As String
    Private Sub Downloader_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    End Sub
    Private Sub Downloader_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Using Save As New SaveFileDialog
            Save.FileName = "update.zip"
            If Save.ShowDialog = Windows.Forms.DialogResult.OK Then
                FName = Save.FileName
                Label1.Text = FName
                If DownloadLink.Contains("<!") Then
                    DownloadLink = DownloadLink.Replace("<!", "")
                End If
                BackgroundWorker1.RunWorkerAsync()
            Else
                MsgBox("You must download the latest version!", MsgBoxStyle.Exclamation, "Download")
            End If
        End Using
    End Sub
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If True Then
            Dim sUrlToReadFileFrom As String = DownloadLink
            Dim sFilePathToWriteFileTo As String = FName
            Dim url As New Uri(sUrlToReadFileFrom)
            Dim request As System.Net.HttpWebRequest = DirectCast(System.Net.WebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As System.Net.HttpWebResponse = DirectCast(request.GetResponse(), System.Net.HttpWebResponse)
            response.Close()
            Dim iSize As Int64 = response.ContentLength
            Dim iRunningByteTotal As Int64 = 0
            Using client As New System.Net.WebClient()
                Using streamRemote As System.IO.Stream = client.OpenRead(New Uri(sUrlToReadFileFrom))
                    Using streamLocal As Stream = New FileStream(sFilePathToWriteFileTo, FileMode.Create, FileAccess.Write, FileShare.None)
                        Dim iByteSize As Integer = 0
                        Dim byteBuffer As Byte() = New Byte(iSize - 1) {}
                        While (InlineAssignHelper(iByteSize, streamRemote.Read(byteBuffer, 0, byteBuffer.Length))) > 0
                            streamLocal.Write(byteBuffer, 0, iByteSize)
                            iRunningByteTotal += iByteSize
                            Dim dIndex As Double = CDbl(iRunningByteTotal)
                            Dim dTotal As Double = CDbl(byteBuffer.Length)
                            Dim dProgressPercentage As Double = (dIndex / dTotal)
                            Dim iProgressPercentage As Integer = CInt(Math.Truncate(dProgressPercentage * 100))
                            BackgroundWorker1.ReportProgress(iProgressPercentage)
                        End While
                        streamLocal.Close()
                    End Using
                    streamRemote.Close()
                End Using
            End Using
        End If
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MsgBox("File download to : " & FName, MsgBoxStyle.Information, "Finished")
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class