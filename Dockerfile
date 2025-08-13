FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

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

# USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]

COPY ["src/ContainerTestImage/ContainerTestImage.csproj", "ContainerTestImage/"]

# COPY ["src/MyOtherProject/MyOtherProject.csproj", "MyOtherProject/"]

# This layer is deleted after use, so the PAT is not stored in the final image
ARG NUGET_PAT
RUN if [ -n "$NUGET_PAT" ]; then \
      dotnet nuget remove source "DemoSharedAPIFeed"; \
      dotnet nuget add source "https://pkgs.dev.azure.com/demo-demo/_packaging/SharedAPIFeed/nuget/v3/index.json" \
      --name "DemoSharedAPIFeed" --username "PAT" --password $NUGET_PAT --store-password-in-clear-text; \
    fi

RUN dotnet restore "./ContainerTestImage/ContainerTestImage.csproj"

COPY src/ .
WORKDIR "/src/ContainerTestImage"
RUN dotnet build "ContainerTestImage.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContainerTestImage.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "ContainerTestImage.dll"]

COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN dos2unix /usr/local/bin/docker-entrypoint.sh \
    && chmod +x /usr/local/bin/docker-entrypoint.sh

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
