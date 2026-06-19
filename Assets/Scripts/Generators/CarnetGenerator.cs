using UnityEngine;

public class CarnetGenerator : MonoBehaviour
{
    [Header("Generadores")]
    public AlumnoGenerator alumnoGenerator;

    [Header("Probabilidades de error")]
    [Range(0f, 1f)] public float probErrorIdentificacion = 0.10f;
    [Range(0f, 1f)] public float probErrorArea = 0.08f;
    [Range(0f, 1f)] public float probErrorTurno = 0.12f;
    [Range(0f, 1f)] public float probFechaVencida = 0.10f;

    [Header("Configuracion del comedor")]
    public int maxTurno = 10;

    public CarnetData GenerarCarnet(Alumno alumno)
    {
        CarnetData carnet = GenerarCarnetCorrecto(alumno);

        // Luego aplicamos errores con probabilidades
        AplicarErrores(carnet, alumno);

        return carnet;
    }

    private CarnetData GenerarCarnetCorrecto(Alumno alumno)
    {
        CarnetData carnet = new CarnetData();

        // copiamos los datos reales correctamente
        carnet.nombreCompleto = alumno.nombreCompleto;
        carnet.codigo = alumno.codigo;

        carnet.area = 1;

        carnet.fotoID = alumno.fotoID;

        carnet.facultad = alumno.facultad;
        carnet.carrera = alumno.carrera;

        carnet.turno = UnityEngine.Random.Range(1, ObtenerTurnoCorrecto() + 1);

        // Fecha normal válida.
        carnet.diaVencimiento = 31;
        carnet.mesVencimiento = 12;
        carnet.anioVencimiento = 2027;

        return carnet;
    }

    private void AplicarErrores(CarnetData carnet, Alumno alumno)
    {
        if (UnityEngine.Random.value < probErrorIdentificacion)
        {
            AplicarErrorIdentificacion(carnet, alumno);
        }

        if (UnityEngine.Random.value < probErrorArea)
        {
            AplicarErrorArea(carnet);
        }

        if (UnityEngine.Random.value < probErrorTurno)
        {
            AplicarErrorTurno(carnet);
        }

        if (UnityEngine.Random.value < probFechaVencida)
        {
            AplicarFechaVencida(carnet);
        }
    }

    // Errores

    private void AplicarErrorIdentificacion(CarnetData carnet, Alumno alumnoReal)
    {
        Genero generoReal = alumnoReal.nombreCompleto.genero;

        if (alumnoGenerator == null)
        {
            Debug.LogWarning("No hay AlumnoGenerator asignado. Se aplicara error basico de identificacion.");

            carnet.codigo = GenerarCodigoAlternativo(alumnoReal.codigo);
            carnet.fotoID = GenerarFotoDistinta(alumnoReal);

            return;
        }
        
        Alumno alumnoFalso = alumnoGenerator.GenerarAlumno(generoReal);

        carnet.nombreCompleto = alumnoFalso.nombreCompleto;
        carnet.codigo = GenerarCodigoAlternativo(alumnoReal.codigo);
        carnet.fotoID = GenerarFotoDistinta(alumnoReal);

        carnet.facultad = alumnoFalso.facultad;
        carnet.carrera = alumnoFalso.carrera;

        Debug.Log("Carnet con error de identificacion.");
    }

    private void AplicarErrorArea(CarnetData carnet)
    {
        carnet.area = 2;

        Debug.Log("Carnet con error de area.");
    }

    private void AplicarErrorTurno(CarnetData carnet)
    {
        int turnoActual = ObtenerTurnoCorrecto();

        if (turnoActual < maxTurno)
        {
            // aplicar error con un turno futuro.
            carnet.turno = UnityEngine.Random.Range(turnoActual + 1, maxTurno + 1);
            Debug.Log("Carnet con error de turno.");
        }
        
    }

    private void AplicarFechaVencida(CarnetData carnet)
    {
        int[] mesesPosibles = { 6, 12 };
        carnet.mesVencimiento = mesesPosibles[UnityEngine.Random.Range(0, mesesPosibles.Length)];
        carnet.anioVencimiento = UnityEngine.Random.Range(2023, 2026);
        
        if (carnet.mesVencimiento == 6)
        {
            carnet.diaVencimiento = 30;
        }
        else
        {
            carnet.diaVencimiento = 31;
        }

        Debug.Log("Carnet con fecha vencida.");
    }
    
    //metodos auxiliares 
    private int ObtenerTurnoCorrecto()
    {
        int turnoActual = GameClock.TurnoGlobal;

        if (turnoActual < 1) //ya que la gente se empieza a generar desde antes de las 12
        {
            turnoActual = 1;
        }

        return turnoActual;
    }

    private string GenerarCodigoAlternativo(string codigoReal)
    {
        int codigo;
        do {
            codigo = UnityEngine.Random.Range(20200000, 20270000);

        } while (codigo.ToString() == codigoReal);
        
        return codigo.ToString();
    }

    private int GenerarFotoDistinta(Alumno alumnoReal)
    {
        int nuevaFoto;

        do
        {
            if (alumnoReal.nombreCompleto.genero == Genero.Hombre)
            {
                nuevaFoto = UnityEngine.Random.Range(1, 11); // 1 al 10
            }
            else
            {
                nuevaFoto = UnityEngine.Random.Range(11, 21); // 11 al 20
            }
        }
        while (nuevaFoto == alumnoReal.fotoID);

    return nuevaFoto;
    }

}