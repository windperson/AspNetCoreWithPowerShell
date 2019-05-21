#!/usr/bin/env pwsh
# input parameter
param(
    [Parameter(HelpMessage="string to display")]
    [String]$inputContent
)

Write-Output "Input is:`r`n======`r`n$inputContent"
