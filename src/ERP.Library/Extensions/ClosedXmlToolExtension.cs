using System.Collections;
using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// 指定xlsx套版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="workbook"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XLWorkbook BindData<T>(this XLWorkbook workbook, T data)
        {
            foreach (var worksheet in workbook.Worksheets)
            {
                BindWorksheet(worksheet, data);
            }

            return workbook;
        }
        /// <summary>
        /// 共用xlsx模板套版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="workbook"></param>
        /// <param name="rows"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static XLWorkbook BindQueryTable<T>(this XLWorkbook workbook, IEnumerable<T> rows, string title)
        {
            var worksheet = workbook.Worksheets.First();
            var columns = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(x => x.MetadataToken)
                .ToList();

            PrepareQueryTableTemplate(worksheet, columns, title);

            return workbook.BindData(new QueryTableData<T>
            {
                Items = rows
            });
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

        private static void PrepareQueryTableTemplate(IXLWorksheet worksheet, IReadOnlyList<PropertyInfo> columns, string title)
        {
            const int systemTitleRowNumber = 1;
            const int reportTitleRowNumber = 2;
            const int headerRowNumber = 3;
            const int templateRowNumber = 4;

            if (columns.Count == 0)
            {
                return;
            }

            var templateLastColumnNumber = worksheet.Row(headerRowNumber)
                .CellsUsed(XLCellsUsedOptions.AllFormats)
                .LastOrDefault()
                ?.Address.ColumnNumber ?? columns.Count;
            var lastColumnNumber = Math.Max(columns.Count, templateLastColumnNumber);
            var headerTemplateCell = worksheet.Cell(headerRowNumber, templateLastColumnNumber);
            var bodyTemplateCell = worksheet.Cell(templateRowNumber, templateLastColumnNumber);

            worksheet.MergedRanges.ToList().ForEach(range => range.Unmerge());
            worksheet.Range(systemTitleRowNumber, 1, systemTitleRowNumber, columns.Count).Merge();
            worksheet.Range(reportTitleRowNumber, 1, reportTitleRowNumber, columns.Count).Merge();
            worksheet.Cell(reportTitleRowNumber, 1).Value = title;

            worksheet.Range(headerRowNumber, 1, templateRowNumber, lastColumnNumber).Clear(XLClearOptions.Contents);

            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                var columnNumber = columnIndex + 1;
                var column = columns[columnIndex];
                var headerCell = worksheet.Cell(headerRowNumber, columnNumber);
                var bodyCell = worksheet.Cell(templateRowNumber, columnNumber);

                if (columnNumber > templateLastColumnNumber)
                {
                    headerCell.Style = headerTemplateCell.Style;
                    bodyCell.Style = bodyTemplateCell.Style;
                    worksheet.Column(columnNumber).Width = worksheet.Column(templateLastColumnNumber).Width;
                }

                headerCell.Value = GetDisplayName(column);
                bodyCell.Value = $"&=Items.{column.Name}";
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
                    ApplyNumberFormat(cell, floatValue);
                    break;
                case double doubleValue:
                    cell.Value = doubleValue;
                    ApplyNumberFormat(cell, doubleValue);
                    break;
                case decimal decimalValue:
                    cell.Value = decimalValue;
                    ApplyNumberFormat(cell, decimalValue);
                    break;
                default:
                    cell.Value = value.ToString();
                    break;
            }
        }

        private static void ApplyNumberFormat(IXLCell cell)
        {
            cell.Style.NumberFormat.Format = "#,##0";
        }

        private static void ApplyNumberFormat(IXLCell cell, decimal value)
        {
            cell.Style.NumberFormat.Format = decimal.Truncate(value) == value ? "#,##0" : "#,##0.##";
        }

        private static void ApplyNumberFormat(IXLCell cell, double value)
        {
            cell.Style.NumberFormat.Format = Math.Truncate(value) == value ? "#,##0" : "#,##0.##";
        }

        private static void ApplyNumberFormat(IXLCell cell, float value)
        {
            ApplyNumberFormat(cell, (double)value);
        }

        private static string GetDisplayName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
        }

        private sealed class QueryTableData<T>
        {
            public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
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
