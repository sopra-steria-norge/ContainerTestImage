@REM This script will try to pull images from Azure Container Registry and then build a new container image

cd %~dp0

docker swarm init

docker rm -f containerpipelinetest-1

docker-compose -f docker-compose.yml down --remove-orphans

docker network create -d overlay --attachable clac_common_network

@REM docker-compose -f docker-compose.yml build --no-cache containerpipelinetest

@REM docker-compose -f docker-compose.yml pull --no-parallel

docker-compose -f docker-compose.yml build --progress plain 

docker-compose -f docker-compose.yml up -d --remove-orphans
REM wait for 1-2 seconds for the container to start
pause
docker exec -it containerpipelinetest-1 /bin/bash
