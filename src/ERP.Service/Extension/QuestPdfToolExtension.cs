using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ERP.Library.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Library.Extensions
{
    public static class QuestPdfToolExtension
    {
        public static FileModel ConvertToPdfFileModel<T>(this IEnumerable<T> rows, string fileName, string? title = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var items = rows.ToList();
            var columns = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(x => x.MetadataToken)
                .ToList();

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Microsoft JhengHei"));

                    page.Header()
                        .PaddingBottom(10)
                        .Text(title ?? fileName)
                        .SemiBold()
                        .FontSize(16);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(definition =>
                        {
                            foreach (var _ in columns)
                            {
                                definition.RelativeColumn();
                            }
                        });

                        foreach (var column in columns)
                        {
                            table.Cell()
                                .Element(HeaderCellStyle)
                                .Text(GetDisplayName(column))
                                .SemiBold();
                        }

                        foreach (var item in items)
                        {
                            foreach (var column in columns)
                            {
                                var cell = table.Cell().Element(BodyCellStyle);
                                var value = column.GetValue(item);
                                var text = FormatValue(value);

                                if (IsNumber(column.PropertyType))
                                {
                                    cell.AlignRight().Text(text);
                                }
                                else
                                {
                                    cell.Text(text);
                                }
                            }
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            }).GeneratePdf();

            var base64String = Convert.ToBase64String(pdfBytes);

            return new FileModel
            {
                DataUrl = base64String.Base64StrToDataUrl("pdf"),
                FileName = $"{fileName}.pdf"
            };
        }

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .Border(0.5f)
                .BorderColor(Colors.Grey.Medium)
                .Padding(4);
        }

        private static IContainer BodyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(4);
        }

        private static string GetDisplayName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
        }

        private static string FormatValue(object? value)
        {
            return value switch
            {
                null => string.Empty,
                DateTime dateTime => dateTime.ToString("yyyy/MM/dd"),
                decimal decimalValue => decimalValue.ToString("#,##0.##"),
                double doubleValue => doubleValue.ToString("#,##0.##"),
                float floatValue => floatValue.ToString("#,##0.##"),
                byte byteValue => byteValue.ToString("#,##0"),
                short shortValue => shortValue.ToString("#,##0"),
                int intValue => intValue.ToString("#,##0"),
                long longValue => longValue.ToString("#,##0"),
                _ => value.ToString() ?? string.Empty
            };
        }

        private static bool IsNumber(Type type)
        {
            var valueType = Nullable.GetUnderlyingType(type) ?? type;

            return valueType == typeof(byte)
                || valueType == typeof(short)
                || valueType == typeof(int)
                || valueType == typeof(long)
                || valueType == typeof(float)
                || valueType == typeof(double)
                || valueType == typeof(decimal);
        }
    }
}
