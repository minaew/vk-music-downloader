import requests
import subprocess
import os
import time
import datetime
import json

def sendRequest(method, params = {}):
    r = requests.post('https://api.telegram.org/bot%s/%s'%(token, method), data = params)
    responce = r.json()
    assert responce['ok'] == True
    return responce['result']


def parseText(text):
    if text.startswith('https://vk.com/wall-'):
        substr = text[len('https://vk.com/wall-'):len(text)]
        splitted = substr.split('_')
        assert len(splitted) == 2
        group_id = splitted[0]
        post_id = splitted[1]
        print('downloading post %s of group %s'%(post_id, group_id))
        process = subprocess.run([cui, 'post', '-g', group_id, '-p', post_id], stdout=subprocess.PIPE)
        return process.stdout.decode().splitlines()[0]
    return ''


def getTitle(filePath):
    popen = subprocess.Popen([id3man, filePath, 'title'], stdout=subprocess.PIPE)
    title = popen.stdout.readline().decode()[:-1]
    return title


with open('config.json', 'r') as file:
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

while True:
    now = datetime.datetime.now()
    nowstr = now.strftime('%d/%m/%y %H:%M:%S')
    print(f'STARTING at {nowstr}')
    updates = sendRequest('getUpdates')
    print(f'{len(updates)} updates')

    # TODO: do not process if 0 updates found
    processed_id = []
    with open(done_id_file_path, 'r') as file:
        processed_id = file.read()
    # print(processed_id)

    for update in updates:
        # print('UPDATE')
        # print(update)
        # id = update['update_id']
        if 'message' in update:
            update_id = str(update['update_id'])
            message = update['message']
            message_id = str(message['message_id'])
            print(f'processing {update_id}')

            if update_id in processed_id:
                print(f'{update_id} already processed')
                continue

            if 'text' in message:
                text = message['text']
                from_id = message['from']['id']
                sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'downloading ...', 'reply_to_message_id' : message['message_id']})
                print('downloading ...')
                dirPath = parseText(text)
                if len(dirPath) != 0:
                    for entry in os.listdir(dirPath):
                        filePath = dirPath + '/' + entry
                        print('sending ' + filePath)
                        with open(filePath, 'rb') as audio:
                            r = requests.post("https://api.telegram.org/bot%s/sendAudio"%(token),
                                            data = { 'chat_id': from_id, 'title': getTitle(filePath), 'parse_mode': 'HTML' },
                                            files = { 'audio': audio.read() })
                            assert r.json()['ok'] == True
                sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'completed', 'reply_to_message_id' : message['message_id']})
                print('completed')

            with open(done_id_file_path, 'a') as file:
                file.write(update_id + '\n')
            print(f'{update_id} processed')
    
    now = datetime.datetime.now()
    nowstr = now.strftime('%d/%m/%y %H:%M:%S')
    print(f'DONE at {nowstr}')
    time.sleep(check_interval)
    