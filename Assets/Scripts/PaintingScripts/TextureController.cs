using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureController : MonoBehaviour
{
    public float timeStep = 5;
    private float currTime;
    private float updates = 0;
    private bool generate;

    public SpriteRenderer spriteRenderer;
    public Texture2D generatedTexture;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D newTex = new Texture2D(1, 50);
        Sprite newsprite = Sprite.Create(newTex, new Rect(0.0f, 0.0f, newTex.width, newTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = newsprite;
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
    }

    public void StopGenerating()
    {
        generate = false;
    }

    private void ExpandTexture()
    {
        int expandStep = 10;

        Texture2D oldTex = spriteRenderer.sprite.texture;
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

        Sprite newsprite = Sprite.Create(newTex, new Rect(0.0f, 0.0f, newTex.width, newTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = newsprite;

        generatedTexture = newTex;

        updates += 10;
        updates = updates % 255;
    }
}
