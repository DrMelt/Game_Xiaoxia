using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.Globalization;

public enum ScriptColumnNames
{
    None = -1,
    RowIndex = 0,
    ObjectName = 1,
    ActionName = 2,
    IsContinuous = 3,
    ActionDurationTime = 4,
    TextContent = 5,
    TextContentDurationTime = 6,
    CharacterDifferenceName = 7,
    AnimationName = 8,
    ColumnNums,
}

public class ScriptRow
{
    public ScriptRow(Dictionary<ScriptColumnNames, string> scriptRowContent)
    {
        this.scriptRowContent = scriptRowContent;
    }
    Dictionary<ScriptColumnNames, string> scriptRowContent = null;
    public Dictionary<ScriptColumnNames, string> ScriptRowContent => scriptRowContent;

    public string GetCellValue(ScriptColumnNames columnName)
    {
        if (scriptRowContent.Keys.Contains(columnName))
        {
            string content = scriptRowContent[columnName];
            if (string.IsNullOrEmpty(content))
            {
                if (columnName != ScriptColumnNames.IsContinuous)
                {
                    GD.Print($"Cell with name: {columnName} is empty.");
                }
                return null;
            }
            return content;
        }
        else
        {
            GD.Print($"Cell with name: {columnName} does not exist in the row.");
            return null;
        }
    }

    public int GetRowIndex()
    {
        string rowInd = GetCellValue(ScriptColumnNames.RowIndex);
        return int.Parse(rowInd);
    }
    public float GetActionDurationTime()
    {
        return float.Parse(GetCellValue(ScriptColumnNames.ActionDurationTime));
    }

    public float GetTextContentDurationTime()
    {
        return float.Parse(GetCellValue(ScriptColumnNames.TextContentDurationTime));
    }

    public string RowContentString()
    {
        string res = "";
        foreach (KeyValuePair<ScriptColumnNames, string> cell in scriptRowContent)
        {
            res += $"{cell.Key}: {cell.Value}" + "\n";
        }
        return res;
    }
}

public class ScriptTable
{

    List<ScriptRow> scriptList = new List<ScriptRow>();
    int currentScriptIndex = 0;
    public int CurrentScriptIndex => currentScriptIndex;

    public void ClearTable()
    {
        scriptList.Clear();
    }
    public void ReadExcelFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            GD.Print("File path is not specified.");
            return;
        }

        try
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                int rowIndex = 0;
                while (csv.Read())
                {
                    Dictionary<ScriptColumnNames, string> rowContent = new Dictionary<ScriptColumnNames, string>();
                    rowContent.Add(ScriptColumnNames.RowIndex, (++rowIndex).ToString());

                    bool isEmptyRow = true;
                    for (int col = 1; col < (int)ScriptColumnNames.ColumnNums; col++)
                    {
                        string cellValue = csv.GetField<string>(col);


                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            isEmptyRow = false;
                        }
                        cellValue = cellValue.Trim();
                        rowContent.Add((ScriptColumnNames)col, cellValue);
                    }

                    if (!isEmptyRow)
                    {
                        scriptList.Add(new ScriptRow(rowContent));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error reading CSV file: {ex.Message}");
        }

        // foreach (ScriptRow script in scriptList)
        // {
        //     GD.Print(script.RowContentString());
        // }
    }
    public string GetScriptRowInfo()
    {
        string rowInd = CurrentRow().GetCellValue(ScriptColumnNames.RowIndex);
        return $"Script Row: {rowInd}  ";
    }
    public ScriptRow CurrentRow()
    {
        if (currentScriptIndex >= scriptList.Count || currentScriptIndex < 0)
        {
            GD.Print(GetScriptRowInfo() + "Script Index is out of range.");
            return null;
        }
        return scriptList[currentScriptIndex];
    }
    public ScriptRow NextRow()
    {
        currentScriptIndex++;
        if (currentScriptIndex >= scriptList.Count || currentScriptIndex < 0)
        {
            GD.Print(GetScriptRowInfo() + "Script Index is out of range.");
            return null;
        }
        var scriptRow = scriptList[currentScriptIndex];
        return scriptRow;
    }


}
