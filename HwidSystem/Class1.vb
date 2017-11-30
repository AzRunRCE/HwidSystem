Imports System.Net, System.IO, System.Management, System, System.Security.Cryptography, System.Text, System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Page

Public Class Hwid
    Private PVerison As String = ""
    Private URL As String = ""
    Public Sub New(ByVal SystemURL As String, ByVal version As String)
        URL = SystemURL
        PVerison = version
    End Sub

    Public Function CheckLogin(ByVal Username As String, ByVal Password As String) As Integer
        Dim Answer1, Answer2, Answer3 As String
        Username = Username.ToLower()
        Password = GetMd5Hash(MD5.Create, Password)
        Using md5Hash As MD5 = MD5.Create()
            Dim Hwid As String = GetID()
            Answer1 = GetMd5Hash(md5Hash, Hwid & " Not Banned " & Username & " " & Password & " 0")
            Answer2 = GetMd5Hash(md5Hash, Hwid & " Banned " & Username & " " & Password & " 1")
            Answer3 = GetMd5Hash(md5Hash, Hwid & " Not On System " & Username & " " & Password)
        End Using
        Dim GET_Data As String = URL & "/api.php?Action=Login&usr=" & Username & "&pas=" & Password & "&hwid=" & GetID()
        Try
            Dim WebReq As HttpWebRequest = HttpWebRequest.Create(GET_Data)
            WebReq.Proxy = Nothing
            Using WebRes As HttpWebResponse = WebReq.GetResponse
                Using Reader As New StreamReader(WebRes.GetResponseStream)
                    Dim Str As String = Reader.ReadLine
                    Select Case True
                        Case Str.Contains(Answer1)
                            Return 0
                        Case Str.Contains(Answer2)
                            Return 1
                        Case Str.Contains(Answer3)
                            Return 2
                        Case Else
                            Return 3
                    End Select
                End Using
            End Using
        Catch Ex As Exception
            MsgBox("Unable to contact server, Please try again later!", MsgBoxStyle.Exclamation, "Error")
            Return "Invalid"
        End Try
    End Function
    Public Sub RegisterUser(ByVal Username As String, ByVal Password As String, ByVal Code As String)
        Username = Username.ToLower()
        Dim WebReq As HttpWebRequest = HttpWebRequest.Create(URL & "/api.php?Action=Register&code=" & Code & "&usr=" & Username & "&pas=" & GetMd5Hash(MD5.Create, Password) & "&hwid=" & GetID())
        WebReq.Proxy = Nothing
        Using WebRes As WebResponse = WebReq.GetResponse
            Using Reader As New StreamReader(WebRes.GetResponseStream)
                Dim Stream As String = Reader.ReadToEnd
            End Using
        End Using
        Select Case CheckLogin(Username, Password)
            Case 0
                MsgBox("Your Account has been created!", MsgBoxStyle.Information, "Created!")
                'TextBox1.Text = Username
                'TextBox2.Text = Password
            Case 1
                MsgBox("You Have Been Banned", MsgBoxStyle.Critical, "Banned")
            Case 2
                MsgBox("Invalid Code", MsgBoxStyle.Critical, "Error")
            Case Else
                MsgBox("An Error has occurred, Please try again later", MsgBoxStyle.Exclamation, "Error")
        End Select
    End Sub
    Public Function GetMOTD() As String
        Try
            Using web As New WebClient
                web.Proxy = Nothing
                Return web.DownloadString(URL & "/Infomation.php?Action=Motd")
            End Using
        Catch ex As Exception
            Return "Unable to contact server, Please try again later!"
        End Try

    End Function
    Public Sub ChechforUpdate()
        Using Web As New WebClient
            Web.Proxy = Nothing
            Dim Version As String = Web.DownloadString(URL & "/Infomation.php?Action=Version")
            If Version = PVerison Then
                GetMOTD()
            Else
                Dim Link As String = Web.DownloadString(URL & "/Infomation.php?Action=Link")
                MsgBox("Update Found", MsgBoxStyle.Information, "Update")

                Dim down As New Downloader
                down.DownloadLink = Link
                down.Show()
                While (down.IsHandleCreated = True)
                    Application.DoEvents()
                End While
            End If
        End Using
    End Sub
    Public Sub Changepass(ByVal Username As String, ByVal Password As String, ByVal NewPass As String)
        Username = Username.ToLower()
        Dim WebReq As HttpWebRequest = HttpWebRequest.Create(URL & "/api.php?Action=ChangePass&usr=" & Username & "&pas=" & Password & "&hwid=" & GetID() & "&npas=" & NewPass)
        WebReq.Proxy = Nothing
        Using WebRes As WebResponse = DirectCast(WebReq.GetResponse, HttpWebResponse)

            Using Reader As New StreamReader(WebRes.GetResponseStream)
                Dim Stream As String = Reader.ReadToEnd
            End Using
        End Using
    End Sub
    Public Function GetID() As String
        Dim HWID As String = SystemSerialNumber() & GetCPUID()
        If HWID.Contains(" ") Then HWID = HWID.Replace(" ", "")
        Return Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes(HWID))
    End Function
    Private Function GetCPUID()
        Dim cpuInfo As String = String.Empty
        Dim mc As New ManagementClass("win32_processor")
        Dim moc As ManagementObjectCollection = mc.GetInstances()
        For Each mo As ManagementObject In moc
            cpuInfo = mo.Properties("processorID").Value.ToString()
        Next
        Return cpuInfo
    End Function
    Private Function SystemSerialNumber() As String
        Try
            Dim wmi As Object = GetObject("WinMgmts:")
            Dim serial_numbers As String = String.Empty
            Dim mother_boards As Object = wmi.InstancesOf("Win32_BaseBoard")
            For Each board As Object In mother_boards
                serial_numbers &= board.SerialNumber
            Next board
            Return serial_numbers
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function
    Public Function GetMd5Hash(ByVal md5Hash As MD5, ByVal input As String) As String
        Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input))
        Dim sBuilder As New StringBuilder()
        Dim i As Integer
        For i = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("x2"))
        Next i
        Return sBuilder.ToString()
    End Function


End Class
