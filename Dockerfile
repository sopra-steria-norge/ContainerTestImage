FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base

####################################################################################
### Requirements for implementing MSAL authentication: libsecret-1-dev 
####################################################################################
RUN apt-get update && apt install -y libsecret-1-0 libsecret-1-dev curl gpg

####################################################################################
### Install OpenSSH and set the password for root to "Docker!"
### This will make it possible to debug the container in the App Service SCM tooling
####################################################################################
RUN apt-get update \
     && apt-get install -y openssh-server htop dos2unix iputils-ping curl wget gss-ntlmssp\
     && echo "root:Docker!" | chpasswd 
# Copy the sshd_config file to the /etc/ssh/ directory
COPY ssh/printenv.sh /etc/ssh/
COPY ssh/sshd_config /etc/ssh/
RUN dos2unix /etc/ssh/*

# Copy and configure the ssh_setup file
COPY ssh/ssh_setup.sh /tmp/ssh_setup.sh
RUN dos2unix /tmp/ssh_setup.sh \
    && chmod +x /tmp/ssh_setup.sh \
    && (sleep 1;/tmp/ssh_setup.sh 2>&1 > /dev/null)
EXPOSE 2222
####################################################################################

WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY src/ .

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["src/ContainerPipelineTest/ContainerPipelineTest.csproj", "ContainerPipelineTest/"]
# RUN dotnet restore "ContainerPipelineTest/ContainerPipelineTest.csproj"
COPY src/ .
# WORKDIR "/src/ContainerPipelineTest"
# RUN dotnet build "ContainerPipelineTest.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "ContainerPipelineTest.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .

# # ENTRYPOINT ["dotnet", "ContainerPipelineTest.dll"]
# # Copy source files into the container for testing purposes using Remote SSH
# COPY . /root/ContainerPipelineTest

# COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
# RUN dos2unix /usr/local/bin/docker-entrypoint.sh \
#     && chmod +x /usr/local/bin/docker-entrypoint.sh
# ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]

ENTRYPOINT ["tail", "-f", "/dev/null"]
