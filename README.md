# Unity AI Texture Generator using Stable Diffusion
This allows the developer to select any 3D object in Unity and fill in a text prompt defining a texture desired, and using stable diffusion will generate a texture, material and apply that material to the selected object. 

This editor utility modifies the behavior of any Unity Project to add a  "Tools->Texture Generator" menu item which, when selected, brings up a menu which allows the developer to type in any text prompt defining a texture desired which, provided the developer has actually selected a game object or closed the generator window that pops up - when the developer presses the 'Generate' buttin, this editor plugin will automatically generate a texture and material, and then assign that material to the object selected. 

To change the directory that houses the python code of will contain the generated materials and texture, the first four constants outlined below SHOULD be modifiedto point to your local installation and the Unity Destination for each. 

      private const string sROOTDIRECTORYFORPYTHON = "<PATH TO DIRECTORY OF TEXTURE DIFFUSION SCRIPTS>";
      private const string sEDITORSERVER = sROOTDIRECTORYFORPYTHON + "\\PBRHandler.py";
      private const string sMATERIALSPATH = "Materials/PBR GENERATED";
      private const string sTEXTURESPATH = "Textures/PBR GENERATED";

Installation note:
THIS SCRIPT MUST BE PLACED IN THE ASSETS->EDITOR in your Unity Project for this to work.
 
**INSTALLATION REQUIREMENTS:**

1. Install Stable Diffusion. I'm using Automatic1111 and have tested this out thoroughly with this. 
   Following the download instructions here: https://github.com/AUTOMATIC1111/stable-diffusion-webui to set up the environment.
2. Place PBRHandler.py AND pbrMAT.py in any directory of your choice. Take note of that directory.
3. Open a project in Unity. Create a Folder named "Editor" if it's not already there, and then place the file "Texture Generator.cs" in that folder.

That's it. 

Yep. That's sincerely it. 

Ok, don't believe me, here's a video demonstrating this 

***** TODO:::: INSERT YOUTUBE VIDEO HERE *****

Developer Note:
If you like this, I'm dirt poor (for real) - so donations and words of encouragement are gladly accepted by snail mail at:
           Brian Gregory 1805 NE 94th Street #59 Vancouver, Washington 98665
 
And you can find more of my stuff on my Youtube channel at: https://www.youtube.com/channel/UCPDllySnQNlsQM0oyA_4-Wg
This script/ software and associated content has no license. You are free to distribute it in any way you see fit. 

