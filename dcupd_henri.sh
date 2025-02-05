


#!/bin/bash
 #xdg-open "discord.com/api/download/stable?platform=linux&format=tar.gz" & DEPRECATED
 cd /home/henri/Downloads
 rm -f discord.tar.gz
 sudo curl -L -o /home/henri/Downloads/discord.tar.gz "discord.com/api/download?platform=linux&format=tar.gz"

 rm -r Discord/
 tar -xf discord.tar.gz
 cd Discord

  rm -r  /opt/discord
  mkdir  /opt/discord
 sudo cp -r /home/henri/Downloads/Discord/* /opt/discord/
 sudo cp /home/henri/Downloads/Discord/*.* /opt/discord/

  rm -r  /usr/share/discord
  mkdir  /usr/share/discord
 sudo cp -r /home/henri/Downloads/Discord/* /usr/share/discord/
 sudo cp /home/henri/Downloads/Discord/*.* /usr/share/discord/

 cd ..

 rm -r Discord
 rm -f discord.tar.gz
