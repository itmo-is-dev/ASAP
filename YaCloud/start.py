#!/usr/bin/env python3

import argparse
import os
import requests
from python_on_whales import docker

def start_container(name, port, asp_env=None):
    docker.build(".", file='./Docker/build.dockerfile', tags=name)
    try:
        docker.stop(name)
        docker.remove(name)
        print('stopped and removed previous container')
    except:
        pass
    envs = {}
    envs['ASPNETCORE_ENVIRONMENT'] = asp_env if asp_env is not None else 'Production'
    docker.run(name, envs=envs, publish=[(port, 5069)], tty=True, detach=True, name=name, restart='always',
               volumes=[(os.environ.get('HOME') + '/Logs/%s' % name, '/app/Serilogs')])


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("-n", dest="name")
    parser.add_argument("-e", dest="asp_env")
    parser.add_argument("-p", dest="port")
    args = parser.parse_args()
    start_container(args.name, args.port, args.asp_env)
