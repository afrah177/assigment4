using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Uppgift4;

namespace Vaccination
{
    public class People
    {
        public string BirthOfDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PersonsInRiskGroup { get; set; }
        public int GroupforInfection { get; set; }
        public int HealthCareStaff { get; set; }
    }

    public class PeopleDose
    {
        public string DoseBrithOfDate { get; set; }
        public string DoseFirstName { get; set; }
        public string DoseLastName { get; set; }
        public int DoseAmunt { get; set; }
    }



    public class Program
    {
        private static List<People> peopleList = new List<People>();
        private static string fileInput = @"C:\Vaccin\filein.csv";
        private static int vaccinqaunteti = 0;
        private static bool age = false;

        private static List<PeopleDose> peopleDoses = new List<PeopleDose>();
        private static string fileOutput = @"C:\Vaccin\fileout.csv";

        private static bool running = true;


        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            if (!File.Exists(fileInput))
            {
                using (File.Create(fileInput)) { }
            }

            while (running)
            {


                int option = ShowMenu("Gör ett val", new[]
                {
                "Skapa prioteringslista",
                "Lägg till person",
                "Åldergräns",
                "Antal vaccination",
                "indata",
                "utdata",
                "avslut"
                 });

                if (option == 0)
                {
                    PriorityList();
                }
                else if (option == 1)
                {
                    AddPeople();
                }
                else if (option == 2)
                {
                    AgeLimit();
                }
                else if (option == 3)
                {
                    QuantityVaccine();
                }
                else if (option == 4)
                {
                    Indata();
                }
                else if (option == 5)
                {
                    Outdata();
                }
                else if (option == 6)
                {
                    running = false;
                }

            }

            //skapa pri.
            //Addpeople.
            //åldersgräns.
            //vaccin antal.
            //indata.
            //utdata.

        }
        public static void PriorityList()
        {
            CreateVaccinationOrder(ReadFromIndataCSV(), vaccinqaunteti, age);

        }

        public static void AddPeople()
        {
            Console.Write("Personnummer: ");
            string birtOFDate = Console.ReadLine();

            Console.Write("Namn: ");
            string firstName = Console.ReadLine();

            Console.Write("Efternamn: ");
            string lastName = Console.ReadLine();

            int personsInRiskGroup = ShowChoice("Tillhör du riskgruppen?");

            int groupForInfection = ShowChoice("Har du varit sjuk tidigare?");

            int healthCareStaff = ShowChoice("Jobbar du inom vården?");

            var people = new People
            {
                BirthOfDate = birtOFDate,
                FirstName = firstName,
                LastName = lastName,
                PersonsInRiskGroup = personsInRiskGroup,
                GroupforInfection = groupForInfection,
                HealthCareStaff = healthCareStaff
            };
            peopleList.Add(people);
        }

        public static void AgeLimit()
        {
            int changeAge = ShowChoice("Vill du sätta åldergräns");
            if (changeAge < 1)
            {
                age = true;
            }
            else
            {
                age = false;
            }
        }

        public static void QuantityVaccine()
        {
            Console.Write("Antal dos ");
            vaccinqaunteti = int.Parse(Console.ReadLine());
            Console.WriteLine("Du har lagt till " + vaccinqaunteti);
        }

        public static void Indata()
        {
            while (true)
            {
                Console.Write("En ny sökväg för indata ");
                string newFilePath = Console.ReadLine();
                fileInput = newFilePath;

                if (File.Exists(fileInput))
                {
                    Console.WriteLine(newFilePath);

                    break;
                }
                else
                {
                    Console.WriteLine("Hittar inte filen");
                }
            }
        }

        public static void Outdata()
        {
            while (true)
            {

                Console.WriteLine("En ny sökväg för outdata");
                string newFilePath = Console.ReadLine();
                fileOutput = newFilePath;

                if (Directory.Exists(fileInput))
                {
                    Console.WriteLine(fileInput);

                    if (!File.Exists(fileOutput))
                    {
                        using (File.Create(fileOutput)) { }
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("Hittar inte mappen");
                }

            }
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            People people = new People();
            List<People> changeToList = new List<People>();
            List<People> transformbirthofdate = new List<People>();
            List<People> novaccitionforchildren = new List<People>();
            List<People> vaccinationforall = new List<People>();
            List<People> orderPeople = new List<People>();
            int age = int.Parse(people.BirthOfDate.Substring(0, 8));
            int doseforpeople = 2;


            foreach (string personData in input)
            {
                string[] lines = personData.Split(',');

                if (lines.Length >= 6)
                {
                    string birthOfDate = lines[0];
                    string lastName = lines[1];
                    string firstName = lines[2];
                    int personInRiskGroup = int.Parse(lines[3]);
                    int groupForInfection = int.Parse(lines[4]);
                    int healthCareStaff = int.Parse(lines[5]);

                    var person = new People
                    {
                        BirthOfDate= birthOfDate,
                        LastName = lastName,
                        FirstName = firstName,
                        PersonsInRiskGroup= personInRiskGroup,
                        GroupforInfection = groupForInfection,
                        HealthCareStaff = healthCareStaff
                    };
                    changeToList.Add(person);
                }
            }

            foreach (var person in changeToList)
            {
                string BirtOfDate = person.BirthOfDate;

                if (BirtOfDate.Length == 10)
                {
                    BirtOfDate = $"19{BirtOfDate.Insert(6, "-")}";
                }
                else if (BirtOfDate.Length == 11)
                {
                    BirtOfDate = $"19{BirtOfDate}";
                }
                else if (BirtOfDate.Length == 12)
                {
                    BirtOfDate = BirtOfDate.Insert(8, "-");
                }

                person.BirthOfDate = BirtOfDate;

                People changeBirthOfDate = new People
                {
                 BirthOfDate = BirtOfDate,
                 LastName = person.LastName,
                 FirstName = person.FirstName,
                 PersonsInRiskGroup = person.PersonsInRiskGroup,
                 GroupforInfection= person.GroupforInfection,
                 HealthCareStaff = person.HealthCareStaff,

                };
                transformbirthofdate.Add(changeBirthOfDate);

                People excludechildren = new People
                {
                    BirthOfDate = BirtOfDate,
                    LastName = person.LastName,
                    FirstName = person.FirstName,
                    PersonsInRiskGroup = person.PersonsInRiskGroup,
                    GroupforInfection = person.GroupforInfection,
                    HealthCareStaff = person.HealthCareStaff,

                };
                novaccitionforchildren.Add(excludechildren);


            }

            if (!vaccinateChildren)
            {
                if (age <= 20050101)
                {
                    
                    List<People> order = novaccitionforchildren
                   .OrderBy(person => person.BirthOfDate)
                   .ThenBy(person => person.HealthCareStaff == 1)
                   .ThenBy(person => age <= 19580101)
                   .ThenBy(person => person.PersonsInRiskGroup == 1)
                   .ToList();

                    List<People> notinorderpepole = novaccitionforchildren
                   .OrderBy(person => person.BirthOfDate)
                   .Except(orderPeople)
                   .ToList();
                    orderPeople = order.Concat(notinorderpepole).ToList();
                }

            }
            else
            {
                List<People> order = vaccinationforall
                      .OrderBy(person => person.BirthOfDate)
                      .ThenBy(person => person.HealthCareStaff == 1)
                      .ThenBy(person => age <= 19580101)
                      .ThenBy(person => person.PersonsInRiskGroup == 1)
                      .ToList();

                List<People> notinorderpepole = vaccinationforall
               .OrderBy(person => person.BirthOfDate)
               .Except(orderPeople)
               .ToList();
                orderPeople = order.Concat(notinorderpepole).ToList();
            }
            foreach (var person in orderPeople)
            {
                PeopleDose listPeopleDose = new PeopleDose
                {
                    DoseBrithOfDate = person.BirthOfDate,
                    DoseLastName = person.LastName,
                    DoseFirstName = person.FirstName,
                    DoseAmunt = doseforpeople - person.GroupforInfection,
                };

                peopleDoses.Add(listPeopleDose);
            }
            return new string[0];
        }

        public static string[] ReadFromIndataCSV()
        {


            string[] lines = File.ReadAllLines(fileInput);

            foreach (string line in lines)
            {
                string[] entries = line.Split(',');

                string britofdate = entries[0];
                string lastName = entries[1];
                string firstName = entries[2];
                int personsInRiskGroup = int.Parse(entries[3]);
                int groupforInfection = int.Parse(entries[4]);
                int healthCareStaff = int.Parse(entries[5]);

                peopleList.Add(new People
                {
                    BirthOfDate = britofdate,
                    LastName = lastName,
                    FirstName = firstName,
                    PersonsInRiskGroup = personsInRiskGroup,
                    GroupforInfection = groupforInfection,
                    HealthCareStaff = healthCareStaff
                });
            }

            return lines;

        }

        public static void SaveToIndataCSV()
        {
            var list = new List<string>();
            foreach (var person in peopleList)
            {
                string line =
                    person.BirthOfDate + "," +
                    person.LastName + "," +
                    person.FirstName + "," +
                    person.PersonsInRiskGroup + "," +
                    person.GroupforInfection + "," +
                    person.HealthCareStaff;
                list.Add(line);
            }

            File.WriteAllLines(fileInput, list);
        }

        public static void ReadFromOutdataCSV()
        {

        }

        public static void SaveToOutdataCSV()
        {

        }

        public static int ShowChoice(string heading)
        {
            List<string> choice = new List<string>();
            choice.Add("Nej");
            choice.Add("Ja");
            return ShowMenu(heading, choice);
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }


    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            using FakeConsole console = new FakeConsole("First input", "Second input");
            Program.Main();
            Assert.AreEqual("Hello!", console.Output);
        }
    }
}