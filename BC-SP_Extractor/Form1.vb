Imports System.IO
Imports Ionic.Zip

Public Class Form1
    Dim BC_FFVII_GAME_Offsets As New Dictionary(Of UInteger, UInteger) From {'key=pos value=length
            {0, 56320},       '画像データ            fix/fix2
            {56320, 71680},   'マップデータ          map
            {128000, 92160},  'ミッションデータ       train/stage/stage_p
            {220160, 112640}, '敵キャラクターデータ   enemy/chara
            {332800, 48128},  'キャラクターデータ     ply
            {380928, 8192},   'マテリアデータ１       atk1
            {389120, 8192},   'マテリアデータ２       atk2
            {397312, 8192},   'マテリアデータ３       atk3
            {405504, 2048},   'Now accessing...     array1                    //This pos is never written to, only read from
            {407552, 2048}    'Now downloading...   array2                    //This pos is never written to, only read from
            }

    Dim BC_FFVII_EXTRA_Offsets As New Dictionary(Of UInteger, UInteger) From {'key=pos value=length
            {0, 71680},     '画像データ            btlfix
            {71680, 102400},'画像データ            img_btl/img_area
            {174080, 61440},'マップデータ          btlmap
            {235520, 51200},'ミッションデータ       btlstage
            {286720, 15360},'敵キャラクターデータ    btlchara/btlenemy
            {302080, 70656},'キャラクターデータ      btlplay
            {372736, 8192}, 'マテリアデータ1        atk1
            {380928, 8192}, 'マテリアデータ2        atk2
            {389120, 8192}, 'マテリアデータ3        atk3
            {397312, 8192}, 'マテリアデータ4        atk4
            {405504, 2048}, 'Now accessing...     array1                    //This pos is never written to, only read from
            {407552, 2048}  'Now downloading...   array2                    //This pos is never written to, only read from
            }

    Dim BC_FFVII_GS_Offsets As New Dictionary(Of UInteger, UInteger) From {'key=pos value=length
            {0, 51200},       'サウンドデータ     gs_fix
            {51200, 128000},  '画像データ         udezumou/superdunk/chocores
            {179200, 66560},  'マップデータ       chocomap
            {245760, 40960},  'キャラクターデータ  chocobo
            {286720, 102400}, 'アタックデータ     chocoatk
            {389120, 20480}   'Now accessing...                             //This pos is never written to, only read from
            }


    Private Sub btnExtract_Click(sender As Object, e As EventArgs) Handles btnExtract.Click
        Dim spfile = ""
        If ofdSP.ShowDialog = DialogResult.OK Then
            spfile = ofdSP.FileName
        Else
            Exit Sub
        End If

        'SetupDir
        If Directory.Exists("tmp") = False Then
            Directory.CreateDirectory("tmp")
        End If
        If Directory.Exists("built") = False Then
            Directory.CreateDirectory("built")
        End If


        'Start Getting Files
        Using br As BinaryReader = New BinaryReader(File.Open(spfile, FileMode.Open))
            For Each O In BC_FFVII_GAME_Offsets
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
            Dim ExtractFilename As List(Of String)
            For Each e In zip1
                e.Extract("tmp", ExtractExistingFileAction.OverwriteSilently)
                ExtractCount += 1
            Next
        End Using
    End Function
End Class
