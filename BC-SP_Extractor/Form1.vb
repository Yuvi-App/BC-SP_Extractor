Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.IO.Compression


Public Class Form1
    Dim JARFileLocation As String = ""
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
    Dim BC_FFVII_GAME_FileName As New Dictionary(Of UInteger, String) From {'key=pos value=FileName
            {0, "fix_fix2.jar"},
            {56320, "map.jar"},
            {128000, "train_stage_stage_p.jar"},
            {220160, "enemy_chara.jar"},
            {332800, "ply.jar"},
            {380928, "atk1.jar"},
            {389120, "atk2.jar"},
            {397312, "atk3.jar"},
            {405504, "savedata1.bin"},
            {407552, "savedata2.bin"}
            }
    Dim BC_FFVII_EXTRA_Offsets As New Dictionary(Of UInteger, UInteger) From {'key=pos value=length
            {0, 71680},     '画像データ            btlfix
            {71680, 102400},'画像データ            img_btl/img_area
            {174080, 61440},'マップデータ          btlmap
            {235520, 51200},'ミッションデータ       btlstage
            {286720, 15360},'敵キャラクターデータ    btlchara/btlenemy
            {302080, 70656},'キャラクターデータ      btlply
            {372736, 8192}, 'マテリアデータ1        atk1
            {380928, 8192}, 'マテリアデータ2        atk2
            {389120, 8192}, 'マテリアデータ3        atk3
            {397312, 8192}, 'マテリアデータ4        atk4
            {405504, 2048}, 'Now accessing...     array1                    //This pos is never written to, only read from
            {407552, 2048}  'Now downloading...   array2                    //This pos is never written to, only read from
            }
    Dim BC_FFVII_EXTRA_FileName As New Dictionary(Of UInteger, String) From {'key=pos value=Filename
            {0, "btlfix.jar"},
            {71680, "img_btl_img_area.jar"},
            {174080, "btlmap.jar"},
            {235520, "btlstage.jar"},
            {286720, "btlchara_btlenemy.jar"},
            {302080, "btlply.jar"},
            {372736, "atk1.jar"},
            {380928, "atk2.jar"},
            {389120, "atk3.jar"},
            {397312, "atk4.jar"},
            {405504, "savedata1.bin"},
            {407552, "savedata2.bin"}
            }
    Dim BC_FFVII_GS_Offsets As New Dictionary(Of UInteger, UInteger) From {'key=pos value=length
            {0, 51200},       'サウンドデータ     gs_fix
            {51200, 128000},  '画像データ         udezumou/superdunk/chocores
            {179200, 66560},  'マップデータ       chocomap
            {245760, 40960},  'キャラクターデータ  chocobo
            {286720, 102400}, 'アタックデータ     chocoatk
            {389120, 20480}   'Now accessing...                             //This pos is never written to, only read from
            }
    Dim BC_FFVII_GS_FileName As New Dictionary(Of UInteger, String) From {'key=pos value=filename
            {0, "gs_fix.jar"},
            {51200, "udezumou_superdunk_chocores.jar"},
            {179200, "chocomap.jar"},
            {245760, "chocobo.jar"},
            {286720, "chocoatk.jar"},
            {389120, "savedataarray1.bin"}
            }
    Dim FFVII_SB_Offsets As New Dictionary(Of UInteger, UInteger) From {
        {0, 129},
        {129, 4318},
        {4447, 4318},
        {8765, 4318}
        }
    Dim FFVII_SB_FileName As New Dictionary(Of UInteger, String) From {
        {0, "0.bin"},
        {129, "1.bin"},
        {4447, "2.bin"},
        {8765, "3.bin"}
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
            Directory.CreateDirectory("built/bcffvii")
            Directory.CreateDirectory("built/extra")
            Directory.CreateDirectory("built/goldensaucer")
            Directory.CreateDirectory("built/snowboarding")
        End If

        'Clear TMP
        For Each f In Directory.GetFiles("tmp")
            File.Delete(f)
        Next

        Dim SelectedSPType As Integer
        Select Case ComboBox1.SelectedIndex
            Case 0 'bcffvii
                StartExtractProcess(spfile, BC_FFVII_GAME_Offsets, BC_FFVII_GAME_FileName, "built/bcffvii")
            Case 1 'extra
                StartExtractProcess(spfile, BC_FFVII_EXTRA_Offsets, BC_FFVII_EXTRA_FileName, "built/extra")
            Case 2 'GS
                StartExtractProcess(spfile, BC_FFVII_GS_Offsets, BC_FFVII_GS_FileName, "built/goldensaucer")
            Case 3 'GS
                StartExtractProcess(spfile, FFVII_SB_Offsets, FFVII_SB_FileName, "built/snowboarding")
        End Select
    End Sub

    Function StartExtractProcess(SPFile As String, POSList As Dictionary(Of UInteger, UInteger), FileNameList As Dictionary(Of UInteger, String), ExportLOC As String)
        Dim ExtractedCompressedCount = 0
        Dim ExtractedUncomrpessedCount = 0
        Using br As BinaryReader = New BinaryReader(File.Open(SPFile, FileMode.Open))
            For Each O In POSList
                'Setup Names
                Dim tmpExtractName = $"tmp/{O.Key}.bin"
                Dim ExtractFileName = $"{FileNameList(O.Key)}"
                Dim ExtractFilePathName = $"{ExportLOC}/{FileNameList(O.Key)}"

                'Check Ensure its not already built
                If File.Exists(ExtractFilePathName) Then
                    File.Delete(ExtractFilePathName)
                End If

                'Get File Data
                br.BaseStream.Position = O.Key
                Dim Filedata = br.ReadBytes(O.Value)

                'Lets Check if its a JAR
                br.BaseStream.Position = O.Key
                If Encoding.UTF8.GetString(br.ReadBytes(2)) = "PK" Then 'Found Compressed
                    'SaveTMP
                    File.WriteAllBytes(tmpExtractName, Filedata)
                    Dim ExtractedFiles As List(Of String) = ExtractZIP(tmpExtractName, True)
                    If ExtractedFiles.Count > 0 Then
                        CreateJAR($"{JARFileLocation} -cMf {ExtractFileName} *", "tmp")
                        Thread.Sleep(100)
                        If File.Exists($"tmp/{ExtractFileName}") Then
                            File.Copy($"tmp/{ExtractFileName}", ExtractFilePathName)
                            File.Delete($"tmp/{ExtractFileName}")
                            For Each f In ExtractedFiles
                                File.Delete(f)
                            Next
                            ExtractedCompressedCount += 1
                        End If
                    End If
                Else 'No Compression
                    File.WriteAllBytes(ExtractFilePathName, Filedata)
                    ExtractedUncomrpessedCount += 1
                End If
            Next
        End Using
        MessageBox.Show($"Extracted and Created {vbNewLine}Compressed: {ExtractedCompressedCount}{vbNewLine}Uncompressed: {ExtractedUncomrpessedCount}")
    End Function
    Function ExtractZIP(ImportFile As String, DeleteFile As Boolean) As List(Of String)
        Dim ExtractCount = 0
        Dim ExtractedFiles As List(Of String) = New List(Of String)
        Using archive As ZipArchive = ZipFile.OpenRead(ImportFile)
            For Each entry As ZipArchiveEntry In archive.Entries
                Dim destinationPath As String = Path.Combine("tmp", entry.FullName)
                Dim directoryPath As String = Path.GetDirectoryName(destinationPath)
                If Not Directory.Exists(directoryPath) Then
                    Directory.CreateDirectory(directoryPath)
                End If
                entry.ExtractToFile(destinationPath, True)
                ExtractedFiles.Add(destinationPath)
                ExtractCount += 1
            Next
        End Using
        If DeleteFile = True Then
            File.Delete(ImportFile)
        End If

        Return ExtractedFiles
    End Function
    Function CreateJAR(InputCMD As String, WD As String)
        Dim cmd As String = InputCMD
        Dim workingDirectory As String = WD
        Dim psi As New ProcessStartInfo()
        psi.FileName = "cmd.exe"
        psi.Arguments = "/c " & cmd
        psi.WorkingDirectory = workingDirectory
        psi.RedirectStandardOutput = True
        psi.RedirectStandardError = True
        psi.UseShellExecute = False
        psi.CreateNoWindow = True
        Dim process As New Process()
        process.StartInfo = psi
        process.Start()
        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim [error] As String = process.StandardError.ReadToEnd()
        process.WaitForExit()
        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0

        If File.Exists("jar.exe") = False Then
            MessageBox.Show("Missing JAR.exe, Please located and put into root dir of this app and try again.")
            Application.Exit()
        Else
            JARFileLocation = Application.StartupPath() + "jar.exe"
        End If
    End Sub
    Private Sub ExtractFixJarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractFixJarToolStripMenuItem.Click
        If ofdSP.ShowDialog = DialogResult.OK Then
            Dim ExtractedFiles As List(Of String) = ExtractZIP(ofdSP.FileName, False)
            If ExtractedFiles.Count > 0 Then
                For Each f In ExtractedFiles
                    ExtractFIXJAR(f)
                    File.Delete(f)
                Next
            End If
        End If
    End Sub
    Function ExtractFIXJAR(inputDatabin)
        Using br As BinaryReader = New BinaryReader(File.Open(inputDatabin, FileMode.Open))
            Dim FileNameCount = 0
            Dim U1 = br.ReadUInt32
            Dim U2 = br.ReadUInt32
            Dim ID = br.ReadByte
            While br.BaseStream.Position < br.BaseStream.Length
                Dim FileLengthBA = br.ReadBytes(4)
                Array.Reverse(FileLengthBA)
                Dim FileLength = BitConverter.ToUInt32(FileLengthBA)
                If FileNameCount = 86 Then
                    br.BaseStream.Position = br.BaseStream.Position + 5
                    FileLengthBA = br.ReadBytes(4)
                    Array.Reverse(FileLengthBA)
                    FileLength = BitConverter.ToUInt32(FileLengthBA)
                End If

                Dim Filedata = br.ReadBytes(FileLength)
                Dim FileName = ""
                If FileNameCount > 88 Then
                    FileName = $"tmp/{FileNameCount}.bin"
                ElseIf FileNameCount > 86 Then
                    FileName = $"tmp/{FileNameCount}.mld"
                Else
                    FileName = $"tmp/{FileNameCount}.gif"
                End If
                File.WriteAllBytes(FileName, Filedata)
                FileNameCount += 1
            End While
            MessageBox.Show($"Extracted: {FileNameCount}")
        End Using
    End Function

End Class
