CD %1
COPY Gallio.dll Temp.dll
..\..\..\..\tools\ILMerge\ILMerge.exe /out:Gallio.dll Temp.dll Microsoft.Cci.MetadataHelper.dll Microsoft.Cci.MetadataModel.dll Microsoft.Cci.PdbReader.dll Microsoft.Cci.PeReader.dll Microsoft.Cci.SourceModel.dll
DEL Temp.dll