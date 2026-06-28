using System.Collections.Generic;
using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    [Header("Dinero")]
    public float dineroTotal = 0f;
    public float salarioBasePorDia = 100f;
    
    [Header("Penalizaciones")]
    public float multaAprobacionIncorrecta = 20f;
    public float multaRechazoInjustificado = 20f;
    public float multaPermitirColado = 30f;
    public float multaIntrusoVentana = 50f;

    // Registro del día actual
    public struct Penalizacion
    {
        public string motivo;
        public float monto;

        public Penalizacion(string m, float cant)
        {
            motivo = m;
            monto = cant;
        }
    }

    private List<Penalizacion> penalizacionesDelDia = new List<Penalizacion>();

    public event Action<Penalizacion> OnPenalizacionRegistrada;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarNuevoDia()
    {
        penalizacionesDelDia.Clear();
        Debug.Log("Nuevo día iniciado. Salario base: S/ " + salarioBasePorDia);
    }

    public void RegistrarPenalizacion(string motivo, float monto)
    {
        Penalizacion p = new Penalizacion(motivo, monto);
        penalizacionesDelDia.Add(p);
        
        Debug.Log($"Multa registrada: {motivo} (-S/ {monto})");
        
        OnPenalizacionRegistrada?.Invoke(p);
    }

    public float CalcularPagoDelDia()
    {
        float totalMultas = 0f;
        foreach (var p in penalizacionesDelDia)
        {
            totalMultas += p.monto;
        }

        // El pago puede ser negativo si comete muchos errores.
        // min S/0
        float pagoFinal = Mathf.Max(0f, salarioBasePorDia - totalMultas);
        return pagoFinal;
    }

    public void AplicarPagoAlDineroTotal()
    {
        float pago = CalcularPagoDelDia();
        dineroTotal += pago;
        Debug.Log($"Fin del día. Pago: S/ {pago}. Dinero total: S/ {dineroTotal}");
    }

    public List<Penalizacion> ObtenerPenalizacionesDelDia()
    {
        return penalizacionesDelDia;
    }
}
