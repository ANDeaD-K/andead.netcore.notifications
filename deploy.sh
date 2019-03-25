#!/bin/bash
set -ev

echo "dotnet andead.netcore.notifications.dll --server-key=$SERVER_KEY" >> ./publish/entrypoint.sh

docker build -t andead/netcore.notifications:latest publish/.

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker push andead/netcore.notifications:latest
