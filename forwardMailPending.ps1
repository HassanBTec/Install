Add-Type -assembly "Microsoft.Office.Interop.Outlook"
Add-type -assembly "System.Runtime.Interopservices"

# test

try
{
$outlook = [Runtime.Interopservices.Marshal]::GetActiveObject('Outlook.Application')
    $outlookWasAlreadyRunning = $true
}
catch
{
    try
    {
        $Outlook = New-Object -comobject Outlook.Application
        $outlookWasAlreadyRunning = $false
    }
    catch
    {
        write-host "You must exit Outlook first."
        exit
        
    }
}

$namespace = $Outlook.GetNameSpace("MAPI")

$inbox = $namespace.GetDefaultFolder([Microsoft.Office.Interop.Outlook.OlDefaultFolders]::olFolderInbox)

$mails = $inbox.Items | Where-Object{ $_.unread -eq $true } #| Where-Object {$_.Subject -like "ABC TEST*"}

foreach($mail in $mails) {
    $reply = $mail.forward()
    $reply.to = "hbahri@kpmg.ca"
    #$reply.body = "test..."
    #$reply.Subject = $mail.Item(0).subject
    #$reply.Body = $mail.Item(0).body
    $reply.send()
    while(($namespace.GetDefaultFolder([Microsoft.Office.Interop.Outlook.OlDefaultFolders]::olFolderOutbox)).Items.Count -ne 0) {
        Start-Sleep 1
    }
}

# Kill Process Outlook (close COM)
Get-Process "*outlook*" | Stop-Process –force