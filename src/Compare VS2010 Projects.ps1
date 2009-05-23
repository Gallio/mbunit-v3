Param
(
	[Switch] $sync # Open up a diff/merge tool to synchronize changes.
)

function GetXml([string] $path)
{
	$xml = New-Object System.Xml.XmlDocument
	$xml.Load($path)
	$xml
}

function GetProjectItems([string] $path)
{
	$xml = GetXml $path
	$nsmgr = New-Object System.Xml.XmlNamespaceManager $xml.psbase.NameTable
	$nsmgr.AddNamespace("m", "http://schemas.microsoft.com/developer/msbuild/2003")

	$items = $xml.psbase.SelectNodes("//m:Compile|//m:EmbeddedResource|//m:Content|//m:None|//m:Folder", $nsmgr) | sort @{expression={$_.psbase.Name}},@{expression={$_.Include}} | % {$_.psbase.Name + ": " + $_.Include}
	$items
}

function Recurse([string] $path)
{
	$files = Get-ChildItem -Path $path
	if ($files -ne $null) # Did we find anything?
	{
		foreach ($file in $files)
		{
			# Is it a folder?
			if ($file -isnot [System.IO.DirectoryInfo])
			{
				$extension = $file.Extension.ToLower()
				if ($extension -eq ".csproj" -and $file.Name -notlike "*.vs2010.csproj")
				{
					$dir = $file.Directory.FullName
					$vs2008proj = $file.FullName
					$vs2010proj = [System.IO.Path]::Combine($dir, [System.IO.Path]::GetFileNameWithoutExtension($vs2008proj) + ".vs2010.csproj")
					
					Check $vs2008proj $vs2010proj
				}
			}
			else
			{
				# Ignore MbUnit Template projects.
				if ($file.Name -ne "Templates")
				{
					Recurse $file.FullName
				}
			}
		}
	}
}

function Check([string] $vs2008proj, [string] $vs2010proj)
{
	if ([System.IO.File]::Exists($vs2010proj))
	{
		$vs2008items = GetProjectItems $vs2008proj
		$vs2010items = GetProjectItems $vs2010proj
		$diffs = Diff $vs2008items $vs2010items

		if ($diffs)
		{
			PrintResult $vs2010proj $diffs

			if ($sync)
			{
				Echo "Opening WinMerge."
				Echo "Be sure to only synchronize changes to project items not project references."
				Echo ""

				$vs2008name = [System.IO.Path]::GetFileName($vs2008proj)
				$vs2010name = [System.IO.Path]::GetFileName($vs2010proj)

				..\bin\WinMergeU.exe /e /s /dl "VS 2008: $vs2008name" /dr "VS 2010: $vs2010name" /u /s $vs2008proj $vs2010proj
			}

			set-variable -Name outcome -Value 1 -Scope global
		}
	}
	else
	{
		PrintResult $vs2010proj "*** Missing VS 2010 project $vs2010proj"
		
		set-variable -Name outcome -Value 1 -Scope global
	}
}

function PrintResult([string] $vs2010proj, [object] $result)
{
	Echo $vs2010proj
	Echo ""

	$result

	Echo ""
}

set-variable -Name outcome -Value 0 -Scope global
gl | Recurse

""

exit $outcome
