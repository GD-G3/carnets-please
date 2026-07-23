using UnityEngine;

[System.Serializable]
public class RasgosFaciales
{
    public bool esHombre;

    public int ojosID;
    public int cejasID;
    public int bocaID;
    public int cabelloID;

    public int cabezaID;
    public int narizID;

    public RasgosFaciales Copiar()
    {
        return new RasgosFaciales
        {
            esHombre = esHombre,
            ojosID = ojosID,
            cejasID = cejasID,
            bocaID = bocaID,
            cabelloID = cabelloID,
            cabezaID = cabezaID,
            narizID = narizID
        };
    }
}
