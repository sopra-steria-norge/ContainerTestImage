#!/bin/bash

docker compose -f docker-compose.database.yml exec mssql-1 /bin/bash
