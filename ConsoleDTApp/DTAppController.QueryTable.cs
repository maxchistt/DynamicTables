using System.ComponentModel;

namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private enum todoQueryTable
        { ShowTable, AddRow, SelectWithParams };

        public void queryTable()
        {
            bool tableCosen = chooseTable();
            if (!tableCosen) return;

            var keysAndTypes = dtManager.Scaner.getColsKeysAndTypes(chosenTable);
            view.printMsg($"'{chosenTable}' table keys and types:\n{string.Join(" | ", keysAndTypes)}");

            bool resume = true;
            while (resume)
            {
                resume = chooseToDoQueryTable();
            }
        }

        public bool chooseToDoQueryTable()
        {
            var todoQueryTableRes = view.getChoiceEnum<todoQueryTable>("Choose what to do");
            if (todoQueryTableRes == null) return false;
            switch (todoQueryTableRes)
            {
                case todoQueryTable.ShowTable:
                    showTable();
                    break;

                case todoQueryTable.AddRow:
                    view.printMsg(addRow() ? "Row added" : "Row not added");
                    break;

                case todoQueryTable.SelectWithParams:
                    selectAndShowByParams();
                    break;

                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void showTable()
        {
            var table = dtManager.Querier.Select(chosenTable);

            string tableStr = $"Table '{chosenTable}'\n";
            table.ForEach(row =>
            {
                tableStr += string.Join(" | ", row) + "\n";
            });
            view.printMsg(tableStr);
        }

        public bool addRow()
        {
            Dictionary<string, object> row = new();
            var colsKeysAndTypes = dtManager.Scaner.getColsKeysAndCSTypes(chosenTable);
            foreach (var col in colsKeysAndTypes)
            {
            // label
            repeat:
                var type = col.Value;
                var input = view.getStringname($"Enter {type.Name} value for {col.Key} column:");
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null && input != null && input != "")
                {
                    var convres = converter.ConvertFromString(input);
                    if (convres != null)
                    {
                        row[col.Key] = convres;
                        continue;
                    }
                }
                var choiceRes = view.getChoice(new[] { "No value", "Null", "Empty str", "Rechoose" }, $"No value. Chose what to set as {col.Key}:");
                switch (choiceRes)
                {
                    case "No value":
                        continue;
                    case "Null":
                        row[col.Key] = null;
                        break;

                    case "Empty str":
                        row[col.Key] = "";
                        break;

                    case "Rechoose":
                        goto repeat;
                }
            }

            view.printMsg("Row:\n" + string.Join(" | ", row));
            if (view.getChoice(new[] { "Insert row", "Dont insert row" }) == "Insert row")
            {
                dtManager.Querier.Insert(chosenTable, row);
                return true;
            }
            return false;
        }
    }
}