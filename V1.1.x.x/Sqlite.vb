Imports System.Data.SQLite
Imports System.Data

'- original Class made by Kati Maya on http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/page__view__findpost__p__1930275

Class Sqlite
    Private db As String
    Private bdebug As Boolean = False

    Public Sub New(DB_Name As String, Optional nDebug As Boolean = False)
        If DB_Name.IndexOf("Data Source=") = -1 Then
            MsgBox("Check your db_name, no data source provided", MsgBoxStyle.Critical, "SQLite class")
            Exit Sub
        End If

        db = DB_Name
#If DEBUG Or nDebug Then
        bdebug = True
#End If
    End Sub

    'grab a single value from command
    Public Function GetValue(sql As SQLiteCommand) As Object
        If bdebug Then SqlString(sql)
        Dim value As Object
        Using conn As SQLiteConnection = New SQLiteConnection(db)
            conn.OpenAsync()
            Using trans As SQLiteTransaction = conn.BeginTransaction
                sql.Connection = conn
                value = sql.ExecuteScalar()
                trans.Commit()
            End Using

        End Using
        If value IsNot Nothing Then
            If bdebug Then Debug.WriteLine("> Response: " & value.ToString)
            GetValue = value
        Else
            GetValue = False
        End If
        value = Nothing
    End Function

    'grab a single value from string query
    Public Function GetValue(sql As String) As Object
        If bdebug Then Debug.WriteLine(sql)
        Dim value As Object
        Using conn As SQLiteConnection = New SQLiteConnection(db)
            conn.OpenAsync()
            Try
                Using trans As SQLiteTransaction = conn.BeginTransaction
                    Using cmd As New SQLiteCommand(conn)
                        cmd.CommandText = sql
                        value = cmd.ExecuteScalar()
                        trans.Commit()
                    End Using
                End Using
            Catch e As Exception
                Throw New Exception(e.Message)
            End Try

        End Using

        If value IsNot Nothing Then
            If bdebug Then Debug.WriteLine("> Response: " & value.ToString)
            GetValue = value
        Else
            GetValue = False
        End If
        value = Nothing
    End Function

    'returns a DataTable from a command
    Public Function GetData(sql As SQLiteCommand) As DataTable
        If bdebug Then SqlString(sql)
        Using conn As SQLiteConnection = New SQLiteConnection(db)
            conn.OpenAsync()
            Using trans As SQLiteTransaction = conn.BeginTransaction
                Try
                    GetData = New DataTable()
                    sql.Connection = conn
                    Using reader As SQLiteDataReader = sql.ExecuteReader()
                        GetData.Load(reader)
                    End Using
                    trans.Commit()
                Catch e As Exception
                    Throw New Exception(e.Message)
                End Try
            End Using
        End Using
    End Function

    'returns a DataTable from a string query
    Public Function GetData(sql As String) As DataTable
        If bdebug Then Debug.WriteLine(sql)
        Using conn As SQLiteConnection = New SQLiteConnection(db)
            conn.OpenAsync()
            Using trans As SQLiteTransaction = conn.BeginTransaction
                Try
                    GetData = New DataTable()
                    Using cmd As New SQLiteCommand(conn)
                        cmd.CommandText = sql
                        Using reader As SQLiteDataReader = cmd.ExecuteReader()
                            GetData.Load(reader)
                        End Using
                        trans.Commit()
                    End Using
                Catch e As Exception
                    Throw New Exception(e.Message)
                End Try
            End Using
        End Using
    End Function

    'executes sql statement returning # of rows affected
    Public Function Execute(sql As String) As Integer
        Execute = -1
        Try
            Using conn As SQLiteConnection = New SQLiteConnection(db)
                conn.OpenAsync()
                Using trans As SQLiteTransaction = conn.BeginTransaction
                    Using cmd As New SQLiteCommand(conn)
                        cmd.CommandText = sql
                        Execute = cmd.ExecuteNonQuery()
                    End Using
                    trans.Commit()
                End Using
            End Using
        Catch e As Exception
            Throw New Exception(e.Message)
        End Try

        If bdebug Then
            Debug.WriteLine(sql)
            Debug.WriteLine(String.Format("Rows Updated: {0}", Execute))
        End If
    End Function

    'executes sql statement returning # of rows affected
    Public Function Execute(sql As SQLiteCommand) As Integer
        Execute = -1
        Try
            Using conn As SQLiteConnection = New SQLiteConnection(db)
                conn.OpenAsync()
                Using trans As SQLiteTransaction = conn.BeginTransaction
                    sql.Connection = conn
                    Execute = sql.ExecuteNonQuery()
                    If sql.CommandText.ToLower.IndexOf("insert") > -1 Then
                        sql.CommandText = "select last_insert_rowid();"
                        Execute = CInt(sql.ExecuteScalar)
                    End If
                    trans.Commit()
                End Using
            End Using
        Catch e As Exception
            Throw New Exception(e.Message)
        Finally
            If bdebug Then SqlString(sql, Execute)
        End Try
    End Function

    'return dictionary record
    Public Function GetRow(Table As String, Column As String, Where As String) As Dictionary(Of String, String)
        GetRow = New Dictionary(Of String, String)
        Try
            Using cmd As New SQLiteCommand("SELECT " & Column & " FROM " & Table & " WHERE " & Where & " LIMIT 1;") '"SELECT @column FROM @table WHERE @where LIMIT 1")
                'cmd.Parameters.AddWithValue("@table", Table)
                'cmd.Parameters.AddWithValue("@column", Column)
                'cmd.Parameters.AddWithValue("@where", Where)

                Using conn As SQLiteConnection = New SQLiteConnection(db)
                    conn.OpenAsync()
                    Using trans As SQLiteTransaction = conn.BeginTransaction
                        cmd.Connection = conn
                        Using reader As SQLiteDataReader = cmd.ExecuteReader(CommandBehavior.SingleRow)
                            While reader.Read()
                                For i = 0 To reader.FieldCount - 1
                                    GetRow.Add(reader.GetName(i), reader.GetString(i).ToString)
                                Next
                            End While
                        End Using
                        trans.Commit()
                    End Using
                End Using
                cmd.Parameters.Clear()
            End Using
        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    Public Function GetRow(SQL As String) As Dictionary(Of String, String)
        GetRow = New Dictionary(Of String, String)
        Try
            Using cmd As New SQLiteCommand
                cmd.CommandText = SQL
                Using conn As SQLiteConnection = New SQLiteConnection(db)
                    conn.OpenAsync()
                    Using trans As SQLiteTransaction = conn.BeginTransaction
                        cmd.Connection = conn
                        Using reader As SQLiteDataReader = cmd.ExecuteReader()
                            While reader.Read()
                                For i = 0 To reader.FieldCount - 1
                                    GetRow.Add(reader.GetName(i), reader.GetString(i).ToString)
                                Next
                            End While
                        End Using
                        trans.Commit()
                    End Using
                End Using
            End Using
        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    'insert data, returns last inserted id
    Public Function Insert(table As String, data As Dictionary(Of String, Object)) As Integer
        Insert = -1
        Try
            If data.Count >= 1 Then
                Using cmd As New SQLiteCommand("INSERT INTO " & table & " (@colums)VALUES(@values);")
                    Dim columns As String = ""
                    Dim values As String = ""
                    For Each val As KeyValuePair(Of String, Object) In data
                        columns &= val.Key & ","
                        values &= "@" & val.Key & ","
                        cmd.Parameters.AddWithValue("@" & val.Key, val.Value)
                    Next

                    columns = columns.Substring(0, columns.Length - 1)
                    values = values.Substring(0, values.Length - 1)

                    'cmd.Parameters.AddWithValue("@table", table)
                    'cmd.Parameters.AddWithValue("@colums", columns)
                    cmd.Parameters.AddWithValue("@values", values)
                    cmd.CommandText = cmd.CommandText.Replace("@colums", columns) '.Replace("@table", table)
                    columns = Nothing
                    values = Nothing

                    If bdebug Then SqlString(cmd)

                    Using conn As SQLiteConnection = New SQLiteConnection(db)
                        conn.OpenAsync()
                        Using trans As SQLiteTransaction = conn.BeginTransaction
                            cmd.Connection = conn
                            cmd.ExecuteNonQuery()
                            cmd.CommandText = "select last_insert_rowid();"
                            Insert = CInt(cmd.ExecuteScalar)
                            trans.Commit()
                        End Using
                    End Using
                    cmd.Parameters.Clear()
                End Using
            End If
        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    'insert or ignore data, returns rows updated
    Public Function InsertOrIgnore(table As String, data As Dictionary(Of String, Object)) As Integer
        InsertOrIgnore = -1
        Try
            If data.Count >= 1 Then
                Using cmd As New SQLiteCommand("insert or ignore into " & table & " (@columns) values(@values);")
                    Dim columns As String = ""
                    Dim values As String = ""
                    For Each val As KeyValuePair(Of String, Object) In data
                        columns &= val.Key & ","
                        values &= "@" & val.Key & ","
                        cmd.Parameters.AddWithValue("@" & val.Key, val.Value)
                    Next
                    columns = columns.Substring(0, columns.Length - 1)
                    values = values.Substring(0, values.Length - 1)

                    'cmd.Parameters.AddWithValue("@table", table)
                    'cmd.Parameters.AddWithValue("@columns", columns)
                    cmd.Parameters.AddWithValue("@values", values)
                    cmd.CommandText = cmd.CommandText.Replace("@colums", columns) '.Replace("@table", table)
                    columns = Nothing
                    values = Nothing

                    Using conn As SQLiteConnection = New SQLiteConnection(db)
                        conn.OpenAsync()
                        Using trans As SQLiteTransaction = conn.BeginTransaction
                            cmd.Connection = conn
                            InsertOrIgnore = Execute(cmd)
                            If bdebug Then SqlString(cmd, InsertOrIgnore)
                            trans.Commit()
                        End Using
                        ' conn.Close()
                    End Using
                    cmd.Parameters.Clear()
                End Using
            End If

        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    'insert or replace data, returns rows updated
    Public Function InsertOrUpdate(table As String, data As Dictionary(Of String, Object)) As Integer
        InsertOrUpdate = -1
        Try
            If data.Count >= 1 Then
                Using cmd As New SQLiteCommand("insert or replace into " & table & " (@colums) values(@values);")
                    Dim columns As String = ""
                    Dim values As String = ""
                    For Each val As KeyValuePair(Of String, Object) In data
                        columns &= val.Key & ","
                        values &= "@" & val.Key & ","
                        cmd.Parameters.AddWithValue("@" & val.Key, val.Value)
                    Next
                    columns = columns.Substring(0, columns.Length - 1)
                    values = values.Substring(0, values.Length - 1)
                    'cmd.Parameters.AddWithValue("@table", table)
                    'cmd.Parameters.AddWithValue("@colums", columns)
                    cmd.Parameters.AddWithValue("@value", values)
                    cmd.CommandText = cmd.CommandText.Replace("@colums", columns) '.Replace("@table", table)
                    columns = Nothing
                    values = Nothing

                    Using conn As SQLiteConnection = New SQLiteConnection(db)
                        conn.OpenAsync()
                        Using trans As SQLiteTransaction = conn.BeginTransaction
                            cmd.Connection = conn
                            InsertOrUpdate = Execute(cmd)
                            trans.Commit()
                        End Using
                        If bdebug Then SqlString(cmd, InsertOrUpdate)
                        cmd.Parameters.Clear()

                    End Using
                End Using
            End If

        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    'update a table, returns rows updated.  probably best to only use on simple integer primary key-based where clauses
    Public Function Update(table As String, data As Dictionary(Of String, Object), where As String) As Integer
        Update = -1
        Try
            If data.Count >= 1 Then
                Using cmd As New SQLiteCommand
                    Dim sql As String = "update  " & table & " set "
                    For Each val As KeyValuePair(Of String, Object) In data
                        sql &= val.Key & "=@" & val.Key & ","
                        cmd.Parameters.AddWithValue("@" & val.Key, val.Value)
                    Next
                    cmd.CommandText = sql.Substring(0, sql.Length - 1) & " where @where;"
                    cmd.Parameters.AddWithValue("@where", where)
                    sql = Nothing

                    If bdebug Then SqlString(cmd)
                    Using conn As SQLiteConnection = New SQLiteConnection(db)
                        conn.OpenAsync()
                        Using trans As SQLiteTransaction = conn.BeginTransaction
                            cmd.Connection = conn
                            Update = Execute(cmd)
                            trans.Commit()
                        End Using

                    End Using
                    cmd.Parameters.Clear()
                End Using
            End If
        Catch e As Exception
            Throw New Exception(e.Message)
        End Try
    End Function

    Public Function SqlString(ByVal cmd As SQLiteCommand, Optional ByVal rows As Integer = 0) As String
        Dim builder As New Text.StringBuilder(Environment.NewLine & "----------------")
        For Each item As SQLiteParameter In cmd.Parameters
            builder.Append(Environment.NewLine)
            builder.AppendFormat("{0} = [{1}] ({2})", item.ParameterName, item.Value, item.DbType)
        Next
        builder.AppendLine(Environment.NewLine)
        builder.AppendLine(cmd.CommandText)

        If rows > 0 Then
            builder.AppendFormat("Rows Updated: {0}", rows)
            builder.Append(Environment.NewLine)
        End If
        builder.AppendLine("----------------")
        Debug.WriteLine(builder.ToString)
        SqlString = builder.ToString
        builder = Nothing
    End Function
End Class