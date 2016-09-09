[CmdletBinding()]

param()

$environmentVars = get-childitem -path env:*

foreach($var in $environmentVars)
{
    $keyname = $var.Key
    $keyvalue = $var.Value
    
    Write-Output "${keyname}: $keyvalue"
}