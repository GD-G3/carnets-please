using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RevisionManager : MonoBehaviour
{
    public static RevisionManager instance;

    [Header("Carnet a revisar")]
    public CarnetData carnetActual;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {

    }

    public void Permitir()
    {
        Debug.Log("Lo admitiste");
    }

    public void Rechazar()
    {
        Debug.Log("Lo rechazaste");
    }

    void SiguienteTurno()
    {
        Debug.Log("Siguiente...");
        //colocar mas alumnos
    }
}