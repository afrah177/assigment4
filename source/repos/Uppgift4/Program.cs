using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


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

        private static string fileInput = @"C:\Vaccin\Vaccin1.csv";
        private static int vaccinQuantity = 0;
        private static bool age = false;

        private static List<PeopleDose> peopleDoses = new List<PeopleDose>();
        private static string fileOutput = @"C:\Vaccin\output.csv";

        private static bool running = true;


        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (running)
            {
                Console.WriteLine("Huvudmeny\n");
                Console.WriteLine($"Antal vaccindoser: {vaccinQuantity}");
                Console.WriteLine($"Åldersgräns 18 år:");
                Console.WriteLine($"Indata: {fileInput}");
                Console.WriteLine($"Utdata: {fileOutput}");

                int option = ShowMenu("Gör ett val", new[]
                {
                "Skapa prioteringslista",
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
                    AgeLimit();
                }
                else if (option == 2)
                {
                    QuantityVaccine();
                }
                else if (option == 3)
                {
                    Indata();
                }
                else if (option == 4)
                {
                    Outdata();
                }
                else if (option == 5)
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
            string[] files = File.ReadAllLines(fileInput);

            try
            {
                Handleexception(files);
                string[] filesOutput = CreateVaccinationOrder(files, vaccinQuantity, age);

                if (!File.Exists(fileOutput))
                {
                    File.Create(fileOutput);
                    File.WriteAllLines(fileOutput, filesOutput);
                }
                else
                {
                    int choise = ShowChoice("En fil finns redan, Skriv över?");

                    if (choise == 1)
                    {
                        File.WriteAllLines(fileOutput, filesOutput);
                    }
                    else
                    {
                        return;
                    }

                }

                foreach (var person in peopleDoses)
                {
                    while (vaccinQuantity > 0)
                    {
                        if (person.DoseAmunt >= 2 && vaccinQuantity >= 2)
                        {
                            person.DoseAmunt -= 2;
                            vaccinQuantity -= 2;
                        }
                        else if (person.DoseAmunt >= 1 && vaccinQuantity >= 1)
                        {
                            person.DoseAmunt -= 1;
                            vaccinQuantity -= 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
            int inputvaccinamount = Vaccinamoutinput("ändra antal vaccin: ");
            vaccinQuantity += inputvaccinamount;
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

                if (Directory.Exists(Path.GetDirectoryName(fileOutput)))
                {
                    Console.WriteLine(fileOutput);

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

            int doseforpeople = 2;


            foreach (string personData in input)
            {
                string[] lines = personData.Split(',');

                string birthOfDate = lines[0];
                string lastName = lines[1];
                string firstName = lines[2];
                int personInRiskGroup = int.Parse(lines[3]);
                int groupForInfection = int.Parse(lines[4]);
                int healthCareStaff = int.Parse(lines[5]);

                var person = new People
                {
                    BirthOfDate = birthOfDate,
                    LastName = lastName,
                    FirstName = firstName,
                    PersonsInRiskGroup = personInRiskGroup,
                    GroupforInfection = groupForInfection,
                    HealthCareStaff = healthCareStaff
                };
                changeToList.Add(person);

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
                    GroupforInfection = person.GroupforInfection,
                    HealthCareStaff = person.HealthCareStaff,

                };
                transformbirthofdate.Add(changeBirthOfDate);
            }

            List<People> order = transformbirthofdate
                  .OrderBy(person => person.BirthOfDate)
                  .ToList();

            if (!vaccinateChildren)
            {
                order = transformbirthofdate.Where(person => Agecontrol(person.BirthOfDate) >= 18)
                  .ToList();
            }

            List<People> orderPeople = new List<People>();

            List<People> healthcareStaffFilter = new List<People>();
            List<People> oldAgeFilter = new List<People>();
            List<People> personInRiskGroupFilter = new List<People>();
            List<People> noOrderPeopleFilter = new List<People>();

            foreach (var person in order)
            {
                People filterpeople = new People
                {
                    BirthOfDate = person.BirthOfDate,
                    LastName = person.LastName,
                    FirstName = person.FirstName,
                    PersonsInRiskGroup = person.PersonsInRiskGroup,
                    GroupforInfection = person.GroupforInfection,
                    HealthCareStaff = person.HealthCareStaff
                };

                if (person.HealthCareStaff > 0)
                {
                    healthcareStaffFilter.Add(filterpeople);
                }
                else if (Agecontrol(person.BirthOfDate) >= 65)
                {
                    oldAgeFilter.Add(filterpeople);
                }
                else if (person.PersonsInRiskGroup > 0)
                {
                    personInRiskGroupFilter.Add(filterpeople);
                }
                else
                {
                    noOrderPeopleFilter.Add(filterpeople);
                }
            }

            orderPeople.AddRange(healthcareStaffFilter);
            orderPeople.AddRange(oldAgeFilter);
            orderPeople.AddRange(personInRiskGroupFilter);
            orderPeople.AddRange(noOrderPeopleFilter);

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

            var changePeople = peopleDoses;

            string[] outputLines =
            changePeople.Select(person =>
            $"{person.DoseBrithOfDate},{person.DoseLastName},{person.DoseFirstName},{person.DoseAmunt}")
            .ToArray();

            return outputLines;
        }

        public static string[] ReadFromIndataCSV()
        {
            string[] lines = File.ReadAllLines(fileInput);

            return lines;
        }

        public static int ShowChoice(string heading)
        {
            List<string> choice = new List<string>();
            choice.Add("Nej");
            choice.Add("Ja");
            return ShowMenu(heading, choice);
        }

        public static int Agecontrol(string birthDate)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            string birth = birthDate.Substring(0, 8);

            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.ParseExact(birth, "yyyyMMdd", CultureInfo.InvariantCulture));

            int newage = today.Year - dateOnly.Year;

            if (dateOnly > today.AddYears(-newage))
            {
                newage = newage - 1;
            }
            return newage;
        }

        public static void Handleexception(string[] handlearray)
        {
            List<string> exceptions = new List<string>();
            foreach (string a in handlearray)
            {
                string[] strings = a.Split(',');
                if (strings.Length != 6)
                {
                    exceptions.Add($"Fel för få värden");
                }

                string birthDate = strings[0];

                for (int i = 0; i < birthDate.Length; i++)
                {
                    char q = birthDate[i];
                    if (!char.IsDigit(q) || birthDate.Length < 10 || birthDate.Length > 13)
                    {
                        if (q != '-' || birthDate.Length < 10 || birthDate.Length > 13)
                        {
                            exceptions.Add($"Fel personnummer: {birthDate}");
                        }
                    }

                }
                if (strings[1] == null)
                {
                    exceptions.Add($"Fel efternamn: {strings[1]}");
                }

                if (strings[2] == null)
                {
                    exceptions.Add($"Fel namn: {strings[2]}");
                }

                if (strings[3] != "1" || strings[3] == null)
                {
                    if (strings[3] != "0" || strings[3] == null)
                    {
                        exceptions.Add($"Fel siffra 3 {strings[3]}");
                    }
                }

                if (strings[4] != "1" || strings[4] == null)
                {
                    if (strings[4] != "0" || strings[4] == null)
                    {
                        exceptions.Add($"Fel siffra 4 {strings[4]}");
                    }
                }

                if (strings[5] != "1" || strings[5] == null)
                {
                    if (strings[5] != "0" || strings[5] == null)
                    {
                        exceptions.Add($"Fel siffra 5 {strings[5]}");
                    }
                }
            }

            if (exceptions.Count > 0)
            {
                throw new Exception(string.Join(Environment.NewLine, exceptions));
            }

        }
        public static int Vaccinamoutinput(string vaccinamout)
        {
            while (true)
            {
                Console.Write(vaccinamout);
                string inputvaccinamout = Console.ReadLine();
                try
                {
                    int result = int.Parse(inputvaccinamout);
                    return result;
                }
                catch
                {
                    Console.WriteLine("ange ett heltal.");
                }
            }
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
        public void VaccinateWithChildren()
        {
            string[] input =
            {
                "20051215-2221,Austen,Jene,0,0,1",
                "7210012331,Sjödin,Agneta,1,1,0",
                "196701014422,Berg,Bengt,1,0,0",
                "19430204-5544,Bean,Björn,0,1,1",
                "193608021234,Simpson,Homer,1,1,1"
            };

            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 5);
            Assert.AreEqual("19360802-1234,Simpson,Homer,1", output[0]);
            Assert.AreEqual("19430204-5544,Bean,Björn,1", output[1]);
            Assert.AreEqual("20051215-2221,Austen,Jene,2", output[2]);
            Assert.AreEqual("19670101-4422,Berg,Bengt,2", output[3]);
            Assert.AreEqual("19721001-2331,Sjödin,Agneta,1", output[4]);
        }

        public void VaccinatWithoutChildren()
        {
            string[] input =
            {
                "20051215-2221,Austen,Jene,0,0,1",
                "7210012331,Sjödin,Agneta,1,1,0",
                "196701014422,Berg,Bengt,1,0,0",
                "19430204-5544,Bean,Björn,0,1,1",
                "193608021234,Simpson,Homer,1,1,1"
            };

            int doses = 10;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 4);
            Assert.AreEqual("19360802-1234,Simpson,Homer,1", output[0]);
            Assert.AreEqual("19430204-5544,Bean,Björn,1", output[1]);
            Assert.AreEqual("19670101-4422,Berg,Bengt,2", output[2]);
            Assert.AreEqual("19721001-2331,Sjödin,Agneta,1", output[3]);
        }
        public void VaccinationWithException()
        {
            string[] input =
            {
                "20051215-2221,Austen,Jene,0,0,1",
                "7210012331,Sjödin,Agneta,1,1,0",
                "196701014422,Berg,Bengt,1,0,0",
                "1943b204-5544,Bean,Björn,0,1,1",
                "193608021234,Simpson,Homer,1,1,1"
            };

            int doses = 10;
            bool vaccinateChildren = false;
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 1);
            Assert.AreEqual("Fel personnummer: 1943b204-5544", output[0]);

        }
        public void VaccinationWithMoreThanOneException()
        {
            string[] input =
            {
                "200512a15-2221,Austen,Jene,0,0,1",
                "7210012331,Sjödin,Agneta,a,1,0",
                "196701014422,Berg,Bengt,1,0,0",
                "19430204-5544,Bean,Björn,0,1,1",
                "193608021234,Simpson,Homer,1,1,1"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("Fel personnummer: 200512a15-2221", output[0]);
            Assert.AreEqual("Fel siffra 3 a", output[1]);

        }
        public void VaccinationWithAllZero()
        {
            string[] input =
            {
                "20051215-2221,Austen,Jene,0,0,0",
                "7210012331,Sjödin,Agneta,0,0,0",
                "196701014422,Berg,Bengt,0,0,0",
                "19430204-5544,Bean,Björn,0,0,0",
                "193608021234,Simpson,Homer,0,0,0"
             };

            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 5);
            Assert.AreEqual("19360802-1234,Simpson,Homer,2", output[0]);
            Assert.AreEqual("19430204-5544,Bean,Björn,2", output[1]);
            Assert.AreEqual("19670101-4422,Berg,Bengt,2", output[2]);
            Assert.AreEqual("19721001-2331,Sjödin,Agneta,2", output[3]);
            Assert.AreEqual("20051215-2221,Austen,Jene,2", output[4]);
        }
    }

}
