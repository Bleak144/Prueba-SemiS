using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;


class Elemento
{
    public string Nombre { get; set; }
    public int Peso { get; set; }
    public int Calorias { get; set; }
}


class Solucion
{
    private List<Elemento> elementos;
    private int caloriasMinimas;
    private int pesoMaximo;



    public Solucion(List<Elemento> elementos, int caloriasMinimas, int pesoMaximo)
    {
        this.elementos = elementos;
        this.caloriasMinimas = caloriasMinimas;
        this.pesoMaximo = pesoMaximo;
    }

    public (List<Elemento> mejorCombinacion, int mejorPeso, int mejorCalorias) EncontrarMejorCombinacion()
    {
        int mejorPeso = int.MaxValue;
        int mejorCalorias = int.MinValue;
        List<Elemento> mejorCombinacion = new List<Elemento>();
        int totalCombinaciones = 1 << elementos.Count;
        for (int i = 0; i < totalCombinaciones; i++)
        {
            List<Elemento> combinacionActual = new List<Elemento>();
            int pesoTotal = 0;
            int caloriasTotal = 0;
            for (int j = 0; j < elementos.Count; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    combinacionActual.Add(elementos[j]);
                    pesoTotal += elementos[j].Peso;
                    caloriasTotal += elementos[j].Calorias;
                }
            }
            if (caloriasTotal >= caloriasMinimas && pesoTotal <= pesoMaximo)
            {
                if (pesoTotal <= mejorPeso || (pesoTotal == mejorPeso && caloriasTotal >= mejorCalorias))
                {
                    mejorPeso = pesoTotal;
                    mejorCalorias = caloriasTotal;
                    mejorCombinacion = new List<Elemento>(combinacionActual);
                }
            }
        }
        return (mejorCombinacion, mejorPeso, mejorCalorias);
    }

}

class Persistencia
{
    private string archivo;

    public Persistencia(string archivo)
    {
        this.archivo = archivo;
    }

    public void Guardar(List<Elemento> elementos, int peso, int calorias)
    {
        var resultado = new
        {
            Elementos = elementos,
            PesoTotal = peso,
            CaloriasTotales = calorias
        };

        
        string archivo = $"resultado_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string json = JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(archivo, json);

    }
}

class Program
{

    static string archivoJson = "elementos.json";

    static List<Elemento> CargarElementos()
    {
        if (File.Exists(archivoJson))
        {
            string json = File.ReadAllText(archivoJson);
            return JsonSerializer.Deserialize<List<Elemento>>(json) ?? new List<Elemento>();
        }
        return new List<Elemento>();
    }

    static void GuardarElementos(List<Elemento> elementos)
    {
        string json = JsonSerializer.Serialize(elementos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(archivoJson, json);
    }

    static void Main()
    {


        List<Elemento> elementos = CargarElementos();

        Console.WriteLine("Actualmente hay estos elementos guardados:");

        foreach (var e in elementos)
        {
            Console.WriteLine($"- {e.Nombre} (Peso: {e.Peso}, Calorías: {e.Calorias})");
        }

        Console.WriteLine("¿Desea ingresar elementos? En caso de no escriba 0");
        int cantidadElementos = int.Parse(Console.ReadLine() ?? "0");

        for (int i = 0; i < cantidadElementos; i++)
        {
            Console.WriteLine($"Ingrese el nombre del elemento {i + 1}:");
            string nombre = Console.ReadLine() ?? "";
            Console.WriteLine($"Ingrese el peso del elemento {i + 1}:");
            int peso = int.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine($"Ingrese las calorías del elemento {i + 1}:");
            int calorias = int.Parse(Console.ReadLine() ?? "0");
            elementos.Add(new Elemento { Nombre = nombre, Peso = peso, Calorias = calorias });
        }

        Console.WriteLine("\nElementos ingresados:");
        foreach (var e in elementos)
        {
            Console.WriteLine($"- {e.Nombre} (Peso: {e.Peso}, Calorías: {e.Calorias})");
        }

        GuardarElementos(elementos);

        Console.WriteLine("\n Elementos guardados correctamente.");


        Console.WriteLine("Ingrese la cantidad mínima de calorías:");
        int caloriasMinimas = int.Parse(Console.ReadLine() ?? "0");
        Console.WriteLine("Ingrese el peso Máximo Permitido:");
        int pesoMaximo = int.Parse(Console.ReadLine() ?? "0");
        

        Solucion solucion = new Solucion(elementos, caloriasMinimas, pesoMaximo);
        var (mejorCombinacion, mejorPeso, mejorCalorias) = solucion.EncontrarMejorCombinacion();


        Persistencia persistencia = new Persistencia("resultado.json");

        if (mejorCombinacion.Count > 0)
        {
            Console.WriteLine("Mejor combinación encontrada:");
            foreach (var elemento in mejorCombinacion)
            {
                Console.WriteLine($"- {elemento.Nombre} (Peso: {elemento.Peso}, Calorías: {elemento.Calorias})");
            }
            Console.WriteLine($"Peso Total: {mejorPeso}, Calorías Totales: {mejorCalorias}");

            persistencia.Guardar(mejorCombinacion, mejorPeso, mejorCalorias);
            Console.WriteLine("\nResultado guardado en 'resultado.json'");
        }
        else
        {
            Console.WriteLine("No se encontró una combinación que cumpla con los requisitos.");
        }


    }
}






