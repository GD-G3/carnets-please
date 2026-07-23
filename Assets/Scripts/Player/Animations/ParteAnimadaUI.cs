using UnityEngine;

[System.Serializable]
public class ParteAnimadaUI
{
    [Header("Sprite cuando está quieto")]
    public Sprite spriteQuieto;

    [Header("Frames mientras camina")]
    public Sprite[] framesCaminar;

    public Sprite ObtenerFrame(int indice)
    {
        if (framesCaminar == null || framesCaminar.Length == 0)
        {
            return null;
        }

        int indiceSeguro = indice % framesCaminar.Length;
        return framesCaminar[indiceSeguro];
    }
}