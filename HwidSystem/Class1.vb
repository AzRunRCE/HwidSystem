Imports System.Net, System.IO, System.Management, System, System.Security.Cryptography, System.Text, System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Page

Public Class HwidSystem
    Private PVerison As String = ""
    Private URL As String = ""
    Public Sub New(ByVal SystemURL As String, ByVal version As String)
        URL = SystemURL
        PVerison = version
    End Sub

    Public Function CheckLogin(ByVal Username As String, ByVal Password As String) As Integer
        Dim Answer1 As String
        Username = Username.ToLower()
        Password = getSHA1Hash(Password)
        Dim Hwid As String = GetID()
        Answer1 = getSHA1Hash(Hwid & "Not Banned" & Username & Password)
        Dim GET_Data As String = URL & "/api.php?action=login&usr=" & Username & "&pas=" & Password & "&hwid=" & Hwid
        Try
            Dim WebReq As HttpWebRequest = HttpWebRequest.Create(GET_Data)
            WebReq.Proxy = Nothing
            Using WebRes As HttpWebResponse = WebReq.GetResponse
                Using Reader As New StreamReader(WebRes.GetResponseStream)
                    Dim Str As String = Reader.ReadToEnd()
                    If (Str.Contains(Answer1)) Then
                        Return 1
                    Else
                        Return 0
                    End If
                End Using
            End Using
        Catch Ex As Exception
            MsgBox("Unable to contact server, Please try again later!", MsgBoxStyle.Exclamation, "Error")
            Return 0
        End Try
    End Function
    Public Function RegisterUser(ByVal Username As String, ByVal Password As String, ByVal Code As String) As Integer
        Username = Username.ToLower()
        Dim WebReq As HttpWebRequest = HttpWebRequest.Create(URL & "/api.php?action=register&code=" & Code & "&usr=" & Username & "&pas=" & getSHA1Hash(Password) & "&hwid=" & GetID())
        WebReq.Proxy = Nothing
        Using WebRes As WebResponse = WebReq.GetResponse
            Using Reader As New StreamReader(WebRes.GetResponseStream)
                Dim Stream As String = Reader.ReadToEnd
            End Using
        End Using
        Return CheckLogin(Username, Password)
    End Function
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
    Function getSHA1Hash(ByVal strToHash As String) As String

        Dim sha1Obj As New Security.Cryptography.SHA1CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

        bytesToHash = sha1Obj.ComputeHash(bytesToHash)

        Dim strResult As String = ""

        For Each b As Byte In bytesToHash
            strResult += b.ToString("x2")
        Next

        Return strResult

    End Function


End Class
