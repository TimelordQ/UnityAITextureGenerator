import sys
from diffusers import StableDiffusionPipeline
from diffusers import EulerAncestralDiscreteScheduler
import torch

def loadSD():
    tdTYPE = torch.float32
    model_id = "dream-textures/texture-diffusion"
    euler_scheduler = EulerAncestralDiscreteScheduler.from_pretrained(model_id, subfolder="scheduler")
    pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=tdTYPE, scheduler=euler_scheduler)
    pipe = pipe.to("cuda")
    return pipe

def runSD( pipe ):
    negativeprompt = "(low quality, worst quality:1.3), lowres, signature, text, jpeg artifacts"
    iterations = int( sys.argv[3] )
    fileName = sys.argv[2]
    prompt = f'pbr {sys.argv[1]}, 8k, hdr, photorealistic, texture, tileable, seamless'
    count = 0
    while (count < iterations):
        image = pipe(prompt, guidance_scale=7, num_inference_steps=20, negative_prompt=negativeprompt).images[0]
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