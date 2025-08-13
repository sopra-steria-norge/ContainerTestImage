@REM This script will pull images from Azure Container Registry rather than build image locally

cd %~dp0

docker swarm init

docker rm -f mssql-1

docker-compose -f docker-compose.yml down --remove-orphans

docker network create -d overlay --attachable demo_common_network

@REM docker-compose -f docker-compose.yml pull --no-parallel

docker-compose -f docker-compose.yml up -d --remove-orphans
REM wait for 1-2 seconds for the container to start
pause
docker exec -it mssql-1 /bin/bash
