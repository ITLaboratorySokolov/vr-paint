using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureController : MonoBehaviour
{
    public float timeStep = 5;
    public int expandStep = 10;

    private float currTime;
    private float updates = 0;
    private bool generate;

    public Texture2D generatedTexture;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSimpleGradient(6);
    }

    private void GenerateSimpleGradient(int len)
    {
        Texture2D tx = new Texture2D(len, 20, TextureFormat.RGB24, false);
        
        var texPix = tx.GetPixels();

        int updates = 0;
        Color c = new Color(updates / 255.0f, 1, 1 - updates / 255.0f, 1);
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < tx.height; j++)
            {
                texPix[j * tx.width + i] = c;
            }
            c = new Color(updates / 255.0f, 1, 1 - updates / 255.0f, 1);
            updates += 20;
            updates %= 255; 
        }
        
        tx.SetPixels(texPix);
        tx.Apply();

        byte[] bytes = tx.EncodeToPNG();
        var dirPath = Application.dataPath + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "SimpleGradient" + len + ".png", bytes);
    }

    // Update is called once per frame
    void Update()
    {
        if (generate)
        {
            currTime -= Time.deltaTime;

            if (currTime < 0.1)
            {
                ExpandTexture();
                currTime = timeStep;
            }
        }

    }

    public void StartGenerating()
    {
        currTime = timeStep;
        generate = true;
        generatedTexture = new Texture2D(1, 50);
        updates = 0;

        var texPix = generatedTexture.GetPixels();
        Color c = new Color(updates / 255.0f, 1, 1 - updates / 255.0f, 1);

        for (int i = 0; i < texPix.Length; i++)
        {
            texPix[i] = c;
        }
        generatedTexture.SetPixels(texPix);
        generatedTexture.Apply();
    }

    public void StopGenerating()
    {
        generate = false;
    }

    private void ExpandTexture()
    {
        Texture2D oldTex = generatedTexture; // spriteRenderer.sprite.texture;
        Texture2D newTex = new Texture2D(oldTex.width + expandStep, oldTex.height);

        var oldTexPix = oldTex.GetPixels();
        List<Color> newTexPix = new List<Color>(oldTexPix);
        Color c = new Color(updates / 255.0f, 1, 1 - updates / 255.0f, 1);

        for (int i = 0; i < oldTex.height; i++)
        {
            int stInd = i * newTex.width;
            stInd += oldTex.width;

            for (int j = 0; j < expandStep; j++)
            {
                newTexPix.Insert(stInd, c);
            }
        }
        newTex.SetPixels(newTexPix.ToArray());
        newTex.Apply();

        generatedTexture = newTex;

        updates += 50;
        updates = updates % 255;
    }
}
