#!/bin/bash

dotnet ef migrations add "$1" --context GithubDatabaseContext --output-dir Migrations -s ../../../ITMO.Dev.ASAP/