#!/bin/bash

len=$#

if [ $len -eq 0 ]; then
	echo "No argument is given. Usage example: './run_container.sh CONTAINER_NAME PORT ASPNET_ENVIRONMENT'"
fi

if [ $len -ne 3 ]; then
	echo "No argument is given. Usage example: './run_container.sh CONTAINER_NAME PORT ASPNET_ENVIRONMENT'"
fi

name=$1
port=$2
env=$3

docker build -t "$name" -f Docker/build.dockerfile . &&
docker stop "$name" &&
echo stopped "$name" container &&
docker container rm "$name" &&
echo removed "$name" container &&
docker run \
	-e ASPNETCORE_ENVIRONMENT="$env" \
	-d \
	-p "$port":5069 \
	-t \
	--restart=unless-stopped \
	-v "$HOME"/Logs/"$name":/app/Serilogs \
	"$name" &&
echo looks like container "$name" should start