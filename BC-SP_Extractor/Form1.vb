Imports System.IO
Imports Ionic.Zip

Public Class Form1
    Private Sub btnExtract_Click(sender As Object, e As EventArgs) Handles btnExtract.Click
        Dim spfile = ""
        If ofdSP.ShowDialog = DialogResult.OK Then
            spfile = ofdSP.FileName
        Else
            Exit Sub
        End If

        Dim Offsets As New Dictionary(Of UInteger, UInteger) From {
            {0, 56320}, 'MenuJAR, Requires Compression and data.bin
            {56320, 71680},
            {128000, 92160},
            {220160, 112640},
            {332800, 48128},
            {380928, 8192},
            {389120, 8192},
            {397312, 8192},
            {405504, 2048},
            {407552, 2048}
            }

        'SetupDir
        If Directory.Exists("tmp") = False Then
            Directory.CreateDirectory("tmp")
        End If
        If Directory.Exists("built") = False Then
            Directory.CreateDirectory("built")
        End If


        'Start Getting Files
        Using br As BinaryReader = New BinaryReader(File.Open(spfile, FileMode.Open))
            For Each O In Offsets
                Dim tmpExtractName = $"tmp/{O.Key}.bin"
                br.BaseStream.Position = O.Key
                Dim Filedata = br.ReadBytes(O.Value)
                'Take File and Modify it
                If O.Key = 0 Then
                    File.WriteAllBytes(tmpExtractName, Filedata)
                    ExtractZIP(tmpExtractName)
                    ExportFile(Filedata, True)
                End If
            Next
        End Using
    End Sub


    Function ExportFile(inputbytes As Byte(), Compression As Boolean)
        'If file requires compression, we must first decompress it, then recompress it using a new standard.
        If Compression = True Then

        End If
    End Function


    Function ExtractZIP(ImportFile)
        Using zip1 As ZipFile = ZipFile.Read(ImportFile)
            Dim e As ZipEntry
            Dim ExtractCount = 0
            Dim ExtractFilename As list(of string)
            For Each e In zip1
                e.Extract("tmp", ExtractExistingFileAction.OverwriteSilently)
                ExtractCount +=1
            Next
            If ExtractCount = 1 
        End Using
    End Function
End Class
