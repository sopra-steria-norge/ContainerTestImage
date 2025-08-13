# Kjør som Administrator i forkant:
# Set-ExecutionPolicy RemoteSigned

Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

$Source="ContainerTestImage"
$Target="ContainerTestImage"

Get-ChildItem -Filter "*" -Recurse | ForEach {  (Get-Content $_.PSPath | ForEach {$_ -creplace $Source, $Target}) | Set-Content $_.PSPath }
   
Get-ChildItem -Filter "*" -Recurse | Rename-Item -NewName {$_.name -creplace $Source, $Target }
