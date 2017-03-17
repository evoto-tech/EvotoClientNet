function Test-RegistryValue {

    param (
        [parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]$Path,

        [parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]$Value
    )

    try
    {
        Get-ItemProperty -Path $Path | Select-Object -ExpandProperty $Value -ErrorAction Stop | Out-Null
        return $true
    }
    catch
    {
        return $false
    }
}


New-PSDrive -Name HKCR -PSProvider Registry -Root HKEY_CLASSES_ROOT

$dir = pwd

Push-Location
Set-Location HKCR:

If (-Not (Test-Path ./evoto)) {
    echo "Creating"
    New-Item -Path ./ -Name evoto
}

If (-Not (Test-RegistryValue -Path ./evoto -Value "URL Protocol")) {
    New-ItemProperty -Path ./evoto -Name "URL Protocol" -Value ""
}

If (-Not (Test-Path ./evoto/shell)) {
    New-Item -Path ./evoto -Name shell
}

If (-Not (Test-Path ./evoto/shell/open)) {
    New-Item -Path ./evoto/shell -Name open
}

If (-Not (Test-Path ./evoto/shell/open/command)) {
    New-Item -Path ./evoto/shell/open -Name command
}

#Overwrite any existing value here
$path = Join-Path $dir "/bin/Debug/EvotoClient.exe"
$value = '"' + $path + '" "%1"'
Set-Item -Path ./evoto/shell/open/command -Value $value

Pop-Location