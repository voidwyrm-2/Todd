# Todd Discord Rich Presence Client

Todd is a customizable Discord RPC meant to allow you to have a custom Discord activity

# Installation

1. Go to the [Discord Developer Portal](<https://discord.com/developers/applications>) and click "New Application"
2. Name your application something (I named mine "Todd")
3. Keep that tab open, you'll need it in a second
4. Download an executable from the Releases on the right
    - win-x64: Windows on Intel or AMD, osx-arm64: MacOS, linux-x64: Linux on Intel or AMD
5. Move it to any folder you can `cd` into easily
6. Open a terminal `cd` to the folder with the executable
    - if you're on Windows, CMD or Powershell are fine, and you're free to use something else if you want
7. Go back to tab with your application and click the "Copy" button under "APPLICATION ID"
8. Back in the terminal, run `./Todd` (or `.\Todd.exe` on Windows)
9. You'll be prompted to input your application ID, paste (Shift + Control + V on CMD and Powershell) or type in the ID you got from step 7
10. You're all set! You can customize the rich presence with the configuration file at `[YOUR HOME FOLDER]/.todd/config.json`
