Param
(
	[Switch] $silent # No output provided if set to true
)

# Rules
$ruleSummaryLengthMax = 400
$ruleParamMustTerminateByPeriod = $TRUE
$ruleParamTypeMustTerminateByPeriod = $TRUE
$ruleExceptionMustTerminateByPeriod = $TRUE

# Variables
$newLine = [Environment]::NewLine
$startPath = Get-Location

# Displays some disclaiming text.
function DisplayHeader()
{
  if (!$silent)
  {
    Clear-Host
    Write-Host "=========================="
    Write-Host " Verify XML Documentation"
    Write-Host "=========================="
    Write-Host
    Write-Host Please wait...
  }
}

# Returns the content of a file as a single string.
function GetFileContent([string] $path)
{
	[String]::Join($newLine, (Get-Content -Path $path))
}

# Recurses over the directories of the solution.
function Recurse([string] $path)
{
	$files = Get-ChildItem -Path $path -Recurse
  
	if ($files -ne $null)
	{
		foreach ($file in $files)
		{
			if (($file -is [System.IO.FileInfo]) -and ($file.Extension.ToLower() -eq '.cs'))
			{
        AnalyseFile $file.FullName
			}
		}
	}	
}

# Print an warning message and increment the warning counter.
function Warn ([string] $filePath, [string] $header, [string] $message)
{
  $global:warningCount ++

  if (!$silent)
  {
    Write-Host
    Write-Host ">> $filePath"
    Write-Host "   $header"
    Write-Host "   $message" -foregroundColor Red
  }
}

# Display the final message.
function DisplayFooter
{
  if (!$silent)
  {
    Write-Host
    Write-Host "Analysis complete with $global:warningCount warning(s)."
    Write-Host "Press [Enter] to exit..."
    Read-Host
  }
}

# Analyse the content of a file.
function AnalyseFile([string] $filePath)
{
  $content = GetFileContent $filePath 
  $pattern = '(\s*/// ?(?<line>.+)\r\n)+\s*(?<header>.+)\r\n'
  $matches = [Regex]::Matches($content, $pattern)
  
  foreach ($match in $matches)
  {  
    $doc = [String]::Empty
    $header = $match.Groups['header'].Value
    $match.Groups['line'].Captures | Foreach-Object { $doc += $_.Value }
    AnalyseSummary -filePath $filePath -header $header -doc $doc
    AnalyseParam -filePath $filePath -header $header -doc $doc
    AnalyseTypeParam -filePath $filePath -header $header -doc $doc
    AnalyseException -filePath $filePath -header $header -doc $doc
  }
}

# Analyse the <summary> element.
function AnalyseSummary([string] $filePath, [string] $header, [string] $doc)
{
  $pattern = '<summary>(?<data>.*)</summary>'
  $match = [Regex]::Match($doc, $pattern)

  if ($match.Success)
  {
    $data = $match.Groups['data'].Value
    
    if ($data.Length -gt $ruleSummaryLengthMax)
    {
      Warn -filePath $filePath -header $header -message "The content of the <summary> element is too long (max. $ruleSummaryLengthMax characters)."
    }
    
    #TODO: implemented other analysis criteria here...
  }
}

# Analyse the <param> element.
function AnalyseParam([string] $filePath, [string] $header, [string] $doc)
{
  $pattern = '<param name="\w+">(?<data>.*)</param>'
  $match = [Regex]::Match($doc, $pattern)

  if ($match.Success)
  {
    $data = $match.Groups['data'].Value
    
    if ($ruleParamMustTerminateByPeriod -and !$data.EndsWith('.'))
    {
      Warn -filePath $filePath -header $header -message "The content of the <param> element must ends with a period character."
    }
    
    #TODO: implemented other analysis criteria here...
  }
}

# Analyse the <paramtype> element.
function AnalyseTypeParam([string] $filePath, [string] $header, [string] $doc)
{
  $pattern = '<typeparam name="\w+">(?<data>.*)</typeparam>'
  $match = [Regex]::Match($doc, $pattern)

  if ($match.Success)
  {
    $data = $match.Groups['data'].Value
    
    if ($ruleParamTypeMustTerminateByPeriod -and !$data.EndsWith('.'))
    {
      Warn -filePath $filePath -header $header -message "The content of the <typeparam> element must ends with a period character."
    }
    
    #TODO: implemented other analysis criteria here...
  }
}

# Analyse the <exception> element.
function AnalyseException([string] $filePath, [string] $header, [string] $doc)
{
  $pattern = '<exception cref="\w+">(?<data>.*)</exception>'
  $match = [Regex]::Match($doc, $pattern)

  if ($match.Success)
  {
    $data = $match.Groups['data'].Value
    
    if ($ruleExceptionMustTerminateByPeriod -and !$data.EndsWith('.'))
    {
      Warn -filePath $filePath -header $header -message "The content of the <exception> element must ends with a period character."
    }
    
    #TODO: implemented other analysis criteria here...
  }
}

# Core script.
. DisplayHeader
. Recurse $startPath 
. DisplayFooter
return $global:warningCount