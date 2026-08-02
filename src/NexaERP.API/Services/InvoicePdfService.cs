using System.Globalization;
using NexaERP.BLL.DTOs.Invoice;
using NexaERP.DAL.Enums;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NexaERP.API.Services;

public sealed class InvoicePdfService
{
    // Company logo displayed in the invoice header.
    private static readonly byte[] Logo =
        File.ReadAllBytes("Assets/logo.png");

    // Generates a PDF document for the specified invoice.
    public MemoryStream Generate(InvoiceDto invoice)
    {
        // Use the QuestPDF Community license.
        Settings.License = LicenseType.Community;

        var stream = new MemoryStream();

        // Build the PDF document.
        Document.Create(container =>
        {
            container.Page(page =>
            {
                // Configure page layout.
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);

                // Set the default font size.
                page.DefaultTextStyle(x => x.FontSize(11));

                // Add the document header.
                page.Header()
                    .Column(column =>
                    {
                        // Company information.
                        column.Item()
                            .Element(x => BuildHeader(x));

                        // Separator below the header.
                        column.Item()
                            .PaddingTop(12)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                    });

                // Add the document body.
                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        // Display invoice and customer information side by side.
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                BuildInvoiceInfo(left, invoice);
                            });

                            row.ConstantItem(20);

                            row.RelativeItem().Column(right =>
                            {
                                BuildCustomer(right, invoice);
                            });
                        });

                        // Display invoice items.
                        BuildLines(column, invoice);

                        // Display invoice totals.
                        BuildSummary(column, invoice);
                    });

                // Add the footer.
                page.Footer()
                    .Element(BuildFooter);
            });
        })
        .GeneratePdf(stream);

        // Reset stream position before returning.
        stream.Position = 0;

        return stream;
    }

    // Builds the invoice header.
    private static void BuildHeader(
        IContainer container)
    {
        container.Row(row =>
        {
            // Company logo and information.
            row.RelativeItem().Row(left =>
            {
                left.ConstantItem(60)
                    .Height(60)
                    .Image(Logo);

                left.RelativeItem()
                    .PaddingLeft(10)
                    .Column(column =>
                    {
                        column.Item()
                            .Text("NexaERP")
                            .FontSize(22)
                            .Bold();

                        column.Item()
                            .Text("Enterprise Resource Planning");
                    });
            });

            // Invoice title.
            row.RelativeItem()
                .AlignRight()
                .Column(column =>
                {
                    column.Item()
                        .Text("Invoice")
                        .FontSize(26)
                        .Bold();
                });
        });
    }

    // Displays invoice information.
    private static void BuildInvoiceInfo(
        ColumnDescriptor column,
        InvoiceDto invoice)
    {
        // Add spacing before the section.
        column.Item().PaddingTop(20);

        // Invoice information container.
        column.Item()
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(10)
            .Column(info =>
            {
                // Section title.
                info.Item()
                    .Text("Invoice Information")
                    .Bold()
                    .FontSize(15);

                info.Item().PaddingTop(5);

                // Invoice date.
                InfoRow(
                    info,
                    "Invoice Date",
                    invoice.InvoiceDate.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture));

                // Due date.
                InfoRow(
                    info,
                    "Due Date",
                    invoice.DueDate.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture));

                // Invoice status.
                StatusRow(info, invoice.Status);

            });
    }
    // Displays customer information.
    private static void BuildCustomer(
        ColumnDescriptor column,
        InvoiceDto invoice)
    {
        // Add spacing before the section.
        column.Item().PaddingTop(20);

        // Customer information container.
        column.Item()
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(10)
            .Column(customer =>
            {
                // Section title.
                customer.Item()
                    .Text("Customer")
                    .Bold()
                    .FontSize(15);

                customer.Item().PaddingTop(5);

                // Customer name.
                CustomerRow(
                    customer,
                    "Name",
                    invoice.CustomerName);

                // Customer email.
                CustomerRow(
                    customer,
                    "Email",
                    invoice.CustomerEmail);

                // Customer phone number.
                CustomerRow(
                    customer,
                    "Phone",
                    invoice.CustomerPhone);
            });
    }

    // Builds the invoice items table.
    private static void BuildLines(
        ColumnDescriptor column,
        InvoiceDto invoice)
    {
        // Add spacing before the section.
        column.Item().PaddingTop(25);

        // Section title.
        column.Item()
            .Text("Invoice Lines")
            .Bold()
            .FontSize(15);

        // Display invoice line items in a table.
        column.Item().Table(table =>
        {
            // Configure table columns.
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(5);      // Description
                columns.RelativeColumn(1.5f);   // Quantity
                columns.RelativeColumn(2);      // Price
                columns.RelativeColumn(1.5f);   // Tax
                columns.RelativeColumn(2);      // Total
            });

            // Create table header.
            table.Header(header =>
            {
                header.Cell()
                    .Element(HeaderCellStyle)
                    .AlignLeft()
                    .Text("Description")
                    .Bold();

                header.Cell()
                    .Element(HeaderCellStyle)
                    .AlignCenter()
                    .Text("Quantity")
                    .Bold();

                header.Cell()
                    .Element(HeaderCellStyle)
                    .AlignRight()
                    .Text("Price")
                    .Bold();

                header.Cell()
                    .Element(HeaderCellStyle)
                    .AlignRight()
                    .Text("Tax")
                    .Bold();

                header.Cell()
                    .Element(HeaderCellStyle)
                    .AlignRight()
                    .Text("Total")
                    .Bold();
            });

            // Add one row for each invoice line.
            foreach (var line in invoice.Lines)
            {
                // Calculate the line total.
                var subtotal = line.Quantity * line.UnitPrice;
                var tax = subtotal * line.TaxRate / 100m;
                var total = subtotal + tax;

                // Description.
                table.Cell()
                    .Element(CellStyle)
                    .AlignLeft()
                    .Text(line.Description);

                // Quantity.
                table.Cell()
                    .Element(CellStyle)
                    .AlignCenter()
                    .Text(line.Quantity.ToString(CultureInfo.InvariantCulture));

                // Unit price.
                table.Cell()
                    .Element(CellStyle)
                    .AlignRight()
                    .Text($"{line.UnitPrice:N2}");

                // Tax percentage.
                table.Cell()
                    .Element(CellStyle)
                    .AlignRight()
                    .Text($"{line.TaxRate}%");

                // Line total.
                table.Cell()
                    .Element(CellStyle)
                    .AlignRight()
                    .Text($"{total:N2}");
            }
        });
    }

    // Builds the invoice summary.
    private static void BuildSummary(
        ColumnDescriptor column,
        InvoiceDto invoice)
    {
        // Calculate invoice totals.
        var subtotal = invoice.Lines.Sum(x => x.Quantity * x.UnitPrice);

        var tax = invoice.Lines.Sum(x =>
            x.Quantity * x.UnitPrice * x.TaxRate / 100m);

        // Display the invoice totals section.
        column.Item()
            .PaddingTop(25)
            .AlignRight()
            .Width(220)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(10)
            .Column(summary =>
            {
                // Subtotal.
                SummaryRow(
                    summary,
                    "Subtotal",
                    subtotal);

                // Total tax.
                SummaryRow(
                    summary,
                    "Tax",
                    tax);

                // Separator line.
                summary.Item()
                    .PaddingVertical(5)
                    .LineHorizontal(1);

                // Grand total.
                SummaryRow(
                    summary,
                    "Grand Total",
                    invoice.TotalAmount,
                    true);

                // Display payment date if available.
                if (invoice.PaidAt is not null)
                {
                    summary.Item()
                        .PaddingTop(10)
                        .Text(text =>
                        {
                            text.Span("Paid At: ").Bold();

                            text.Span(
                                invoice.PaidAt.Value.ToString(
                                    "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture));
                        });
                }

                // Display payment method if available.
                if (!string.IsNullOrWhiteSpace(invoice.PaymentMethod))
                {
                    summary.Item().Text(text =>
                    {
                        text.Span("Payment Method: ").Bold();
                        text.Span(invoice.PaymentMethod);
                    });
                }
            });
    }

    // Builds the footer with page numbering.
    private static void BuildFooter(
        IContainer container)
    {
        container.Row(row =>
        {
            // Application name.
            row.RelativeItem()
                .Text("Generated by NexaERP ERP System")
                .FontSize(10);

            // Display current page and total pages.
            row.ConstantItem(120)
                .AlignRight()
                .Text(text =>
                {
                    text.CurrentPageNumber();

                    text.Span(" / ");

                    text.TotalPages();
                });
        });
    }

    // Applies the default style for table cells.
    private static IContainer CellStyle(
        IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6)
            .PaddingHorizontal(4);
    }

    // Displays a customer information row.
    private static void CustomerRow(
        ColumnDescriptor column,
        string title,
        string value)
    {
        column.Item().Text(text =>
        {
            text.Span($"{title}: ").Bold();
            text.Span(value);
        });
    }

    // Applies the header style for table columns.
    private static IContainer HeaderCellStyle(
        IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten3)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten1)
            .Padding(6);
    }

    // Returns the badge color based on invoice status.
    private static string GetStatusColor(
        InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Draft => Colors.Yellow.Lighten4,

            InvoiceStatus.Paid => Colors.Green.Lighten3,

            InvoiceStatus.Issued => Colors.Red.Lighten3,

            _ => Colors.Grey.Lighten3
        };
    }

    // Displays the invoice status as a colored badge.
    private static void StatusRow(
        ColumnDescriptor column,
        InvoiceStatus status)
    {
        column.Item().Row(row =>
        {
            row.AutoItem()
                .Text("Status:")
                .Bold();

            row.AutoItem()
                .PaddingLeft(6)
                .Background(GetStatusColor(status))
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingHorizontal(8)
                .PaddingVertical(3)
                .Text(status.ToString())
                .Bold();
        });
    }

    // Displays a labeled invoice information row.
    private static void InfoRow(
        ColumnDescriptor column,
        string title,
        string value)
    {
        column.Item().Text(text =>
        {
            text.Span($"{title}: ").Bold();
            text.Span(value);
        });
    }

    // Displays a summary row with a label and value.
    private static void SummaryRow(
        ColumnDescriptor column,
        string label,
        decimal value,
        bool isBold = false)
    {
        // Apply bold style when requested.
        var style = isBold
            ? TextStyle.Default.Bold()
            : TextStyle.Default;

        column.Item().Row(row =>
        {
            // Summary label.
            row.RelativeItem()
                .AlignLeft()
                .Text(label)
                .Style(style);

            // Summary value.
            row.ConstantItem(90)
                .AlignRight()
                .Text(value.ToString(
                    "N2",
                    CultureInfo.InvariantCulture))
                .Style(style);
        });
    }
}
