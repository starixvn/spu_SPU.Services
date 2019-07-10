Get-Process | Where-Object { $_.Name -eq 'Fanex.NAP.FS.FlaggedAccount.Console' } | Select-Object -First 1 | Stop-Process -Force
