Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json.Linq

Module Extentions
    <Extension()>
    Public Function IsNullOrEmpty(ByVal array() As String) As Boolean
        Return IsDBNull(array) OrElse array Is Nothing OrElse (array.Length < 1)
    End Function

    <Extension()>
    Public Function IsNullOrEmpty(ByVal array() As Integer) As Boolean
        Return IsDBNull(array) OrElse array Is Nothing OrElse (array.Length < 1)
    End Function

    <Extension()>
    Public Function IsNullOrEmpty(ByVal array() As Decimal) As Boolean
        Return IsDBNull(array) OrElse array Is Nothing OrElse (array.Length < 1)
    End Function

    <Extension()>
    Public Function IsNullOrEmpty(ByVal str As String) As Boolean
        Return String.IsNullOrEmpty(str) OrElse str Is Nothing
    End Function

    <Extension()>
    Public Function ColumnToArray(ByVal str As DataTable, columnName As String) As String()
        Return str.Rows.OfType(Of DataRow).Select(Function(dr) dr.Field(Of String)(columnName)).ToArray()
    End Function

    <Extension()>
    Public Function ToJArray(ByRef array As Array) As JArray
        If IsDBNull(array) Then Return Nothing
        If array Is Nothing Then Return Nothing
        If array.Length = 0 Then Return Nothing

        Return JArray.FromObject(array)
    End Function
End Module
