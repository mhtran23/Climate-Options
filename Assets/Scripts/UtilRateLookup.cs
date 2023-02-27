using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linq;
using UlitliyRates;

public class UtilRateLookup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var csvFile = File.ReadAllLines("state_rates.csv").Skip(1).Where(row => row.Length > 0).Select(Rates.ParseRow).ToList();
        int zip = Convert.ToInt32(Console.ReadLine());
        double elect = GetElect(zip, csvFile);
        double electric = Math.Round(elect, 2);
        double gases = GetGas(zip, csvFile);
        double gas = Math.Round(gases, 2);
        double oile = GetOil(zip, csvFile);
        double oil = Math.Round(gas, 2);
        double woods = GetWood(zip, csvFile);
        double wood = Math.Round(woods, 2);
        Console.WriteLine(electric + ", " + gas + ", " + oil + ", " + wood);

    }

    // Update is called once per frame
    void Update()
    {
        utliRate = new UtililityRates()
    }

    internal static double GetElect(int zip, List<Rates> file)
    {
        var item = (from r in file
                    where r.ZipCode.Equals(zip)
                    select (r.ElectricityPerKWH));
        return item.Sum();
    }

    internal static double GetGas(int zip, List<Rates> file)
    {
        var item = (from r in file
                    where r.ZipCode.Equals(zip)
                    select r.GasPerTherm);
        return item.Sum();
    }

    internal static double GetOil(int zip, List<Rates> file)
    {
        var item = (from r in file
                    where r.ZipCode.Equals(zip)
                    select r.OilPerGallon);
        return item.Sum();
    }
    internal static double GetWood(int zip, List<Rates> file)
    {
        var item = (from r in file
                    where r.ZipCode.Equals(zip)
                    select r.WoodPerPound);
        return item.Sum();
    }
}
