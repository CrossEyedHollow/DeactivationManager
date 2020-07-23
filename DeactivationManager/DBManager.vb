Imports ReportTools

Public Class DBManager
    Inherits DBBase

    Public Sub New()
        Init()
    End Sub

#Region "Functions"
    Public Function CheckForExpired() As DataTable
        Dim query As String
        Select Case Main.WorkMode
            Case Enums.WorkMode.Facility
                query = $"SELECT * FROM `{DBName}`.`tblprimarycodes` WHERE fldDeactivate = 1 AND fldPrintDate IS NULL AND fldCreatedDate > NOW() - INTERVAL 6 MONTH;"
            Case Enums.WorkMode.Primary
                query = $"SELECT * FROM `{DBName}`.`tblprimarycodes` WHERE fldIDA IS NULL AND fldPrintDate IS NULL AND fldIssueDate > NOW() - INTERVAL 6 MONTH;"
            Case Else
                Throw New Exception("Should be unreachable")
        End Select
        Return ReadDatabase(query)
    End Function

    Public Sub DeactivateCodes(codes As String())
        Dim query As String = $"UPDATE `{DBName}`.`tblprimarycodes` SET fldDeactivated = 2 WHERE fldCode in ('{String.Join("','", codes)}')"
        Execute(query)
    End Sub

    Public Function InsertRawJson(table As String, Json As String, type As String) As Boolean
        'Generate the query 
        Dim query As String = $"INSERT INTO `{DBName}`.`{table}` (fldJson, fldType) VALUES ('{Json}', '{type}');"
        'Execute it
        Return Execute(query)
    End Function
#End Region

End Class
