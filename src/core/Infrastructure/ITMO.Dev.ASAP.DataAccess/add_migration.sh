#!/bin/bash

dotnet ef migrations add $1 --context DatabaseContext --output-dir Migrations -s ../../../ITMO.Dev.ASAP/
