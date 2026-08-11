using ClosedXML.Excel;
using MacrobioticaLaBendicion.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MacrobioticaLaBendicion.Services
// Genera el PDF/Excel del detalle de un pedido ya confirmado, a partir de
// los mismos datos que ya muestra Pedido/Details — no recalcula nada.
{
    public class PedidoExportService
    {
        public byte[] GenerarPdf(Pedido pedido)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Macrobiótica La Bendición").FontSize(16).Bold();
                        col.Item().Text($"Pedido #{pedido.id_Pedido}").FontSize(13);
                        col.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Cliente: ").SemiBold();
                            text.Span(pedido.Cliente?.Nombre ?? "-");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Cédula: ").SemiBold();
                            text.Span(pedido.Cliente?.Cedula ?? "-");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Fecha: ").SemiBold();
                            text.Span(pedido.Fecha.ToString("dd/MM/yyyy HH:mm"));
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Estado: ").SemiBold();
                            text.Span(pedido.Estado);
                        });
                    });

                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.3f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Producto").Bold();
                            header.Cell().Text("Cant.").Bold();
                            header.Cell().Text("Precio unit.").Bold();
                            header.Cell().Text("Desc. %").Bold();
                            header.Cell().Text("Imp. %").Bold();
                            header.Cell().Text("Total línea").Bold();
                            header.Cell().ColumnSpan(6).PaddingTop(3).BorderBottom(1).BorderColor(Colors.Grey.Medium);
                        });

                        foreach (var d in pedido.Detalles)
                        {
                            table.Cell().Text(d.Producto?.Nombre ?? "-");
                            table.Cell().Text(d.Cantidad.ToString());
                            table.Cell().Text($"₡{d.PrecioUnitario:N2}");
                            table.Cell().Text($"{d.Descuento}%");
                            table.Cell().Text($"{d.Porcentaje_Imp}%");
                            table.Cell().Text($"₡{d.Total_Linea:N2}");
                        }
                    });

                    page.Footer().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Subtotal: ₡{pedido.Subtotal:N2}");
                        col.Item().Text($"Impuestos: ₡{pedido.Impuestos:N2}");
                        col.Item().Text($"Total: ₡{pedido.Total:N2}").Bold().FontSize(12);
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GenerarExcel(Pedido pedido)
        {
            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add($"Pedido {pedido.id_Pedido}");

            hoja.Cell(1, 1).Value = "Macrobiótica La Bendición";
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 14;

            hoja.Cell(2, 1).Value = $"Pedido #{pedido.id_Pedido}";
            hoja.Cell(3, 1).Value = "Cliente:";
            hoja.Cell(3, 2).Value = pedido.Cliente?.Nombre ?? "-";
            hoja.Cell(4, 1).Value = "Cédula:";
            hoja.Cell(4, 2).Value = pedido.Cliente?.Cedula ?? "-";
            hoja.Cell(5, 1).Value = "Fecha:";
            hoja.Cell(5, 2).Value = pedido.Fecha.ToString("dd/MM/yyyy HH:mm");
            hoja.Cell(6, 1).Value = "Estado:";
            hoja.Cell(6, 2).Value = pedido.Estado;

            var filaEncabezado = 8;
            string[] encabezados = ["Producto", "Cantidad", "Precio unit.", "Descuento %", "Impuesto %", "Total línea"];
            for (var i = 0; i < encabezados.Length; i++)
            {
                var celda = hoja.Cell(filaEncabezado, i + 1);
                celda.Value = encabezados[i];
                celda.Style.Font.Bold = true;
                celda.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            var fila = filaEncabezado + 1;
            foreach (var d in pedido.Detalles)
            {
                hoja.Cell(fila, 1).Value = d.Producto?.Nombre ?? "-";
                hoja.Cell(fila, 2).Value = d.Cantidad;
                hoja.Cell(fila, 3).Value = d.PrecioUnitario;
                hoja.Cell(fila, 4).Value = d.Descuento;
                hoja.Cell(fila, 5).Value = d.Porcentaje_Imp;
                hoja.Cell(fila, 6).Value = d.Total_Linea;
                fila++;
            }

            fila++;
            hoja.Cell(fila, 5).Value = "Subtotal:";
            hoja.Cell(fila, 6).Value = pedido.Subtotal;
            fila++;
            hoja.Cell(fila, 5).Value = "Impuestos:";
            hoja.Cell(fila, 6).Value = pedido.Impuestos;
            fila++;
            hoja.Cell(fila, 5).Value = "Total:";
            hoja.Cell(fila, 5).Style.Font.Bold = true;
            hoja.Cell(fila, 6).Value = pedido.Total;
            hoja.Cell(fila, 6).Style.Font.Bold = true;

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            libro.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
