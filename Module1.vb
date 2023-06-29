
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml

Module Module1

    Sub Main()
        Dim sql As New SQL()
        sql.ExecuteStoredProcedure()
    End Sub

End Module


Public Class XMLX

    Public Shared Function GetSingleValue(xpath) As String
        Dim doc As New XmlDocument()
        doc.Load($"{My.Application.Info.DirectoryPath}\Settings.xml")
        Return doc.SelectSingleNode(xpath).InnerText
    End Function

End Class

Public Class Logger
    Shared filepath As String = $"{My.Application.Info.DirectoryPath}\Logs\Log_{Date.UtcNow.ToString("yyyyMMdd")}.log"
    Shared file As New FileInfo(filepath)
    Public Shared Sub WriteLine(str As String)
        If Not file.Exists() Then
            file.Create().Close()
        End If

        Dim datetime = Date.Now.ToString("ddMMyyyy hh:mm:ss tt")
        Dim sw As StreamWriter = file.AppendText()
        Console.WriteLine(str)
        sw.WriteLine(datetime & " : " & str)
        sw.Close()

    End Sub

    Public Shared Sub WriteLine(ex As Exception)
        If Not file.Exists() Then
            file.Create().Close()
        End If

        Dim datetime = Date.Now.ToString("ddMMyyyy hh:mm:ss tt")
        Dim sw As StreamWriter = file.AppendText()
        Console.WriteLine(ex.ToString & " - " & ex.Message)
        sw.WriteLine(datetime & " : " & ex.ToString & " - " & ex.Message)
        sw.Close()

    End Sub
End Class

Public Class SQL

    Dim sqlConnStr As String
    Dim sqlCommand As SqlCommand
    Dim sqlDataReader As SqlDataReader
    Dim sqlConnection As SqlConnection

    Public Sub New()
        sqlConnStr =
            $"
                server={XMLX.GetSingleValue("//database/serverIP")};
                data source={XMLX.GetSingleValue("//database/dataSource")};
                initial catalog={XMLX.GetSingleValue("//database/initialCatalog")};
                user id={XMLX.GetSingleValue("//database/dbUserID")};
                password={XMLX.GetSingleValue("//database/dbPassword")};"
        sqlConnection = New SqlConnection(sqlConnStr)
    End Sub

    Public Sub ExecuteStoredProcedure()

        Dim query = XMLX.GetSingleValue("//database/query")

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection) With {
                .CommandTimeout = 600
            }
            sqlDataReader = sqlCommand.ExecuteReader()
            While sqlDataReader.Read

            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex)
        End Try
    End Sub

End Class


