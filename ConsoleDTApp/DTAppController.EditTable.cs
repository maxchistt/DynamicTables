namespace ConsoleDTApp
{
    internal partial class DTAppController
    {
        private enum todoEditTable
        { AddColumn };

        //{ RenameTable, AddColumn, EditColumn, RenameColumn, AddPrimaryKey, AddPriamryKeys, DropTable, DropColumn, DropPrimaryKeys };

        public void editTable()
        {
            bool tableCosen = chooseTable();
            if (!tableCosen) return;

            var keysAndTypes = dtManager.Scaner.getColsKeysAndTypes(chosenTable);
            view.printMsg($"'{chosenTable}' table keys and types:\n{string.Join(" | ", keysAndTypes)}");

            bool resume = true;
            while (resume)
            {
                resume = chooseToDoEditTable();
            }
        }

        public bool chooseToDoEditTable()
        {
            var todoEditTableRes = view.getChoiceEnum<todoEditTable>("Choose what to do");
            if (todoEditTableRes == null) return false;
            switch (todoEditTableRes)
            {
                case todoEditTable.AddColumn:
                    addColumn();
                    break;

                default:
                    view.printMsg("Nothing chosen to do");
                    return false;
            }
            return true;
        }

        public void addColumn()
        {
            string colname;
            string type;
            string? len;
            bool nullable;

            colname = view.getStringname("Write column name:") ?? "";
            if (colname == "") return;

            type = view.getChoice(dtManager.Builder.getAllSqlTypesList().ToArray(), "Choose column type:");
            if (type == "") return;

            len = view.getStringname("Enter length or just press 'Enter':");
            if (len == null || !int.TryParse(len, out _)) len = null;

            nullable = view.getChoice(new[] { "Null", "Not Null" }, "Choose if nullable:") != "Not Null";

            dtManager.Builder.AddColumn(chosenTable, colname, type, len, nullable);
        }
    }
}