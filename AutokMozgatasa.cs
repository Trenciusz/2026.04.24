using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutokMozgatasa;
class Jeladas
{
    public string Rendszam { get; set; }
    public int Ora { get; set; }
    public int Perc { get; set; }
    public int Sebesseg { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        List<Jeladas> jeladasok = new List<Jeladas>();
        Dictionary<string, List<Jeladas>> jarmuJeladasok = new Dictionary<string, List<Jeladas>>();

        string[] sorok = File.ReadAllLines("jeladas.txt");
        foreach (string sor in sorok)
        {
            if (string.IsNullOrWhiteSpace(sor)) continue;
            string[] reszek = sor.Split('\t');
            Jeladas j = new Jeladas
            {
                Rendszam = reszek[0],
                Ora = int.Parse(reszek[1]),
                Perc = int.Parse(reszek[2]),
                Sebesseg = int.Parse(reszek[3])
            };
            jeladasok.Add(j);
            if (!jarmuJeladasok.ContainsKey(j.Rendszam))
                jarmuJeladasok[j.Rendszam] = new List<Jeladas>();
            jarmuJeladasok[j.Rendszam].Add(j);
        }

        string elsoJarmu = jeladasok[0].Rendszam;

        Jeladas utolso = jeladasok[jeladasok.Count - 1];
        Console.WriteLine("2. feladat:");
        Console.WriteLine($"Az utolsó jeladás időpontja {utolso.Ora}:{utolso.Perc:D2}, a jármű rendszáma {utolso.Rendszam}");
        Console.WriteLine();

        Console.WriteLine("3. feladat:");
        Console.WriteLine($"Az első jármű: {elsoJarmu}");
        List<Jeladas> elsoJarmuJeladasok = jarmuJeladasok[elsoJarmu];
        string idopontok = string.Join(" ", elsoJarmuJeladasok.Select(j => $"{j.Ora}:{j.Perc}"));
        Console.WriteLine($"Jeladásainak időpontjai: {idopontok}");
        Console.WriteLine();

        Console.WriteLine("4. feladat:");
        Console.Write("Kérem, adja meg az órát: ");
        int bekertOra = int.Parse(Console.ReadLine());
        Console.Write("Kérem, adja meg a percet: ");
        int bekertPerc = int.Parse(Console.ReadLine());
        int darab = jeladasok.Count(j => j.Ora == bekertOra && j.Perc == bekertPerc);
        Console.WriteLine($"A jeladások száma: {darab}");
        Console.WriteLine();

        Console.WriteLine("5. feladat:");
        int maxSeb = jeladasok.Max(j => j.Sebesseg);
        var maxRendszamok = jeladasok.Where(j => j.Sebesseg == maxSeb).Select(j => j.Rendszam);
        Console.WriteLine($"A legnagyobb sebesség km/h: {maxSeb}");
        Console.WriteLine($"A járművek: {string.Join(" ", maxRendszamok)}");
        Console.WriteLine();

        Console.WriteLine("6. feladat:");
        Console.Write("Kérem, adja meg a rendszámot: ");
        string bekertRendszam = Console.ReadLine();

        if (!jarmuJeladasok.ContainsKey(bekertRendszam))
        {
            Console.WriteLine($"Nem szerepel {bekertRendszam} rendszámú jármű az adatok között.");
        }
        else
        {
            List<Jeladas> jarmuLista = jarmuJeladasok[bekertRendszam];
            double tavolsag = 0.0;
            for (int i = 0; i < jarmuLista.Count; i++)
            {
                Jeladas aktualis = jarmuLista[i];
                Console.WriteLine($"{aktualis.Ora}:{aktualis.Perc} {tavolsag:F1} km");

                if (i < jarmuLista.Count - 1)
                {
                    Jeladas kovetkezo = jarmuLista[i + 1];
                    int percKulonbseg = (kovetkezo.Ora * 60 + kovetkezo.Perc) - (aktualis.Ora * 60 + aktualis.Perc);
                    tavolsag += aktualis.Sebesseg * (percKulonbseg / 60.0);
                }
            }
        }
        Console.WriteLine();

        using (StreamWriter sw = new StreamWriter("ido.txt"))
        {
            foreach (var kvp in jarmuJeladasok)
            {
                string rendszam = kvp.Key;
                List<Jeladas> lista = kvp.Value;
                Jeladas elso = lista[0];
                Jeladas utolsoJ = lista[lista.Count - 1];
                sw.WriteLine($"{rendszam}\t{elso.Ora}\t{elso.Perc}\t{utolsoJ.Ora}\t{utolsoJ.Perc}");
            }
        }
        Console.WriteLine("7. feladat:");
        Console.WriteLine("Az ido.txt állomány elkészült.");
    }
}
