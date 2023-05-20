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

try_stop_and_remove_container () {
	if [ "$(docker ps -f name="$name" -q)" ]; then
		echo trying to stop "$name" container &&
		docker stop "$name" &&
		echo stopped "$name" container
	fi

	if [ "$(docker ps -f name="$name" -q -a)" ]; then
		echo trying to remove "$name" container &&
		docker container rm "$name" &&
		echo removed "$name" container
	fi
}

docker build -t "$name" -f Docker/build.dockerfile . &&
try_stop_and_remove_container &&
docker run \
	-e ASPNETCORE_ENVIRONMENT="$env" \
	-d \
	-p "$port":5069 \
	-t \
	--restart=unless-stopped \
	-v "$HOME"/Logs/"$name":/app/Serilogs \
	--name "$name" \
	"$name" &&
echo looks like container "$name" should start