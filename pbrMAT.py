import sys
from diffusers import StableDiffusionPipeline
import torch

def loadSD():
    model_id = "dream-textures/texture-diffusion"
    pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
    pipe = pipe.to("cuda")
    return pipe

def runSD( pipe ):
    iterations = int( sys.argv[3] )
    fileName = sys.argv[2]
    prompt = sys.argv[1]
    count = 0
    while (count < iterations):
        image = pipe(prompt).images[0]
        saveFileNameFull = f'D:\\Develop\\Unity\\TextToMaterial\\{fileName}_{count}.png'
        image.save( saveFileNameFull )
        print(f'Saved image {saveFileNameFull}')
        count = count + 1

def runSDExtern( pipe, prompt, fileName, num_copies ):
    iterations = int( num_copies )
    count = 0
    while (count < iterations):
        image = pipe(prompt).images[0]
        saveFileNameFull = f'D:\\Develop\\Unity\\TextToMaterial\\{fileName}_{count}.png'
        image.save( saveFileNameFull )
        print(f'Saved image {saveFileNameFull}')
        count = count + 1


if __name__ == '__main__':
    n = len(sys.argv)
    if n <= 3: print("Usage: pbrMat.py <texture description> <texture filename> <number of generations>"), exit()

    pipe = loadSD()
    runSD( pipe )