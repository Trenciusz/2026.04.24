using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeleptetoRendszer;
class Esemeny
{
    public string Kod { get; set; }
    public int Ora { get; set; }
    public int Perc { get; set; }
    public int EsemenyKod { get; set; }

    public int PercekBen => Ora * 60 + Perc;
}

class Program
{
    static void Main(string[] args)
    {
        List<Esemeny> esemenyek = new List<Esemeny>();

        foreach (string sor in File.ReadAllLines("bedat.txt"))
        {
            if (string.IsNullOrWhiteSpace(sor)) continue;
            string[] r = sor.Split(' ');
            string[] idoParts = r[1].Split(':');
            esemenyek.Add(new Esemeny
            {
                Kod = r[0],
                Ora = int.Parse(idoParts[0]),
                Perc = int.Parse(idoParts[1]),
                EsemenyKod = int.Parse(r[2])
            });
        }

        Console.WriteLine("2. feladat");
        var belepesek = esemenyek.Where(e => e.EsemenyKod == 1).ToList();
        var kilepesek = esemenyek.Where(e => e.EsemenyKod == 2).ToList();

        var elso = belepesek.First();
        var utolso = kilepesek.Last();

        Console.WriteLine($"Az első tanuló {elso.Ora:D2}:{elso.Perc:D2}-kor lépett be a főkapun.");
        Console.WriteLine($"Az utolsó tanuló {utolso.Ora:D2}:{utolso.Perc:D2}-kor lépett ki a főkapun.");

        var kesok = belepesek.Where(e => e.PercekBen > 470 && e.PercekBen <= 495).ToList();

        using (StreamWriter sw = new StreamWriter("kesok.txt"))
        {
            foreach (var k in kesok)
                sw.WriteLine($"{k.Ora:D2}:{k.Perc:D2} {k.Kod}");
        }

        Console.WriteLine("4. feladat");
        int menzasok = esemenyek.Where(e => e.EsemenyKod == 3).Select(e => e.Kod).Distinct().Count();
        Console.WriteLine($"A menzán aznap {menzasok} tanuló ebédelt.");

        Console.WriteLine("5. feladat");
        int kolcsonzok = esemenyek.Where(e => e.EsemenyKod == 4).Select(e => e.Kod).Distinct().Count();
        Console.WriteLine($"Aznap {kolcsonzok} tanuló kölcsönzött a könyvtárban.");
        if (kolcsonzok > menzasok)
            Console.WriteLine("Többen voltak, mint a menzán.");
        else
            Console.WriteLine("Nem voltak többen, mint a menzán.");

        int t1045 = 10 * 60 + 45;
        int t1050 = 10 * 60 + 50;
        int t1100 = 11 * 60 + 0;

        var visszajottek = esemenyek
            .Where(e => e.EsemenyKod == 1 && e.PercekBen > t1050 && e.PercekBen <= t1100)
            .Select(e => e.Kod)
            .Distinct()
            .ToHashSet();

        var kileptekSzunetben = esemenyek
            .Where(e => e.EsemenyKod == 2 && e.PercekBen >= t1045 && e.PercekBen <= t1050)
            .Select(e => e.Kod)
            .ToHashSet();

        var hatsosok = new List<string>();
        foreach (string tanulo in visszajottek)
        {
            if (kileptekSzunetben.Contains(tanulo)) continue; 

            var tanuloEsemenyek = esemenyek
                .Where(e => e.Kod == tanulo && e.PercekBen < t1045)
                .ToList();

            int egyenleg = 0;
            foreach (var e in tanuloEsemenyek)
            {
                if (e.EsemenyKod == 1) egyenleg++;
                else if (e.EsemenyKod == 2) egyenleg--;
            }

            if (egyenleg > 0)
                hatsosok.Add(tanulo);
        }

        Console.WriteLine("6. feladat");
        Console.WriteLine("Az érintett tanulók: ");
        Console.WriteLine(string.Join(" ", hatsosok)); string.Join(" ", hatsosok);

        Console.WriteLine("7. feladat");
        Console.Write("Egy tanuló azonosítója=");
        string keresett = Console.ReadLine().Trim();

        var tanuloOsszes = esemenyek.Where(e => e.Kod == keresett).ToList();
        var tanuloBelepesek = tanuloOsszes.Where(e => e.EsemenyKod == 1).ToList();
        var tanuloKilepesek = tanuloOsszes.Where(e => e.EsemenyKod == 2).ToList();

        if (tanuloBelepesek.Count == 0)
        {
            Console.WriteLine("Ilyen azonosítójú tanuló aznap nem volt az iskolában.");
        }
        else
        {
            var elsoBelepes = tanuloBelepesek.First();
            var utolsoKilepes = tanuloKilepesek.Last();

            int kulonbseg = utolsoKilepes.PercekBen - elsoBelepes.PercekBen;
            int orak = kulonbseg / 60;
            int percek = kulonbseg % 60;

            Console.WriteLine($"A tanuló érkezése és távozása között {orak} óra {percek} perc telt el.");
        }
    }
}

