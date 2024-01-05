import socket
import sys
from diffusers import StableDiffusionPipeline
import torch
import pbrMAT

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the address given on the command line
server_address = ('localhost', 10000)
print( 'starting up on %s port %s' % server_address )
sock.bind(server_address)
sock.listen(1)

model_id = "dream-textures/texture-diffusion"
pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
pipe = pipe.to("cuda")

while True:
    print( 'waiting for a connection' )
    connection, client_address = sock.accept()
    allData = ""
    try:
        print( 'client connected:' )

        while True:
            data = connection.recv(250)
            if data == b'':
                params = allData.split("|")
                pbrMAT.runSDExtern( pipe, params[0], params[1], params[2] );
                break;
            else:
                allData = data.decode("utf-8")

            print( 'received "%s"' % data )
            print( 'AllData="%s"' % allData )
    finally:
        connection.close()
