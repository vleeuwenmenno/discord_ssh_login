### Pre-requisites

- For compilation you need dotnet 5.0 or newer.
- For runtime you shouldn't need anything as long as you compile with `-p:PublishSingleFile=true --self-contained true`

### Usage

1. Update your `/etc/pam.d/sshd` file.
```
sudo nano /etc/pam.d/sshd
```

2. Add the following to the bottom of the file but just above `@include common-password`
```
# Request login permission from Discord webhook
session    required     pam_exec.so   /usr/local/sbin/discord_sshd_login
```

3. [A] Clone the repository and compile the executable:
```
git clone https://github.com/vleeuwenmenno/discord_ssh_login.git
cd discord_ssh_login/
dotnet publish pam_discord_login/pam_discord_login.csproj -c Release --runtime linux-x64 -p:PublishSingleFile=true --self-contained true -o build/
```
3. [B] Download the executable from the releases page.
4. Upload the executable to your server using your preferred method and then move it to the correct folder:<br />
```
sudo cp PATH_TO_BINARY_FILE /usr/local/sbin/pam_discord_login
sudo chmod +x /usr/local/sbin/pam_discord_login
```
 
5. Upload the shell script to your server using your preferred method and place the shell script in the same folder:
```
sudo cp PATH_TO_SHELL_SCRIPT /usr/local/sbin/discord_sshd_login
sudo chmod +x /usr/local/sbin/discord_sshd_login
```

6. Update the shell and replace the variable `WEBHOOK` with the end part of your webhook url from Discord.
7. Open a new terminal and try logging into your server, see if everything works. Try it multiple times with decline, allow etc.

### WARNING & DISCLAIMER

I recommend keeping a seperate shell with sudo rights ready and open at all times until you have confirmed everything is working as excepted. This can help save you in case you screwed something up.
I do not take any responsibility if your server went nuclear or if anyone somehow managed to bypass security and got in your server, USE THIS AT YOUR OWN RISK!
