using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeyObjTrail1 : MonoBehaviour
{
    private TrailRenderer tr;

    public int beyID;
    public Color beyColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    private Color pink = new Color(1.0f, 0.0f, 0.38f, 1.0f);
    private Color yellow = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private Color blue = new Color(0.1f, 0.77f, 1.0f, 1.0f);
    private Color red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    private Color purple = new Color(0.6f, 0.0f, 1.0f, 1.0f);
    // Start is called before the first frame update

    void Start()
    {


        tr = GetComponent<TrailRenderer>();

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();

        //Random
        int r = Random.Range(1, 3);

        /*
        if (beyID % 2 == 0)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(pink, 0.0f), new GradientColorKey(yellow, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        else
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(red, 0.0f), new GradientColorKey(purple, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
        }



        if (beyID % 4 == 1)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(pink, 0.0f), new GradientColorKey(pink, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        else if (beyID % 4 == 2)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(yellow, 0.0f), new GradientColorKey(yellow, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        else if (beyID % 4 == 3)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(red, 0.0f), new GradientColorKey(red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        else if (beyID % 4 == 0)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(purple, 0.0f), new GradientColorKey(purple, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        */

        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(pink, 0.0f), new GradientColorKey(yellow, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );



        tr.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setID(int ID)
    {
        beyID = ID;
    }
}
