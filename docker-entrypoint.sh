#!/bin/bash

nohup /usr/sbin/sshd -D & disown

dotnet ContainerTestImage.dll
