using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BlipFill = DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using Font = DocumentFormat.OpenXml.Spreadsheet.Font;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties;
using NonVisualPictureDrawingProperties = DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties;
using NonVisualPictureProperties = DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties;

namespace InfrastructureCore.Helpers
{
    public static class ExcelHelper
    {
        public static SpreadsheetDocument UpdateCell(string filePath, string cellValues, int rowIndex, string columnName, CellValues cellDataType = CellValues.String, SpreadsheetDocument spreadsheet = null, string sheetName = "Sheet1")
        {
            if (string.IsNullOrEmpty(filePath) & spreadsheet == null) return null;
            // Open the document for editing.
            try
            {
                spreadsheet = spreadsheet == null ? SpreadsheetDocument.Open(filePath, true) : spreadsheet;
                WorksheetPart worksheetPart =
                      GetWorksheetPartByName(spreadsheet, sheetName);

                if (worksheetPart != null)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet,
                                             columnName, rowIndex);

                    cell.CellValue = new CellValue(cellValues);
                    cell.DataType = cellDataType;

                    worksheetPart.Worksheet.Save();
                }
                return spreadsheet;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // spreadsheet?.Dispose();
            }
            return null;
        }

        public static Row UpdateCell(Worksheet worksheet, Row row, string cellValues, int rowIndex, string columnName, CellValues cellDataType = CellValues.String)
        {
            try
            {
                Cell cell = GetCell(row, columnName, rowIndex);

                cell.CellValue = new CellValue(cellValues);
                cell.DataType = cellDataType;

                worksheet.Save();
            }
            catch (Exception ex)
            {

            }
            return row;
        }

        public static Row UpdateCell(Row row, string cellValues, int rowIndex, string columnName, CellValues cellDataType = CellValues.String)
        {
            try
            {
                Cell cell = GetCell(row, columnName, rowIndex);

                cell.CellValue = new CellValue(cellValues);
                cell.DataType = cellDataType;
            }
            catch (Exception ex)
            {

            }
            return row;
        }

        public static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName = "Sheet1")
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        public static WorksheetPart FirstWorksheetPart(SpreadsheetDocument document)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>();

            if (sheets.Count() == 0)
            {
                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        public static Cell GetCell(Row row, string columnName, int rowIndex)
        {
            if (row == null)
                return null;
            string cellindex = $"{columnName}{rowIndex}";
            return row.Elements<Cell>().Where(c => c.CellReference.Value == cellindex).First();
        }

        // Given a worksheet and a row index, return the row.
        public static Row GetRow(Worksheet worksheet, int rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        public static IEnumerable<Row> Rows(Worksheet worksheet, int rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>();
        }

        public static IEnumerable<Row> Rows(Worksheet worksheet)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>();
        }

        public static SheetData GetSheetData(Worksheet worksheet)
        {
            return worksheet.GetFirstChild<SheetData>();
        }

        public static Row CreateRow(Row refRow, SheetData sheetData)
        {

            uint newRowIndex = 0;
            var newRow = new Row() { RowIndex = refRow.RowIndex.Value };

            // Loop through all the rows in the worksheet with higher row
            // index values than the one you just added. For each one,
            // increment the existing row index.
            IEnumerable<Row> rows = sheetData.Descendants<Row>().Where(r => r.RowIndex.Value > refRow.RowIndex.Value);
            foreach (Row row in rows)
            {
                newRowIndex = System.Convert.ToUInt32(row.RowIndex.Value + 1);

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for reserved cells.
                    string cellReference = cell.CellReference.Value;
                    cell.CellReference = new StringValue(cellReference.Replace(row.RowIndex.Value.ToString(), newRowIndex.ToString()));
                }
                // Update the row index.
                row.RowIndex = new UInt32Value(newRowIndex);
            }

            //sheetData.InsertAt(newRow, 40);
            sheetData.InsertBefore(newRow, refRow);

            return newRow;
        }

        public static Row CreateRow(Row refRow, Worksheet worksheet)
        {
            var sheetData = worksheet.GetFirstChild<SheetData>();
            uint newRowIndex = 0;
            var newRow = new Row() { RowIndex = refRow.RowIndex.Value };

            // Loop through all the rows in the worksheet with higher row
            // index values than the one you just added. For each one,
            // increment the existing row index.
            IEnumerable<Row> rows = sheetData.Descendants<Row>().Where(r => r.RowIndex.Value > refRow.RowIndex.Value);
            foreach (Row row in rows)
            {
                newRowIndex = System.Convert.ToUInt32(row.RowIndex.Value + 1);

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for reserved cells.
                    string cellReference = cell.CellReference.Value;
                    cell.CellReference = new StringValue(cellReference.Replace(row.RowIndex.Value.ToString(), newRowIndex.ToString()));
                }
                // Update the row index.
                row.RowIndex = new UInt32Value(newRowIndex);
            }

            //sheetData.InsertAt(newRow, 40);
            sheetData.InsertBefore(newRow, refRow);

            return newRow;
        }

        // Set height for Row
        public static Row SetHeightForRow(this Row row, WorksheetPart worksheetPart, int height)
        {
            try
            {
                Worksheet worksheet = worksheetPart.Worksheet;

                row.Height = height;
                row.CustomHeight = true;

                worksheetPart.Worksheet.Save();
            }
            catch (Exception ex)
            {

            }
            return row;
        }

        #region "New code"

        public static Cell CopyStyleIndexFrom(this Cell cell, Cell from)
        {
            var row = cell.Parent as Row;
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;
            cell.StyleIndex = from.StyleIndex;
            worksheet.Save();

            return cell;
        }

        public static UInt32Value GetStyleIndex(this Cell cell)
        {
            return cell.StyleIndex;
        }

        public static Cell SetStyleIndex(this Cell cell, UInt32Value value)
        {
            var row = cell.Parent as Row;
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;

            cell.StyleIndex = value;
            worksheet.Save();

            return cell;
        }

        public static Row RemoveMergeCell(this Row row)
        {
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;
            var mergeCells = worksheet.Elements<MergeCells>().First();

            var merges = mergeCells.Elements<MergeCell>().ToList();
            var delmerge = merges.Where(x => !x.Reference.Value.Contains(row.RowIndex)).ToList();

            mergeCells.RemoveAllChildren();

            foreach (var merge in delmerge)
            {
                mergeCells.Append(merge);
            }

            return row;
        }

        public static Cell AddFormular(this Cell cell, string text)
        {
            Row row = cell.Parent as Row;
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;

            CellFormula cellformula = new CellFormula();
            cellformula.Text = text;
            cellformula.AlwaysCalculateArray = true;
            cellformula.CalculateCell = true;
            //cellformula.FormulaType = new EnumValue<CellFormulaValues>(CellFormulaValues.)
            cell.CellFormula = cellformula;

            worksheet.Save();

            return cell;
        }

        public static Cell GetCell(this Row row, string columnName)
        {
            if (row == null)
                return null;
            return row.Elements<Cell>().Where(c => c.CellReference.Value.StartsWith(columnName)).First();
        }

        public static Cell GetCell(this Cell cell, string columnName)
        {
            var row = cell.Parent as Row;
            if (row == null)
                return null;
            return row.Elements<Cell>().Where(c => c.CellReference.Value.StartsWith(columnName)).First();
        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        public static Cell GetCell(Worksheet worksheet, string columnName, int rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;
            string cellindex = $"{columnName}{rowIndex}";
            return row.Elements<Cell>().Where(c => c.CellReference.Value == cellindex).First();
        }

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static Cell UpdateCell(this Cell cell, object cellValues, CellValues cellDataType = CellValues.String)
        {
            try
            {
                string cvalue = cellValues.ToString();
                if (cellValues.IsNumber()) cellDataType = CellValues.Number;
                else if (cellValues is bool) cellDataType = CellValues.Boolean;
                else if (cellValues is DateTime) cellDataType = CellValues.Date;

                cell.CellValue = new CellValue(cvalue);
                cell.DataType = cellDataType;

            }
            catch (Exception ex)
            {

            }
            return cell;
        }

        // Set style custom for Cell in Excel
        public static Cell SetStyleCustom(this Cell cell, SpreadsheetDocument spreadSheet, WorksheetPart worksheetPart, object cellValues, CellValues cellDataType = CellValues.String, string color = "white", string fontColor = "Black")
        {
            try
            {
                Worksheet worksheet = worksheetPart.Worksheet;
                //reference https://stackoverflow.com/questions/14051642/how-to-set-cells-background
                WorkbookStylesPart styles = spreadSheet.WorkbookPart.WorkbookStylesPart;
                Stylesheet stylesheet = styles.Stylesheet;

                CellFormats cellformats = stylesheet.CellFormats;

                string cvalue = cellValues.ToString();
                if (cellValues.IsNumber()) cellDataType = CellValues.Number;
                else if (cellValues is bool) cellDataType = CellValues.Boolean;
                else if (cellValues is DateTime) cellDataType = CellValues.Date;

                cell.CellValue = new CellValue(cvalue);
                cell.DataType = cellDataType;

                switch (color)
                {
                    case "white":
                        color = "FFFFFF"; break;
                    case "Yellow":
                        color = "ffff00"; break;
                    case "Red":
                        color = "ff0000"; break;
                    case "Green":
                        color = "008000"; break;
                    case "gray":
                        color = "666666"; break;
                    case "blue":
                        color = "c5d9f1"; break;
                    case "Orange":
                        color = "ffc000 "; break;
                    default:
                        color = "FFFFFF"; break;
                }

                switch (fontColor)
                {
                    case "white":
                        fontColor = "FFFFFF"; break;
                    case "Yellow":
                        fontColor = "ffff00"; break;
                    case "Red":
                        fontColor = "ff0000"; break;
                    case "Green":
                        fontColor = "008000"; break;
                    case "gray":
                        fontColor = "666666"; break;
                    case "blue":
                        fontColor = "c5d9f1"; break;
                    case "Orange":
                        fontColor = "ffc000 "; break;
                    default:
                        fontColor = "000000"; break;
                }

                Fonts fonts = stylesheet.Fonts;

                //UInt32 fontIndex = fonts.Count;
                UInt32 formatIndex = cellformats.Count;

                CellFormat f = (CellFormat)cellformats.ElementAt((int)cell.StyleIndex.Value);
                CellFormat newformat = (CellFormat)f.Clone();

                var font = (Font)fonts.ElementAt((int)f.FontId.Value);
                var newfont = (Font)font.Clone();
                newfont.Color = new Color() { Rgb = new HexBinaryValue(fontColor) };
                fonts.Append(newfont);
                var fontId = stylesheet.Fonts.Count++;
                newformat.FontId = (UInt32)fontId;

                #region "Set background color for Cell"

                Fill fill = new Fill() { PatternFill = new PatternFill { ForegroundColor = new ForegroundColor { Rgb = new HexBinaryValue(color) }, PatternType = PatternValues.Solid } };
                stylesheet.Fills.Append(fill);
                //or use the method Count(),not need to plus 1,but slowly.
                var fid = stylesheet.Fills.Count++;
                newformat.FillId = (UInt32)fid;

                #endregion

                #region "Set border for Cell"

                Borders borders1 = new Borders() { Count = (UInt32Value)1U };
                Border border1 = new Border();
                LeftBorder leftBorder1 = new LeftBorder();
                leftBorder1.Color = new Color() { Rgb = new HexBinaryValue("#000000") };
                RightBorder rightBorder1 = new RightBorder();
                rightBorder1.Color = new Color() { Rgb = new HexBinaryValue("#000000") };
                TopBorder topBorder1 = new TopBorder();
                topBorder1.Color = new Color() { Rgb = new HexBinaryValue("#000000") };
                BottomBorder bottomBorder1 = new BottomBorder();
                bottomBorder1.Color = new Color() { Rgb = new HexBinaryValue("#000000") };
                DiagonalBorder diagonalBorder1 = new DiagonalBorder();

                border1.Append(leftBorder1);
                border1.Append(rightBorder1);
                border1.Append(topBorder1);
                border1.Append(bottomBorder1);
                border1.Append(diagonalBorder1);
                borders1.Append(border1);

                newformat.BorderId = (UInt32)borders1.Count;

                #endregion

                var theStyleIndex = stylesheet.CellFormats.Count++;
                cell.StyleIndex = new UInt32Value { Value = (UInt32)theStyleIndex };

                cellformats.Append(newformat);
                stylesheet.Save();
                cell.StyleIndex = formatIndex;
                //Save the worksheet.
                worksheetPart.Worksheet.Save();
            }
            catch (Exception ex)
            {

            }
            return cell;
        }

        public static Cell SetFullBorder(this Cell cell, WorksheetPart worksheetPart)
        {
            WorkbookPart workbookPart = (WorkbookPart)worksheetPart.GetParentParts().FirstOrDefault();
            CellFormat cellFormat = cell.StyleIndex != null ? GetCellFormat(workbookPart, cell.StyleIndex).CloneNode(true) as CellFormat : new CellFormat();
            cellFormat.FillId = InsertFill(workbookPart, GenerateFill());
            cellFormat.BorderId = InsertBorder(workbookPart, GenerateBorder());

            cell.StyleIndex = InsertCellFormat(workbookPart, cellFormat);

            return cell;
        }

        public static CellFormat GetCellFormat(WorkbookPart workbookPart, uint styleIndex)
        {
            return workbookPart.WorkbookStylesPart.Stylesheet.Elements<CellFormats>().First().Elements<CellFormat>().ElementAt((int)styleIndex);
        }

        public static uint InsertCellFormat(WorkbookPart workbookPart, CellFormat cellFormat)
        {
            CellFormats cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.Elements<CellFormats>().First();
            cellFormats.Append(cellFormat);
            return (uint)cellFormats.Count++;
        }

        public static Border GenerateBorder()
        {
            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color1 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color1);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color2 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color2);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color3);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color4);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            return border2;
        }

        public static Fill GenerateFill()
        {
            Fill fill = new Fill();

            PatternFill patternFill = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFFFF" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };

            patternFill.Append(foregroundColor1);
            patternFill.Append(backgroundColor1);

            fill.Append(patternFill);

            return fill;
        }

        public static uint InsertBorder(WorkbookPart workbookPart, Border border)
        {
            Borders borders = workbookPart.WorkbookStylesPart.Stylesheet.Elements<Borders>().First();
            borders.Append(border);
            return (uint)borders.Count++;
        }

        public static uint InsertFill(WorkbookPart workbookPart, Fill fill)
        {
            Fills fills = workbookPart.WorkbookStylesPart.Stylesheet.Elements<Fills>().First();
            fills.Append(fill);
            return (uint)fills.Count++;
        }

        public static Cell UpdateCell(this Cell cell)
        {
            try
            {

                cell.CellValue = new CellValue(string.Empty);
                cell.DataType = CellValues.String;
            }
            catch (Exception ex)
            {

            }
            return cell;
        }

        public static Row CopyRowTo(this Row row, WorksheetPart worksheetPart, uint newindex)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            ////https://stackoverflow.com/questions/11116176/cell-styles-in-openxml-spreadsheet-spreadsheetml
            WorkbookPart workbookPart = (WorkbookPart)worksheet.WorksheetPart.GetParentParts().FirstOrDefault();

            //var b = a.GetPartsOfType<WorkbookStylesPart>().FirstOrDefault();

            Row newRow;

            if (sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).Count() != 0)
            {
                newRow = sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).First();
            }
            else
            {
                newRow = new Row() { RowIndex = newindex };
                sheetData.Append(newRow);
            }

            var cells = row.Elements<Cell>();
            foreach (var cell in cells)
            {
                Cell newCell = new Cell
                {
                    CellReference = $"{GetColumnName(cell.CellReference)}{newindex}",
                    StyleIndex = cell.StyleIndex, //InsertCellFormat(workbookPart, GetCellFormat(workbookPart, cell.StyleIndex)),
                    CellValue = new CellValue(cell.CellValue != null ? !string.IsNullOrEmpty(cell.CellValue.InnerText) ? cell.CellValue.InnerText : "" : ""),
                };

                newRow.InsertBefore(newCell, null);

            }

            row.CopyMergeCellTo(newRow, worksheetPart);

            worksheet.Save();


            return newRow;
        }

        public static Row CopyRowTo(this Row row, WorksheetPart worksheetPart, uint newindex, string startColumn, string endColumn)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row newRow;

            if (sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).Count() != 0)
            {
                newRow = sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).First();
            }
            else
            {
                newRow = new Row() { RowIndex = newindex };
                sheetData.Append(newRow);
            }

            var cells = row.Elements<Cell>();
            var oldcolumns = cells.Select(x => x.CellReference.Value).ToArray();
            var columntmp = oldcolumns.Select(x => GetColumnName(x)).ToList();
            columntmp.RemoveRange(0, columntmp.IndexOf(startColumn));
            columntmp.RemoveRange(columntmp.IndexOf(endColumn) + 1, columntmp.Count - columntmp.IndexOf("L"));
            var newcolumn = columntmp.Select(x => $"{x}{newindex}");
            foreach (var cell in cells)
            {
                string cellreference = $"{GetColumnName(cell.CellReference)}{newindex}";
                if (newcolumn.Contains(cellreference))
                {
                    Cell newCell = new Cell
                    {
                        CellReference = $"{GetColumnName(cell.CellReference)}{newindex}",
                        StyleIndex = cell.StyleIndex,
                        CellValue = new CellValue(cell.CellValue != null ? !string.IsNullOrEmpty(cell.CellValue.InnerText) ? cell.CellValue.InnerText : "" : ""),
                    };

                    newRow.InsertBefore(newCell, null);
                }

            }

            row.CopyMergeCellTo(newRow, worksheetPart, newcolumn);

            worksheet.Save();


            return newRow;
        }

        public static Row CopyRowTo(this Row row, uint newindex)
        {
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;
            Row newRow;

            if (sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).Count() != 0)
            {
                newRow = sheetData.Elements<Row>().Where(r => r.RowIndex == newindex).First();
            }
            else
            {
                newRow = new Row() { RowIndex = newindex };
                sheetData.Append(newRow);
            }

            var cells = row.Elements<Cell>();
            foreach (var cell in cells)
            {
                Cell newCell = new Cell
                {
                    CellReference = $"{GetColumnName(cell.CellReference)}{newindex}",
                    StyleIndex = cell.StyleIndex,
                    CellValue = new CellValue(cell.CellValue != null ? !string.IsNullOrEmpty(cell.CellValue.InnerText) ? cell.CellValue.InnerText : "" : ""),
                };

                newRow.InsertBefore(newCell, null);
            }

            row.CopyMergeCellTo(newRow);

            worksheet.Save();

            return newRow;
        }

        public static MergeCells CopyMergeCellTo(this Row row, Row newRow)
        {
            SheetData sheetData = row.Parent as SheetData;
            Worksheet worksheet = sheetData.Parent as Worksheet;
            var mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            if (mergeCells == null) return null;
            var merges = mergeCells.Elements<MergeCell>().Where(x => x.Reference.Value.Contains(row.RowIndex)).ToList();

            foreach (var merge in merges)
            {
                string newReference = merge.Reference.Value.Replace(row.RowIndex, newRow.RowIndex);
                MergeCell mergeCell = new MergeCell() { Reference = new StringValue(newReference) };
                mergeCells.Append(mergeCell);

                worksheet.Save();
            }

            return mergeCells;
        }

        public static MergeCells CopyMergeCellTo(this Row row, Row newRow, WorksheetPart worksheetPart)
        {
            SheetData sheetDatatemplate = row.Parent as SheetData;
            Worksheet worksheettemplate = sheetDatatemplate.Parent as Worksheet;
            var mergeCellstemplate = worksheettemplate.Elements<MergeCells>().First();

            var merges = mergeCellstemplate.Elements<MergeCell>().Where(x => x.Reference.Value.Contains(row.RowIndex));

            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            var mergeCells = worksheet.Elements<MergeCells>().First();


            foreach (var merge in merges)
            {
                string newReference = merge.Reference.Value.Replace(row.RowIndex, newRow.RowIndex);
                MergeCell mergeCell = new MergeCell() { Reference = new StringValue(newReference) };
                mergeCells.Append(mergeCell);

                worksheet.Save();
            }

            return mergeCells;
        }

        public static MergeCells MergeCell(Worksheet worksheet, string reference)
        {
            var mergeCells = worksheet.Elements<MergeCells>().First();
            MergeCell mergeCell = new MergeCell() { Reference = new StringValue(reference) };
            mergeCells.Append(mergeCell);
            worksheet.Save();

            return mergeCells;
        }

        public static MergeCells CopyMergeCellTo(this Row row, Row newRow, WorksheetPart worksheetPart, IEnumerable<string> column)
        {
            SheetData sheetDatatemplate = row.Parent as SheetData;
            Worksheet worksheettemplate = sheetDatatemplate.Parent as Worksheet;
            var mergeCellstemplate = worksheettemplate.Elements<MergeCells>().First();

            var merges = mergeCellstemplate.Elements<MergeCell>().Where(x => x.Reference.Value.Contains(row.RowIndex) & column.Contains(x.Reference.Value));

            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            var mergeCells = worksheet.Elements<MergeCells>().First();


            foreach (var merge in merges)
            {
                string newReference = merge.Reference.Value.Replace(row.RowIndex, newRow.RowIndex);
                MergeCell mergeCell = new MergeCell() { Reference = new StringValue(newReference) };
                mergeCells.Append(mergeCell);

                worksheet.Save();
            }

            return mergeCells;
        }

        public static MemoryStream CreateNewTableExcel(DataTable table, string sheetName = "Sheet1")
        {
            var stream = new MemoryStream();

            try
            {
                using (var spreadsheet = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = spreadsheet.AddWorkbookPart();

                    spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    spreadsheet.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                    var worksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = spreadsheet.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (System.Data.DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                    // Add table

                    TableDefinitionPart tableDefinitionPart = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
                    GenerateTablePartContent(tableDefinitionPart, table);

                    TableParts tableParts1 = new TableParts() { Count = (UInt32Value)1U };
                    TablePart tablePart1 = new TablePart() { Id = "rId1" };

                    tableParts1.Append(tablePart1);

                    worksheetPart.Worksheet.Append(tableParts1);
                    //#region Add Shared strings part
                    //SharedStringTablePart sharedStringTablePart1 = spreadsheet.WorkbookPart.AddNewPart<SharedStringTablePart>("rId6");
                    //GenerateSharedStringPartContent(sharedStringTablePart1, table);

                    spreadsheet.WorkbookPart.Workbook.Save();

                }
            }
            catch
            {

            }

            return stream;
        }

        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private static IList<string> GenerateTablePartContent(TableDefinitionPart part, DataTable table, string tableName = "Table1")
        {
            string reference = $"A1:{GetExcelColumnName(table.Columns.Count)}{table.Rows.Count + 1}"; //A1:D4
            Table tableTMP = new Table() { Id = (UInt32Value)1U, Name = tableName, DisplayName = tableName, Reference = reference, TotalsRowShown = false };
            AutoFilter autoFilterTMP = new AutoFilter() { Reference = reference };

            TableColumns tableColumnTMP = new TableColumns();
            //header of table
            int colIndex = 1;
            IList<string> res = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                tableColumnTMP.Append(new TableColumn() { Id = Convert.ToUInt32(colIndex), Name = col.ColumnName });//Name = col.ColumnName
                res.Add(col.ColumnName);
                colIndex++;
            }

            TableStyleInfo tableStyleInfoTMP = new TableStyleInfo() { Name = "TableStyleMedium2", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };
            tableTMP.Append(autoFilterTMP);
            tableTMP.Append(tableColumnTMP);
            tableTMP.Append(tableStyleInfoTMP);
            //tableTMP.HeaderRowCount = 1;
            part.Table = tableTMP;

            return res;
        }

        private static string CellIndex(int index)
        {
            string cell;
            if (index < 26)
            {
                cell = Convert.ToString(Convert.ToChar(65 + index));
            }
            else if (index >= 26 && index < 52)
            {
                cell = "A" + Convert.ToString(Convert.ToChar(65 + index - 26));
            }
            else if (index >= 52 && index < 78)
            {
                cell = "B" + Convert.ToString(Convert.ToChar(65 + index - 52));
            }
            else if (index >= 78 && index < 90)
            {
                cell = "C" + Convert.ToString(Convert.ToChar(65 + index - 78));
            }
            else
            {
                cell = Convert.ToString(Convert.ToChar(65 + index));
            }
            return cell;
        }

        private static Cell CreateCells(string header, int row, string text, CellValues dataType)
        {
            Cell c = null;
            CellValue cellval;
            switch (dataType)
            {
                case CellValues.SharedString:
                    c = new Cell { CellReference = header + row, DataType = CellValues.SharedString };
                    cellval = new CellValue { Text = text };
                    c.Append(cellval);
                    break;
                case CellValues.Number:
                    c = new Cell { CellReference = header + row, DataType = CellValues.Number };
                    cellval = new CellValue { Text = text };
                    c.Append(cellval);
                    break;
            }
            return c;
        }

        public static SpreadsheetDocument RemoveSheet(SpreadsheetDocument spreadsheet, Sheet sheet)
        {
            var workbookPart = spreadsheet.WorkbookPart;

            // Remove the sheet reference from the workbook.
            var worksheetPart = (WorksheetPart)(workbookPart.GetPartById(sheet.Id));
            sheet.Remove();

            // Delete the worksheet part.
            workbookPart.DeletePart(worksheetPart);

            return spreadsheet;
        }

        private static Cell CreateCells(Cell cell, string header, int row, string text, CellValues dataType)
        {
            Cell c = null;
            CellValue cellval;

            switch (dataType)
            {
                case CellValues.SharedString:
                    c = new Cell { CellReference = header + row, DataType = CellValues.SharedString };
                    cellval = new CellValue { Text = text };
                    c.Append(cellval);
                    break;
                case CellValues.Number:
                    c = new Cell { CellReference = header + row, DataType = CellValues.Number };
                    cellval = new CellValue { Text = text };
                    c.Append(cellval);
                    break;
            }
            if (cell != null)
            {
                c.StyleIndex = cell.StyleIndex;
            }
            return c;
        }

        /// <summary>
        /// [Sampled from http://www.codeplex.com/PowerTools]
        /// Gets the column Id for a given column number
        /// </summary>
        /// <param name="columnNumber">Column number</param>
        /// <returns>Column Id</returns>
        public static string GetColumnAlpha(int columnNumber)
        {
            int alpha = (int)'Z' - (int)'A' + 1;
            if (columnNumber <= alpha)
                return ((char)((int)'A' + columnNumber - 1)).ToString();
            else
                return
                    GetColumnAlpha(
                        (int)((columnNumber - 1) / alpha)
                    ) +
                    (
                        (char)(
                            (int)'A' + (int)((columnNumber - 1) % alpha)
                        )
                    ).ToString();
        }

        /// <summary>
        /// [Sampled from http://www.codeplex.com/PowerTools]
        /// Returns the column number from the cell reference received as parameter
        /// </summary>
        /// <param name="cellReference">Cell reference to obtain the column number from</param>
        /// <param name="row">Row containing the cell to obtain the column number from</param>
        /// <returns></returns>
        private static int GetColumnNumberFromColumnId(string columnId)
        {
            int columnNumber = 0;
            //Removing row number from cell reference
            int charPosition = 1;
            int charValue = 0;
            foreach (char c in columnId)
            {
                //Getting the Unicode value for the current char in cell reference
                charValue = System.Convert.ToInt32(c);
                if (charPosition < columnId.Length)
                {   //we have not reached the last character in cell reference
                    //we need to multiply the charValue (from 0 to 25) by 26 and add the result of powering 26 to current char position in cell reference
                    //65 is the Unicode value for "A" letter
                    //26 is the number of letters in English alphabet
                    columnNumber += (((charValue - 65) * 26) + (System.Convert.ToInt32(Math.Pow(26, charPosition++))));
                }
                else
                {   //This is the last character in cell reference
                    columnNumber += (charValue - 64);
                }
            }
            return columnNumber;
        }
        public static bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public static string GetColumnName(string p)
        {
            Boolean breakLoop = true;
            while (breakLoop)
            {
                if (isNumeric(p.Last().ToString(), System.Globalization.NumberStyles.Integer))
                    p = p.Substring(0, p.Length - 1);
                else
                    breakLoop = false;
            }
            return p;
        }

        public static void GenerateSharedStringPartContent(SharedStringTablePart part, DataTable table)
        {
            SharedStringTable sharedStringTableTMP = new SharedStringTable() { Count = Convert.ToUInt32(table.Rows.Count), UniqueCount = Convert.ToUInt32(table.Rows.Count) };
            //SharedStringItem sharedStringItem2 = new SharedStringItem();
            //Text text2 = new Text();
            //text2.Text = "Column2";
            //sharedStringItem2.Append(text2);
            //SharedStringItem sharedStringItem3 = new SharedStringItem();
            //Text text3 = new Text();
            //text3.Text = "Column3";
            //sharedStringItem3.Append(text3);
            //SharedStringItem sharedStringItem4 = new SharedStringItem();
            //Text text4 = new Text();
            //text4.Text = "Column4";
            //sharedStringItem4.Append(text4);
            //sharedStringTableTMP.Append(new SharedStringItem(new SharedStringItem(new Text("Comumn1"))));
            //// sharedStringTable1.Append(sharedStringItem1);
            //sharedStringTableTMP.Append(sharedStringItem2);
            //sharedStringTableTMP.Append(sharedStringItem3);
            //sharedStringTableTMP.Append(sharedStringItem4);
            //part.SharedStringTable = sharedStringTableTMP;

            foreach (DataColumn col in table.Columns)
            {
                SharedStringItem sharedStringItemTMP = new SharedStringItem();
                Text textTMP = new Text();
                textTMP.Text = col.ColumnName;
                sharedStringItemTMP.Append(textTMP);
                sharedStringTableTMP.Append(sharedStringItemTMP);
            }
        }

        // Get Picture from WorkSheet
        public static string GetPicWorkSheet(WorkbookPart wbPart, string sheetName, string path, string serverPath)
        {
            string pdfPath = "";
            try
            {
                Sheet sheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).First();
                WorksheetPart worksheetPart = wbPart.GetPartById(sheet.Id) as WorksheetPart;
                var workSheet = wbPart.WorksheetParts.FirstOrDefault();
                List<string> imageList = new List<string>();
                int dem = 1;
                foreach (ImagePart i in worksheetPart.DrawingsPart.GetPartsOfType<ImagePart>())
                {
                    ImagePart imagePart = i;
                    string imageFileName = string.Empty;
                    using (System.Drawing.Image toSaveImage = Bitmap.FromStream(imagePart.GetStream()))
                    {
                        imageFileName = path + sheetName + "_" + dem + ".png";
                        try
                        {
                            toSaveImage.Save(imageFileName, ImageFormat.Png);
                            imageList.Add(imageFileName);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            toSaveImage.Dispose();
                        }
                    }
                    dem++;
                }
                //pdfPath = InsertImageIntoPDF(path, sheetName, serverPath, imageList);
                return pdfPath;
            }
            catch
            {
                return "";
            }
        }

        // Insert Image into PDF
        public static string InsertImageIntoPDF(string path, string sheetName, string serverPath, List<string> imageList)
        {
            //string result = "";
            //Document doc = new Document(PageSize.A3, 10f, 10f, 100f, 0f);
            //string pdfFilePath = path;
            //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + sheetName + ".pdf", FileMode.Create));
            //doc.Open();
            //try
            //{
            //    Paragraph paragraph = new Paragraph();
            //    foreach (var item in imageList)
            //    {
            //        string imageURL = item;
            //        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            //        var height = jpg.Height;
            //        var width = jpg.Width;
            //        jpg.Alignment = Element.ALIGN_LEFT;
            //        jpg.ScaleToFit(800, 800 * height / width);
            //        doc.Add(paragraph);
            //        doc.Add(jpg);
            //    }

            //    result = serverPath + sheetName + ".pdf";
            //}
            //catch (Exception)
            //{ }
            //finally
            //{
            //    doc.Close();
            //}

            //return result;
            return null;

        }

        public static string InsertImageToExcel(WorkbookPart wbp, WorksheetPart wsp, Worksheet ws, string fullpath)
        {
            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            //  string sImagePath = "polymathlogo.png";
            var dp = wsp.GetPartsOfType<DrawingsPart>().FirstOrDefault();
            if (dp == null)
            {
                dp = wsp.AddNewPart<DrawingsPart>();
            }
            //DrawingsPart dp = wsp.AddNewPart<DrawingsPart>();
            //var imgp = dp.GetPartsOfType<ImagePart>().FirstOrDefault();
            //if (dp == null)
            //{
            //    imgp = dp.AddImagePart(ImagePartType.Png, wsp.GetIdOfPart(dp));
            //}
            int ImagePartCount = dp.GetPartsOfType<ImagePart>().Count();
            ImagePart imgp = dp.AddImagePart(ImagePartType.Png, $"rid{++ImagePartCount}");
            using (FileStream fs = new FileStream(fullpath, FileMode.Open))
            {
                imgp.FeedData(fs);
            }

            NonVisualDrawingProperties nvdp = new NonVisualDrawingProperties();
            nvdp.Id = 1025;
            nvdp.Name = "Picture 1";
            //  nvdp.Description = "polymathlogo";
            DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
            picLocks.NoChangeAspect = true;
            picLocks.NoChangeArrowheads = true;
            NonVisualPictureDrawingProperties nvpdp = new NonVisualPictureDrawingProperties();
            nvpdp.PictureLocks = picLocks;
            NonVisualPictureProperties nvpp = new NonVisualPictureProperties();
            nvpp.NonVisualDrawingProperties = nvdp;
            nvpp.NonVisualPictureDrawingProperties = nvpdp;

            DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
            stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

            BlipFill blipFill = new BlipFill();
            DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
            blip.Embed = dp.GetIdOfPart(imgp);
            blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
            blipFill.Blip = blip;
            blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
            blipFill.Append(stretch);

            DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
            DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
            offset.X = 0;
            offset.Y = 0;
            t2d.Offset = offset;
            Bitmap bm = new Bitmap(fullpath);
            //http://en.wikipedia.org/wiki/English_Metric_Unit#DrawingML
            //http://stackoverflow.com/questions/1341930/pixel-to-centimeter
            //http://stackoverflow.com/questions/139655/how-to-convert-pixels-to-points-px-to-pt-in-net-c
            DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
            extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
            extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
            bm.Dispose();
            t2d.Extents = extents;
            ShapeProperties sp = new ShapeProperties();
            sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
            sp.Transform2D = t2d;
            DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
            prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
            prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
            sp.Append(prstGeom);
            sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

            DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
            picture.NonVisualPictureProperties = nvpp;
            picture.BlipFill = blipFill;
            picture.ShapeProperties = sp;

            Position pos = new Position();
            pos.X = 0;
            pos.Y = 0;
            Extent ext = new Extent();
            ext.Cx = extents.Cx;
            ext.Cy = extents.Cy;
            AbsoluteAnchor anchor = new AbsoluteAnchor();
            anchor.Position = pos;
            anchor.Extent = ext;
            anchor.Append(picture);
            anchor.Append(new ClientData());
            WorksheetDrawing wsd = new WorksheetDrawing();
            wsd.Append(anchor);
            Drawing drawing = new Drawing();
            drawing.Id = dp.GetIdOfPart(imgp);

            wsd.Save(dp);

            //ws.Append(sheetData);
            ws.Append(drawing);
            wsp.Worksheet.Save();
            return "";
        }

        // Insert image into file Excel
        public static void InsertImage(Worksheet ws, long x, long y, long? width, long? height, string sImagePath, string type)
        {
            try
            {
                WorksheetPart wsp = ws.WorksheetPart;
                DrawingsPart dp;
                ImagePart imgp;
                ImagePart imgp2;
                WorksheetDrawing wsd;

                ImagePartType ipt;
                switch (sImagePath.Substring(sImagePath.LastIndexOf('.') + 1).ToLower())
                {
                    case "png":
                        ipt = ImagePartType.Png;
                        break;
                    case "jpg":
                    case "jpeg":
                        ipt = ImagePartType.Jpeg;
                        break;
                    case "gif":
                        ipt = ImagePartType.Gif;
                        break;
                    default:
                        return;
                }

                if (wsp.DrawingsPart == null)
                {
                    //----- no drawing part exists, add a new one
                    dp = wsp.AddNewPart<DrawingsPart>();
                    imgp = dp.AddImagePart(ipt, wsp.GetIdOfPart(dp));
                    wsd = new WorksheetDrawing();
                    imgp2 = imgp;
                }
                else
                {
                    //----- use existing drawing part
                    dp = wsp.DrawingsPart;
                    imgp = dp.AddImagePart(ipt);
                    var lstImage = dp.GetPartsOfType<ImagePart>().ToList(); // list default image in file Excel
                    imgp2 = lstImage[1];
                    switch (type)
                    {
                        case "sp_SQE_6Panel_PPM_By_Month": // Quality
                            imgp2 = lstImage[4];
                            break;
                        case "sp_SQE_6Panel_OTD_By_Month": // Delivery
                            imgp2 = lstImage[3];
                            break;
                        case "sp_SQE_6Panel_TopDefectPart_By_Month": // Top Quantity of Month by Part
                            imgp2 = lstImage[0];
                            break;
                        case "sp_SQE_6Panel_CustClaim_By_Month": // Customer Claim
                            imgp2 = lstImage[5];
                            break;
                        case "sp_SQE_6Panel_Freight_By_Month": // Freight
                            imgp2 = lstImage[2];
                            break;
                        case "sp_SQE_6Panel_TopDefectPart_By_YTD": // Top Quantity of YTD by Part
                            imgp2 = lstImage[1];
                            break;
                        default:
                            break;
                    }
                    dp.CreateRelationshipToPart(imgp);
                    wsd = dp.WorksheetDrawing;

                }
                using (FileStream fs = new FileStream(sImagePath, FileMode.Open))
                {
                    imgp2.FeedData(fs);
                }

                wsd.Save(dp);
            }
            catch (Exception ex)
            {
                throw ex; // or do something more interesting if you want
            }
        }

        // Delete file
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        // Quan add 2020/08/25
        // ReadExcelFile convert to json datatable using OpenXML
        public static string ReadExcelFile(string UrlFile)
        {
            try
            {
                DataTable dtTable = new DataTable();
                //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(@"C:\Users\Tuan\Desktop\Raw_Data_20200803\111111.xlsx", false))
                {
                    //create the object for workbook part  
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();

                    //using for each loop to get the sheet from the sheetcollection  
                    foreach (Sheet thesheet in thesheetcollection.OfType<Sheet>())
                    {
                        //statement to get the worksheet object by using the sheet id  
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                        SheetData thesheetdata = theWorksheet.GetFirstChild<SheetData>();

                        for (int rCnt = 0; rCnt < thesheetdata.ChildElements.Count(); rCnt++)
                        {
                            List<string> rowList = new List<string>();
                            for (int rCnt1 = 0; rCnt1 < thesheetdata.ElementAt(rCnt).ChildElements.Count(); rCnt1++)
                            {
                                Cell thecurrentcell = (Cell)thesheetdata.ElementAt(rCnt).ChildElements.ElementAt(rCnt1);

                             
                                Cell cell = theWorksheet.WorksheetPart.Worksheet.Descendants<Cell>(). Where(c => c.CellReference == UrlFile).FirstOrDefault();

                                //statement to take the integer value  
                                string currentcellvalue = string.Empty;
                               //var type = thecurrentcell.GetCellValue();
                              

                                if (thecurrentcell.DataType != null)
                                {
                                    if (thecurrentcell.DataType == CellValues.SharedString)
                                    {
                                        int id;
                                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                                        {
                                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                            if (item.Text != null)
                                            {
                                                //first row will provide the column name.
                                                if (rCnt == 0)
                                                {
                                                    dtTable.Columns.Add(item.Text.Text);
                                                }
                                                else
                                                {
                                                    rowList.Add(item.Text.Text);
                                                }
                                            }
                                            else if (item.InnerText != null)
                                            {
                                                currentcellvalue = item.InnerText;
                                            }
                                            else if (item.InnerXml != null)
                                            {
                                                currentcellvalue = item.InnerXml;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (rCnt != 0)//reserved for column values
                                    {
                                        rowList.Add(thecurrentcell.InnerText);
                                    }
                                }
                            }
                            if (rCnt != 0)//reserved for column values
                                dtTable.Rows.Add(rowList.ToArray());

                        }

                    }
                   
                    return JsonConvert.SerializeObject(dtTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static DataTable ReadExcelSheet(string fname, bool firstRowIsHeader)
        {
            List<string> Headers = new List<string>();
            DataTable dt = new DataTable();
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fname, false))
            {
                //Read the first Sheets 
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                int counter = 0;
                foreach (Row row in rows)
                {
                    counter = counter + 1;
                    //Read the first row as header
                    if (counter == 1)
                    {
                        var j = 1;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            var colunmName = firstRowIsHeader ? GetCellValue(doc, cell) : "Field" + j++;
                            Console.WriteLine(colunmName);
                            Headers.Add(colunmName);
                            dt.Columns.Add(colunmName);
                        }
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            if(i < dt.Columns.Count)
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = GetCellValue(doc, cell);
                                i++;
                            }
                        }
                    }
                }

            }
            return dt;
        }

        public static string GetCellValue(SpreadsheetDocument doc, Cell cell)
        {
            if (cell.CellValue == null)
            {
                return "";
            }
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }
        //Test

        

    }
    public static class OpenXMLExtension
    {
        public static string GetCellValue(this CellValue value)
        {
            return string.IsNullOrEmpty(value.InnerText) ? "" : value.InnerText;
        }

        public static string GetCellValue(this Cell value)
        {
            return string.IsNullOrEmpty(value.CellValue.InnerText) ? "" : value.CellValue.InnerText;
        }
    }
}


