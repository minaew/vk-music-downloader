import requests
import subprocess
import os
import time
import datetime
import json
import sys


def sendRequest(method, params = {}):
    r = requests.post('https://api.telegram.org/bot%s/%s'%(token, method), data = params)
    responce = r.json()
    assert responce['ok'] == True
    return responce['result']


def dowloadWallPost(url):
    substr = url[len('https://vk.com/wall-'):len(url)]
    splitted = substr.split('_')
    assert len(splitted) == 2
    group_id = splitted[0]
    post_id = splitted[1]
    print('downloading post %s of group %s'%(post_id, group_id))
    process = subprocess.run([cui, 'post', '-g', group_id, '-p', post_id], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if process.returncode == 0:
        dirPath = process.stdout.decode().split('\n')[0]
        if len(dirPath) != 0:
            for entry in os.listdir(dirPath):
                filePath = dirPath + '/' + entry
                print('sending ' + filePath)
                with open(filePath, 'rb') as audio:
                    r = requests.post("https://api.telegram.org/bot%s/sendAudio"%(token),
                                    data = { 'chat_id': from_id, 'title': getTitle(filePath), 'parse_mode': 'HTML' },
                                    files = { 'audio': audio.read() })
                    assert r.json()['ok'] == True
    else:
        error_message = process.stderr.decode().split('\n')[0]
        print(f'error: {error_message}')
        sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : error_message, 'reply_to_message_id' : message['message_id']})


def processCommand(command):
    if text.startswith('https://vk.com/wall-'):
        dowloadWallPost(command)


def getTitle(filePath):
    popen = subprocess.Popen([id3man, filePath, 'title'], stdout=subprocess.PIPE)
    title = popen.stdout.readline().decode()[:-1]
    return title



config_path = sys.argv[1]
nowstr = datetime.datetime.now().strftime('%d/%m/%y %H:%M:%S')
print(f'program started at {nowstr}')

with open(config_path, 'r') as file:
    content = file.read()
    config = json.loads(content)

    cui = config['cui']
    dirname = os.path.dirname(__file__)
    cui = os.path.join(dirname, cui)

    id3man = config['id3man']
    dirname = os.path.dirname(__file__)
    id3man = os.path.join(dirname, id3man)

    token = config['token']

    check_interval = config['check_interval']

    done_id_file_path = config['done_id_file_path']

    updates_dir = os.path.join('data', 'updates')
    messages_dir = os.path.join('data', 'messages')

print(f'cui: {cui}')
print(f'id3man: {id3man}')
print(f'check_interval: {check_interval}')
print(f'done_id_file_path: {done_id_file_path}')

if not os.path.exists(updates_dir):
    os.mkdir(updates_dir)
if not os.path.exists(messages_dir):
    os.mkdir(messages_dir)

while True:
    nowstr = datetime.datetime.now().strftime('%d/%m/%y %H:%M:%S')
    print(f'iteration started at {nowstr}')

    updates = sendRequest('getUpdates')
    print(f'{len(updates)} updates')

    processed_id = []
    if os.path.exists(done_id_file_path):
        with open(done_id_file_path, 'r') as file:
            processed_id = file.read()

    for update in updates:
        update_id = str(update['update_id'])

        if update_id in processed_id:
            print(f'update {update_id} already processed')
            continue
        else:
            print(f'processing update {update_id}')

        # dump update
        update_file_path = os.path.join(updates_dir, f'{update_id}.json')
        with open(update_file_path, 'w') as update_file:
            update_file.write(json.dumps(update, indent=4))
            update_file.write('\n')

        if 'message' in update:
            message = update['message']
            message_id = str(message['message_id'])
            from_id = message['from']['id']
            from_name = message['from']['first_name']

            # dump message
            from_dir = os.path.join(messages_dir, f'{from_id}_{from_name}')
            if not os.path.exists(from_dir):
                os.mkdir(from_dir)
            message_file_path = os.path.join(from_dir, f'{message_id}.json')
            with open(message_file_path, 'w') as message_file:
                message_file.write(json.dumps(message, indent=4))
                message_file.write('\n')

            if 'text' in message:
                text = message['text']
                sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'got it', 'reply_to_message_id' : message['message_id']})
                print('processing command')
                processCommand(text)
                sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'that is all, folks', 'reply_to_message_id' : message['message_id']})
                print('completed processing')

            with open(done_id_file_path, 'a') as file:
                file.write(update_id + '\n')
            print(f'{update_id} processed')
    
    nowstr = datetime.datetime.now().strftime('%d/%m/%y %H:%M:%S')
    print(f'iteration ended at {nowstr}')
    time.sleep(check_interval)
    