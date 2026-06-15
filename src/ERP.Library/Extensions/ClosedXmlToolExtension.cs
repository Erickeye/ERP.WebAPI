using System.Collections;
using System.Reflection;
using ClosedXML.Excel;
using ERP.Library.ViewModels;

namespace ERP.Library.Extensions
{
    public enum ClosedXmlSaveFormat
    {
        Xlsx = 6
    }

    public static class ClosedXmlToolExtension
    {
        public static FileModel ConvertToFileModel(this XLWorkbook workbook, ClosedXmlSaveFormat saveFormat, string fileName)
        {
            if (saveFormat != ClosedXmlSaveFormat.Xlsx)
            {
                throw new NotSupportedException("ClosedXML only supports xlsx export in this project.");
            }

            using var fileStream = new MemoryStream();
            workbook.SaveAs(fileStream);

            var excelBytes = fileStream.ToArray();
            var base64String = Convert.ToBase64String(excelBytes);
            const string extension = "xlsx";

            return new FileModel
            {
                DataUrl = base64String.Base64StrToDataUrl(extension),
                FileName = $"{fileName}.{extension}"
            };
        }

        public static XLWorkbook BindData<T>(this XLWorkbook workbook, T data)
        {
            foreach (var worksheet in workbook.Worksheets)
            {
                BindWorksheet(worksheet, data);
            }

            return workbook;
        }

        private static void BindWorksheet<T>(IXLWorksheet worksheet, T data)
        {
            var usedRange = worksheet.RangeUsed();
            if (usedRange == null)
            {
                return;
            }

            var markers = usedRange.Cells()
                .Select(cell => new TemplateMarker(cell, cell.GetString()))
                .Where(marker => marker.IsMarker)
                .ToList();

            var listMarkers = markers
                .Where(marker => marker.TableName != null && marker.FieldName != null)
                .GroupBy(marker => new
                {
                    Row = marker.Cell.Address.RowNumber,
                    Table = marker.TableName!
                })
                .OrderByDescending(group => group.Key.Row)
                .ToList();

            foreach (var group in listMarkers)
            {
                BindListRow(worksheet, group.Key.Row, group.Key.Table, group.ToList(), data);
            }

            foreach (var marker in markers.Where(marker => marker.TableName == null && marker.FieldName != null))
            {
                SetCellValue(marker.Cell, GetPropertyValue(data, marker.FieldName!));
            }
        }

        private static void BindListRow<T>(
            IXLWorksheet worksheet,
            int templateRowNumber,
            string tableName,
            IReadOnlyCollection<TemplateMarker> markers,
            T data)
        {
            var rows = GetEnumerableProperty(data, tableName).Cast<object?>().ToList();

            if (rows.Count == 0)
            {
                foreach (var marker in markers)
                {
                    marker.Cell.Clear(XLClearOptions.Contents);
                }

                return;
            }

            if (rows.Count > 1)
            {
                worksheet.Row(templateRowNumber).InsertRowsBelow(rows.Count - 1);
            }

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var currentRowNumber = templateRowNumber + rowIndex;

                foreach (var marker in markers)
                {
                    var cell = worksheet.Cell(currentRowNumber, marker.Cell.Address.ColumnNumber);
                    SetCellValue(cell, GetPropertyValue(rows[rowIndex], marker.FieldName!));
                }
            }
        }

        private static IEnumerable GetEnumerableProperty<T>(T data, string propertyName)
        {
            var value = GetPropertyValue(data, propertyName);
            return value as IEnumerable ?? Array.Empty<object>();
        }

        private static object? GetPropertyValue(object? data, string propertyName)
        {
            return data?.GetType()
                .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                ?.GetValue(data);
        }

        private static void SetCellValue(IXLCell cell, object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.Clear(XLClearOptions.Contents);
                return;
            }

            switch (value)
            {
                case string stringValue:
                    cell.Value = stringValue;
                    break;
                case DateTime dateTimeValue:
                    cell.Value = dateTimeValue;
                    break;
                case bool boolValue:
                    cell.Value = boolValue;
                    break;
                case byte byteValue:
                    cell.Value = byteValue;
                    ApplyNumberFormat(cell);
                    break;
                case short shortValue:
                    cell.Value = shortValue;
                    ApplyNumberFormat(cell);
                    break;
                case int intValue:
                    cell.Value = intValue;
                    ApplyNumberFormat(cell);
                    break;
                case long longValue:
                    cell.Value = longValue;
                    ApplyNumberFormat(cell);
                    break;
                case float floatValue:
                    cell.Value = floatValue;
                    ApplyNumberFormat(cell);
                    break;
                case double doubleValue:
                    cell.Value = doubleValue;
                    ApplyNumberFormat(cell);
                    break;
                case decimal decimalValue:
                    cell.Value = decimalValue;
                    ApplyNumberFormat(cell);
                    break;
                default:
                    cell.Value = value.ToString();
                    break;
            }
        }

        private static void ApplyNumberFormat(IXLCell cell)
        {
            cell.Style.NumberFormat.Format = "#,##0.##";
        }

        private sealed class TemplateMarker
        {
            private const string Prefix = "&=";

            public TemplateMarker(IXLCell cell, string text)
            {
                Cell = cell;
                Text = text?.Trim() ?? string.Empty;

                if (!Text.StartsWith(Prefix, StringComparison.Ordinal))
                {
                    return;
                }

                IsMarker = true;

                var path = Text[Prefix.Length..];
                var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parts.Length == 1)
                {
                    FieldName = parts[0];
                }
                else if (parts.Length == 2)
                {
                    TableName = parts[0];
                    FieldName = parts[1];
                }
            }

            public IXLCell Cell { get; }
            public string Text { get; }
            public bool IsMarker { get; }
            public string? TableName { get; }
            public string? FieldName { get; }
        }
    }
}
