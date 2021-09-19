import requests
import subprocess
import os

cui = '...'
token = '...'

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
    ffmpeg = subprocess.Popen(['ffmpeg', '-i', filePath, '-f', 'ffmetadata'], stderr=subprocess.PIPE)
    grep = subprocess.Popen(['grep', 'title'], stdin=ffmpeg.stderr, stdout=subprocess.PIPE)
    ffmpeg.stderr.close()
    output = grep.communicate()[0].decode()
    s = output.split(':')
    assert len(s) == 2
    return s[1].lstrip().splitlines()[0]


for update in sendRequest('getUpdates'):
    # print('UPDATE')
    # print(update)
    # id = update['update_id']
    if 'message' in update:
        message = update['message']
        message_id = str(message['message_id'])
        # print(message_id)
        f = open('processed.txt', 'rt')
        if message_id in f.read(): # TODO check with chat id
            # print('continuing')
            continue
        if 'text' in message:
            text = message['text']
            from_id = message['from']['id']
            sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'processing started', 'reply_to_message_id' : message['message_id']})
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
            sendRequest('sendMessage', params = {'chat_id' : from_id, 'text' : 'processing completed', 'reply_to_message_id' : message['message_id']})
        with open("processed.txt", "a") as file:
            file.write(message_id + '\n') # TODO write chat id