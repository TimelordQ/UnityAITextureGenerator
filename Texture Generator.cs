/* -----------------------------------------------------------------------------
 * 
 * File Name: Texture Generator.CS
 * Author: Brian Scott Gregory / "Q"
 * 
 * Description: 
 *    Creates a localized hole in space and time and chucks a penguin out the door. 
 *    
 *    All while simultaneously modifying the behavior of Unity to add a 
 *    "Tools->Texture Generator" menu item which, when selected, brings up a menu
 *    which allows the developer to type in any text prompt defining a texture desired
 *    which, provided the developer isn't a numbskull and has actually selected a game 
 *    object or closed the generator window that pops up - when the developer presses 
 *    the 'Generate' buttin, this editor plugin will automatically generate a texture
 *    and material, and then assign that material to the object selected. 
 *    
 *    To change the directory that houses the python code of will contain the generated
 *    materials and texture, the first four constants outlined below SHOULD be modified
 *    to point to your local installation and the Unity Destination for each. 

      private const string sROOTDIRECTORYFORPYTHON = "D:\\Develop\\Unity\\TextToMaterial";
      private const string sEDITORSERVER = sROOTDIRECTORYFORPYTHON + "\\PBRHandler.py";
      private const string sMATERIALSPATH = "Materials/PBR GENERATED";
      private const string sTEXTURESPATH = "Textures/PBR GENERATED";

 *   Installation note:
 *   THIS SCRIPT MUST BE PLACED IN THE ASSETS->EDITOR in your Unity Project for this
 *   to work.
 *   
 *   Developer Note:
 *   If you like this, I'm dirt poor (for real) - so donations and words of encouragement 
 *   are gladly accepted by snail mail at:
 *            Brian Gregory 1805 NE 94th Street #59 Vancouver, Washington 98665
 *   
 *   And you can find more of my stuff on my Youtube channel at: 
 *            https://www.youtube.com/channel/UCPDllySnQNlsQM0oyA_4-Wg
 *            
 *   This script/ software and associated content has no license. 
 *                            You are free to distribute it in any way you see fit. 
* ----------------------------------------------------------------------------- */
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Diagnostics.SymbolStore;
using UnityEngine.XR;
using UnityEngine.WSA;

public class MyCustomEditor : EditorWindow
{
    private const string sROOTDIRECTORYFORPYTHON = "D:\\Develop\\Unity\\TextToMaterial";
    private const string sEDITORSERVER = sROOTDIRECTORYFORPYTHON + "\\PBRHandler.py";
    private const string sMATERIALSPATH = "Materials/PBR GENERATED";
    private const string sTEXTURESPATH = "Textures/PBR GENERATED";

    private static Vector2 m_EditorHW = new Vector2(450, 200);
    private const int MARGIN = 50;
    private static TextField myT = null;
    private Label status;
    private static Process server;
    private static string sFileToImport = "";
    private static string sAssetName = "";

    private GameObject selectedGameObject;

    [MenuItem("Tools/Texture Generator")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("Texture Generator Tool");

        // Limit size of the window
        wnd.minSize = m_EditorHW;
        wnd.maxSize = m_EditorHW;

        server = ExecuteCommand(sEDITORSERVER);
    }

    public void OnDestroy()
    {
        if (server != null)
        {
            server.WaitForExit();
            server.Close();
        }
    }
    public void CreateGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new VisualElement();

        // Add a simple label
        myInspector.Add(new Label("\n\r   Prompt for AI to generate a new texture:\n\r"));
        myT = new TextField(250, true, false, '*');

        myT.isDelayed = true;
        myT.style.left = (MARGIN / 2);
        myT.style.width = m_EditorHW.x - MARGIN;
        myT.style.height = m_EditorHW.y - (int)myT.style.top.value.value - MARGIN - 50;
        //myT.RegisterCallback<KeyDownEvent>(OnKeyDownForAITextLookup);
        //myT.RegisterCallback<MouseDownEvent>(OnSelectGameObject);
        myInspector.Add(myT);
        TwoPaneSplitView tpsv = new TwoPaneSplitView();
        Button myButton = new Button(OnGenerate);
        myButton.text = "Generate";
        myButton.style.left = m_EditorHW.x - 100;
        myButton.style.width = 80;
        myButton.style.top = 5;

        //myInspector.Add(myButton);
        status = new Label("");
        status.style.left = ((m_EditorHW.x - 175) / 2);
        status.style.width = m_EditorHW.x - MARGIN;
        status.style.top = 5;
        // myInspector.Add(status);
        myInspector.Add(status);
        myInspector.Add(myButton);

        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(myInspector);
    }

    private static Process ExecuteCommand(string cmd)
    {
        var processInfo = new ProcessStartInfo(cmd);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = true;

        Process process = Process.Start(processInfo);

        return process;
    }

    private static void SendMessageToGenerator(string preparedStatement)
    {
        TcpClient socketConnection = new TcpClient("localhost", 10000);
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(preparedStatement.ToString());
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                stream.Flush();
                UnityEngine.Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            UnityEngine.Debug.Log("Socket exception: " + socketException);
        }
        socketConnection.Close();
    }

    private static void OnGenerate()
    {
        UnityEngine.Debug.Log("Generating Image");
        string buildDir;
        buildDir = UnityEngine.Application.dataPath;
        for (int x = 0; x < sMATERIALSPATH.Split('/').Length; x++)
        {
            Directory.CreateDirectory(buildDir + "/" + sMATERIALSPATH.Split('/')[x]);
            buildDir += "/" + sMATERIALSPATH.Split('/')[x];
        }
        buildDir = UnityEngine.Application.dataPath;
        for (int x = 0; x < sTEXTURESPATH.Split('/').Length; x++)
        {
            Directory.CreateDirectory(buildDir + "/" + sTEXTURESPATH.Split('/')[x]);
            buildDir += "/" + sTEXTURESPATH.Split('/')[x];
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        CallGenerator(myT.text);
        return;
    }

    private static void CallGenerator(string textureToGenerate)
    {
        //string command = string.Format("D:\\Develop\\Unity\\TextToMaterial\\pbrMAT.py \"{0}\" \"tmp\" 1", textureToGenerate.Trim());
        string fileNameOutput = textureToGenerate.Trim();
        fileNameOutput = fileNameOutput.Replace(" ,", "");
        HashSet<char> charsToKill = new HashSet<char>() { ',', ']', '[', '(', ')', ':' };
        fileNameOutput = fileNameOutput.ReplaceMultiple(charsToKill, ' ');
        fileNameOutput = fileNameOutput.Replace(" ", "");
        sAssetName = fileNameOutput;
        Directory.CreateDirectory(sROOTDIRECTORYFORPYTHON);
        sFileToImport = string.Format("{1}\\{0}_0.png", fileNameOutput, sROOTDIRECTORYFORPYTHON);
        string args = string.Format("pbr {0}|{1}|1", textureToGenerate.Trim(), fileNameOutput);

        SendMessageToGenerator(args);

    }

    internal static class Helper
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;

        private static bool IsFileLocked(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }

        internal static bool CanReadFile(string filePath)
        {
            //Try-Catch so we dont crash the program and can check the exception
            try
            {
                //The "using" is important because FileStream implements IDisposable and
                //"using" will avoid a heap exhaustion situation when too many handles  
                //are left undisposed.
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    if (fileStream != null) fileStream.Close();  //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
                }
            }
            catch (IOException ex)
            {
                //THE FUNKY MAGIC - TO SEE IF THIS FILE REALLY IS LOCKED!!!
                if (IsFileLocked(ex))
                {
                    // do something, eg File.Copy or present the user with a MsgBox - I do not recommend Killing the process that is locking the file
                    return false;
                }
            }
            finally
            { }
            return true;
        }
    }


    public void Update()
    {
        if (selectedGameObject != null && sFileToImport.Length > 0)
        {
            if (File.Exists(sFileToImport) && Helper.CanReadFile(sFileToImport))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(sFileToImport);
                string tName = string.Format("t{0}", sAssetName.ToUpper());
                string mName = string.Format("m{0}", sAssetName.ToUpper());
                string sAssetsFile = string.Format("{0}/{2}/{1}.png", UnityEngine.Application.dataPath, tName, sTEXTURESPATH);
                System.IO.File.Delete(sAssetsFile);
                System.IO.File.WriteAllBytes(sAssetsFile, bytes);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(sAssetsFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(string.Format("Assets/{1}/{0}.png", tName, sTEXTURESPATH), typeof(Texture2D));

                //Find the Standard Shader
                Material myMat = new Material(Shader.Find("Standard"));
                myMat.name = sAssetName;
                //myMat.EnableKeyword("_NORMALMAP");
                myMat.EnableKeyword("_METALLICGLOSSMAP");

                myMat.SetTexture("_MainTex", texture);
                myMat.SetTexture("_DetailMask", texture);
                //myMat.SetTexture("_MetallicGlossMap", texture);
                myMat.SetTexture("_SpecGlossMap", texture);
                myMat.SetTexture("_MaskMap", texture);
                myMat.SetTexture("_ParallaxMap", texture);
                myMat.SetTexture("_OcclusionMap", texture);
                // myMat.SetTexture("_NormalMap", texture);
                string sMATERIALFILE = string.Format("Assets/{1}/{0}.mat", mName, sMATERIALSPATH);
                AssetDatabase.DeleteAsset(sMATERIALFILE);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(myMat, sMATERIALFILE);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //myMat = (Material)AssetDatabase.LoadAssetAtPath(string.Format("Assets/Materials/PBR GENERATED/{0}.mat", mName), typeof(Material));
                myMat = (Material)AssetDatabase.LoadAssetAtPath<Material>(string.Format("Assets/{1}/{0}.mat", mName, sMATERIALSPATH));
                //myMat = Resources.Load(string.Format("Materials/PBR GENERATED/{0}.mat", mName), typeof(Material)) as Material;
                selectedGameObject.GetComponent<Renderer>().material = myMat;
                sFileToImport = sAssetName = "";
                UnityEngine.Debug.Log("Imported File");
            }
        }
        else if (sFileToImport.Length == 0)
        {
            selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                myT.SetEnabled(true);
                status.text = string.Format("GameObject Selected {0}", selectedGameObject.name);
            }
            else
            {
                myT.SetEnabled(false);
                status.text = "";
            }
        }


    }
}
