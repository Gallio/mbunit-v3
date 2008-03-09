Param
(
	[bool] $verifyOnly = $false, # No changes will be made if set to true
	[bool] $verbose = $false
)

function GetFileContent([string] $path)
{
	[String]::Join($newLine, (Get-Content -Path $path))
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
				if ($licencesByExtension.ContainsKey($extension) -and -not $excludedFiles.Contains($file.Name))
				{
					$fileContent = GetFileContent $file.FullName
					$relativeName = $file.FullName.Replace($startPath, "")
					if (-not $fileContent.Contains($firstLicenseLine))
					{	
						$filesToUpdate.Add($relativeName) | Out-Null
						if (-not $verifyOnly)
						{
							echo ("Updating " + $relativeName)
							($licencesByExtension[$file.Extension.ToLower()] + $fileContent) | Set-Content $file.FullName
						}
					}
					else
					{
						if ($verbose) 
						{
							# Not sure we need this
							#echo ("File " + $file.FullName + " is up to date")
						}
					}
				}
				else
				{
					if ($verbose) 
					{
						# Not sure we need this
						#echo ("Skipping file " + $file.FullName + " because extension " + $file.Extension + " has not associated license" )
					}
				}
			}
			else
			{
				if ($excludedFolders.Contains($file.Name.ToLower()))
				{
					if ($verbose)
					{
						$skippedFolders.Add($file.FullName.Replace($startPath, "")) | Out-Null
					}
				}
				else
				{
					Recurse $file.FullName
				}
			}
		}
	}	
}

$newLine = [Environment]::NewLine

$licenseFile = "./LicenseBoilerplate.txt"
$licenseContent = GetFileContent $licenseFile
# We need the first line of the license to search for it in each file
$firstLicenseLine = (Get-Content $licenseFile)[0].Replace("// ", "")

# For each extension a license must be specified
$licencesByExtension = @{}
$licencesByExtension[".cs"] = ($licenseContent + $newLine)
$licencesByExtension[".vb"] = ($licenseContent.Replace("// ", "' ") + $newLine)
$licencesByExtension[".xml"] = ("<!--" + $newLine + $licenseContent + "-->" + $newLine + $newLine)

# Special folders to exclude
$excludedFolders = New-Object System.Collections.ArrayList
$excludedFolders.Add("bin") | Out-Null
$excludedFolders.Add("templates") | Out-Null
$excludedFolders.Add("obj") | Out-Null

# Special files can also be excluded
$excludedFiles = New-Object System.Collections.ArrayList
$excludedFiles.Add("Resources.Designer.cs") | Out-Null

$filesToUpdate = New-Object System.Collections.ArrayList
$skippedFolders = New-Object System.Collections.ArrayList

cls
"Processing..."
$startPath = gl # Need to save it for later
$startPath | Recurse

if ($verifyOnly)
{
	if($filesToUpdate.Count -gt 0)
	{
		"The following files need to be updated:"
		""
		foreach ($file in $filesToUpdate)
		{
			echo ("  " + $file)
		}
	}
	else
	{
		echo "** All files are up to date **"
	}
}
else
{
	if($filesToUpdate.Count -eq 0)
	{
		echo "** All files are up to date **"
	}
}
if ($verbose)
{
	if($skippedFolders.Count -gt 0)
	{
		""
		"Skipped folders:"
		""
		foreach ($folder in $skippedFolders)
		{
			echo ("  " + $folder)
		}
	}
}
""