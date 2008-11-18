Param
(
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

	$items = $xml.psbase.SelectNodes("//m:Compile|//m:Embedded|//m:None", $nsmgr) | sort @{expression={$_.psbase.Name}},@{expression={$_.Include}} | % {$_.psbase.Name + ": " + $_.Include}
	$items}

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
				if ($extension -eq ".csproj")
				{
					$dir = $file.Directory.FullName
					$vs2008proj = $file.FullName
					$vs2010proj = [System.IO.Path]::Combine($dir, [System.IO.Path]::GetFileNameWithoutExtension($vs2008proj) + ".vs2010.csproj")
					if ([System.IO.File]::Exists($vs2010proj))
					{
						Check $vs2008proj $vs2010proj
					}
				}
			}
			else
			{
				Recurse $file.FullName
			}
		}
	}	
}

function Check([string] $vs2008proj, [string] $vs2010proj)
{
	Echo $vs2010proj
	Echo ""

	$vs2008items = GetProjectItems $vs2008proj
	$vs2010items = GetProjectItems $vs2010proj
	Diff $vs2008items $vs2010items	

	Echo ""
}

gl | Recurse

""
