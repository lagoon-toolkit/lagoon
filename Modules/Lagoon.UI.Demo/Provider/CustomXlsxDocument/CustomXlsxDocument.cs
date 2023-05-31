using ClosedXML.Excel;

namespace Lagoon.UI.Demo.Provider.CustomXlsxDocument;

/// <summary>
/// Custom XlsxDocument for example
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomXlsxDocument<T>: XlsxDocument<T>
{
    ///<inheritdoc/>
    public CustomXlsxDocument(XlsxOptions options = null, ExportColumnCollection<T> exportColumns = null)
        : base(options, exportColumns)
    {
    }

    #region GetByteArray

    ///<inheritdoc/>
    public static new byte[] SerializeToByteArray(IEnumerable<T> items, ExportColumnCollection<T> exportColumns = null, XlsxOptions options = null)
    {
        using (MemoryStream stream = new())
        {
            using (CustomXlsxDocument<T> XlsxSerialized = new(options, exportColumns))
            {
                // Write Xslx content to memory
                XlsxSerialized.Serialize(items, stream, true);
                // Return the file content
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }

    #endregion

    ///<inheritdoc/>
    protected override void AddLine(int lineIndex)
    {
        int columnIndex = 1;
        // Loop on each fields
        foreach (string value in Fields)
        {
            //Add border and color for Title row
            if (lineIndex == 1)
            {
                Worksheet.Cell(lineIndex, columnIndex).Style.Fill.BackgroundColor = XLColor.Gray;
                Worksheet.Cell(lineIndex, columnIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            //Add color for even line
            if (lineIndex > 1 && lineIndex % 2 == 0)
            {
                Worksheet.Cell(lineIndex, columnIndex).Style.Fill.BackgroundColor = XLColor.AliceBlue;
            }
            Worksheet.Cell(lineIndex, columnIndex).Value = $"{value}";
            columnIndex++;
        }
        // Clear buffer
        Fields.Clear();
    }
}