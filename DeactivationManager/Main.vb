Imports ReportTools

Module Main
    Public Property Settings As DataSet
    Public Property WorkMode As WorkMode

    Dim db As DBManager

    Sub Main()
        Try
            Initialize()
        Catch ex As Exception
            Output.Report($"Failed Intitialization, reason: {ex.Message}")
            Exit Sub
        End Try

        While True
            Try
                'Check for expired codes
                Dim result = db.CheckForExpired()
                'If any
                If result.Rows.Count > 0 Then

                    'Get the codes
                    Dim expiredCodes As String() = result.ColumnToArray("fldCode")

                    'Deactivate depending on WorkMode
                    Select Case WorkMode
                        Case WorkMode.Facility

                            'Update the database
                            Output.ToConsole($"{result.Rows.Count} expired codes found deactivating.")
                            db.DeactivateCodes(expiredCodes)

                        Case WorkMode.Primary

                            If expiredCodes.Length > 5000 Then
                                'Take 5000 codes at a time
                                For index = 0 To expiredCodes.Length Step 5000
                                    'Create a copy of a part of the total codes
                                    Dim batchSize As Integer = If(expiredCodes.Length - index > 5000, 5000, expiredCodes.Length - index)
                                    Dim batch(batchSize - 1) As String
                                    Array.Copy(expiredCodes, index, batch, 0, batchSize)

                                    'Create a fake JSON and insert into database
                                    Dim recallCode As String = Guid.NewGuid().ToString()
                                    Dim jsonBody = JsonOperationals.IDA(Date.UtcNow, AggregationType.Unit_Packets_Only, DeactivationType.UI_unused, batch, Nothing, recallCode)

                                    Output.ToConsole($"Deactivating a batch of {batchSize} codes.")
                                    'UPDATE db 
                                    db.InsertRawJson("tbljson", jsonBody, "IDA", recallCode)
                                Next
                            Else
                                'Create a fake JSON and insert into database
                                Dim recallCode As String = Guid.NewGuid().ToString()
                                Dim jsonBody = JsonOperationals.IDA(Date.UtcNow, AggregationType.Unit_Packets_Only, DeactivationType.UI_unused, expiredCodes, Nothing, recallCode)

                                Output.ToConsole($"Deactivating a batch of {expiredCodes.Length} codes.")

                                'UPDATE db 
                                db.InsertRawJson("tbljson", jsonBody, "IDA", recallCode)
                            End If



                        Case Else
                            Throw New Exception("Invalid WorkMode value, please check the Settings.xml file")
                    End Select
                End If
                'SLeep to conserve cpu
                Threading.Thread.Sleep(TimeSpan.FromHours(6))
            Catch ex As Exception
                Output.Report($"Unexpected exception occured: {ex.Message}")
                Threading.Thread.Sleep(TimeSpan.FromSeconds(30))
            End Try
        End While
    End Sub

    Private Sub Initialize()
        Settings = New DataSet()
        Settings.ReadXml($"{AppDomain.CurrentDomain.BaseDirectory}Settings.xml")

        'Initialize the DBManager objects
        Dim dbSetting As DataRow = Settings.Tables("tblDBSettings").Rows(0)
        DBBase.DBName = dbSetting("fldDBName")
        DBBase.DBIP = dbSetting("fldServer")
        DBBase.DBUser = dbSetting("fldAccount")
        DBBase.DBPass = dbSetting("fldPassword")
        db = New DBManager()

        'Init the general settings
        Dim generalSettings As DataRow = Settings.Tables("tblGeneral").Rows(0)
        Dim eoID = generalSettings("fldEO_ID")
        Dim fID = generalSettings("fldF_ID")

        WorkMode = CInt(generalSettings("fldWorkMode"))
        JsonOperationals.EO_ID = eoID
        JsonOperationals.F_ID = fID
    End Sub
End Module
