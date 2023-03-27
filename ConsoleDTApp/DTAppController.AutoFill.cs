namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private string[] personalNames = { "Bob", "John", "Max", "Joe", "Bill", "Selena", "Angelina", "Mike", "Piter", "Fred", "George" };
        private string[] personalSurames = { "Brown", "Smith", "Jones", "Biden", "Gates", "Joly", "Wilson", "Griffin", "Mercury", "Cluney", "Williams", "Davis", "Miller" };
        private string[] personalSpecialities = { "Salesman", "Logist", "Programmer", "Manager" };

        public void autoFillTable()
        {
            string fillTabName = "PersonalFilledTable";

            if (!dtManager.Scaner.getTablesNames().Contains(fillTabName))
            {
                dtManager.Builder.CreateTable(fillTabName);
                dtManager.Builder.AddColumn(fillTabName, "Name");
                dtManager.Builder.AddColumn(fillTabName, "Surame");
                dtManager.Builder.AddColumn(fillTabName, "Age", "int", null);
                dtManager.Builder.AddColumn(fillTabName, "Speciality");
                dtManager.Builder.AddColumn(fillTabName, "Salary", "float", null);
            }

            //Создание объекта для генерации чисел
            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                Dictionary<string, object> dict = new();
                dtManager.Scaner.getColsKeys(fillTabName).ForEach(colKey =>
                {
                    switch (colKey)
                    {
                        case "Id":
                            var r1 = dtManager.Querier.Select(fillTabName, new(new string[] { "MAX(Id) AS MAXID" }));
                            var r2 = r1[0];
                            object maxid = r2["MAXID"] ?? "0";
                            int id = int.Parse(maxid.ToString() ?? "0");
                            id++;
                            dict.Add("Id", id);
                            break;
                        case "Name":
                            dict.Add("Name", personalNames[rnd.Next(0, personalNames.Length - 1)]);
                            break;
                        case "Surame":
                            dict.Add("Surame", personalSurames[rnd.Next(0, personalSurames.Length - 1)]);
                            break;
                        case "Age":
                            dict.Add("Age", rnd.Next(18, 50));
                            break;
                        case "Speciality":
                            dict.Add("Speciality", personalSpecialities[rnd.Next(0, personalSpecialities.Length - 1)]);
                            break;
                        case "Salary":
                            dict.Add("Salary", rnd.Next(1000, 8000));
                            break;
                    }
                });
                dtManager.Querier.Insert(fillTabName, dict);
            }

        }
    }
}