using System.Collections;
using System.Data;
using ERP.Library.ViewModels;
using Aspose.Cells;
using Aspose.Words;

namespace ERP.Library.Extensions
{
    public static class AsposeToolExtension
    {
        /// <summary>
        /// 轉換為 檔案-模型
        /// </summary>
        /// <param name="workbook">Workbook 物件</param>
        /// <param name="saveFormat">儲存格式</param>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public static FileModel ConvertToFileModel(this Workbook workbook, Aspose.Cells.SaveFormat saveFormat, string fileName)
        {
            using (var fileStream = new System.IO.MemoryStream())
            {
                workbook.Save(fileStream, saveFormat);
                fileStream.Position = 0;

                byte[] excelBytes = fileStream.ToArray();
                string base64String = Convert.ToBase64String(excelBytes);
                var extension = saveFormat.ToString().ToLower();
                string dataUrl = base64String.Base64StrToDataUrl(extension);

                FileModel result = new FileModel
                {
                    DataUrl = dataUrl,
                    FileName = $"{fileName}.{extension}"
                };

                return result;
            }
        }

        /// <summary>
        /// 轉換為 檔案-模型
        /// </summary>
        /// <param name="doc">doc 物件</param>
        /// <param name="saveFormat">儲存格式</param>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public static FileModel ConvertToFileModel(this Document doc, Aspose.Words.SaveFormat saveFormat, string fileName)
        {
            using (var fileStream = new System.IO.MemoryStream())
            {
                doc.Save(fileStream, saveFormat);
                fileStream.Position = 0;

                byte[] wordBytes = fileStream.ToArray();
                string base64String = Convert.ToBase64String(wordBytes);
                var extension = saveFormat.ToString().ToLower();
                string dataUrl = base64String.Base64StrToDataUrl(extension);

                FileModel result = new FileModel
                {
                    DataUrl = dataUrl,
                    FileName = $"{fileName}.{extension}"
                };

                return result;
            }
        }

        /// <summary>
        /// 把資料合併到 workBook 中同名的㯗位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Aspose.Cells.Workbook BindData<T>(this Aspose.Cells.Workbook workBook, T data)
        {
            var designer = new Aspose.Cells.WorkbookDesigner();
            designer.Workbook = workBook;

            var dataSet = new DataSet();

            var table = GetBindDataSingleField(data);
            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0];
                foreach (DataColumn column in table.Columns)
                {
                    designer.SetDataSource(column.ColumnName, row[column.ColumnName]);
                }
            }

            var listTables = GetBindDataListField(data);
            dataSet.Tables.AddRange(listTables.ToArray());
            designer.SetDataSource(dataSet);
            designer.Process(true);

            return workBook;
        }
        /// <summary>
        /// 取得資料中的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<DataTable> GetBindDataListField<T>(T data)
        {
            var tables = new List<DataTable>();
            var listTypeFields = data?.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.GetValue(data, null) is IList).ToList();
            if (listTypeFields != null)
            {
                foreach (var item in listTypeFields) // 每一個類型為 List<T> 的成員
                {
                    var table = new DataTable
                    {
                        TableName = item.Name
                    };
                    var type = item.PropertyType.GetGenericArguments().Single();
                    var columnFieldList = type.GetProperties().Select(x => new { x.Name, x.PropertyType }).ToList();
                    foreach (var column in columnFieldList) // 取得成員中 List 所包覆類型的屬性。ex: List<ViewType>，取得 ViewType 所有屬性
                    {
                        table.Columns.Add(column.Name, Nullable.GetUnderlyingType(column.PropertyType) ?? column.PropertyType);
                    }

                    var rowDataSource = (IList?)item.GetValue(data, null);

                    if (rowDataSource != null)
                    {
                        foreach (var rowDataItemObj in rowDataSource)
                        {
                            var row = table.NewRow();
                            var rowDataItem = Convert.ChangeType(rowDataItemObj, type);
                            var dataMapping = rowDataItem.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(rowDataItem, null)).ToList();
                            foreach (var fieldData in dataMapping) // 取得
                            {
                                if (fieldData.Value != null)
                                {
                                    if (columnFieldList.Any(x => x.Name == fieldData.Key))
                                    {
                                        row[fieldData.Key] = fieldData.Value;
                                    }
                                }
                                else
                                {
                                    row[fieldData.Key] = DBNull.Value;
                                }
                            }
                            table.Rows.Add(row);
                        }
                    }
                    tables.Add(table);
                }
            }

            return tables;
        }
        /// <summary>
        /// 取得資料中的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static DataTable GetBindDataSingleField<T>(T data)
        {
            var table = new DataTable();
            var columnFieldList = data?.GetType().GetProperties().Where(p => !(p.PropertyType.IsGenericType && p is IList)).Select(x => new { x.Name, x.PropertyType }).ToList();
            if (columnFieldList != null)
            {
                foreach (var column in columnFieldList)
                {
                    table.Columns.Add(column.Name, Nullable.GetUnderlyingType(column.PropertyType) ?? column.PropertyType);
                }
            }
            var row = table.NewRow();
            var dataMapping = data?.GetType().GetProperties().Select(x => new { x.Name, x.PropertyType, Value = x.GetValue(data, null) }).ToList();
            var columnNameList = columnFieldList?.Select(m => m.Name).ToList();
            if (dataMapping != null)
            {

                foreach (var item in dataMapping)
                {
                    if (columnNameList != null && item.Value != null)
                    {
                        if (columnNameList.Any(x => x == item.Name))
                        {
                            row[item.Name] = item.Value;
                        }
                    }
                    else
                    {
                        row[item.Name] = DBNull.Value;
                    }
                }
            }
            table.Rows.Add(row);

            return table;
        }

        /// <summary>
        /// 把base64字串轉換成對應的dataURL
        /// </summary>
        /// <param name="base64String">檔案的base64字串</param>
        /// <param name="extension">副檔名</param>
        /// <returns></returns>
        public static string Base64StrToDataUrl(this string base64String, string extension)
        {
            var prefix = getDataUrlPrefix(extension);

            return $"{prefix}{base64String}";
        }
        /// <summary>
        /// 設定dataUrl前綴
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string getDataUrlPrefix(string extension)
        {

            switch (extension)
            {
                case "jpg":
                case "jpeg":
                    return "data:image/jpeg;base64,";
                case "png":
                    return "data:image/png;base64,";
                case "pdf":
                    return "data:application/pdf;base64,";
                case "doc":
                    return "data:application/msword;base64,";
                case "docx":
                    return "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,";
                case "odt":
                    return "data:application/vnd.oasis.opendocument.text;base64,";
                case "xls":
                    return "data:application/vnd.ms-excel;base64,";
                case "xlsx":
                    return "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
                case "ods":
                    return "data:application/vnd.oasis.opendocument.spreadsheet;base64,";
                case "ppt":
                    return "data:application/vnd.ms-powerpoint;base64,";
                case "pptx":
                    return "data:application/vnd.openxmlformats-officedocument.presentationml.presentation;base64,";
                case "xml":
                    return "data:application/xml;base64,";
                case "zip":
                    return "data:application/zip;base64,";
                case "txt":
                    return "data:text/plain;base64,";
                default:
                    return "data:application/octet-stream;base64,";
            }
        }
    }
}
