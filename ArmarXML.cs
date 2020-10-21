using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrearConexion
{
    class ArmarXML
    {
        public static string ConsultaBarcode(string RequesterNumber,string Country,string Code, string Barcode) {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("BCmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute CountryAttribute = doc.CreateAttribute("Country");
            CountryAttribute.Value = Country;
            XmlAttribute CodeAttribute = doc.CreateAttribute("Code");
            CodeAttribute.Value = Code;
            XmlAttribute BarcodeAttribute = doc.CreateAttribute("BarCode");
            BarcodeAttribute.Value = Barcode;

            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(CodeAttribute);
            bNode.Attributes.Append(CountryAttribute);
            bNode.Attributes.Append(BarcodeAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string ConsultaStock(string OrderNumber, string RequesterNumber, string BarCode, string BatchNumber, string ExternalIdCode)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            // Add the new node to the document.
            XmlElement root = doc.DocumentElement;
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("VCmd");
            XmlAttribute OrderNumberAttribute = doc.CreateAttribute("OrderNumber");
            OrderNumberAttribute.Value = OrderNumber;
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute BarCodeAttribute = doc.CreateAttribute("BarCode");
            BarCodeAttribute.Value = BarCode;
            XmlAttribute BatchNumberAttribute = doc.CreateAttribute("BatchNumber");
            BatchNumberAttribute.Value = BatchNumber;
            XmlAttribute ExternalIdCodeAttribute = doc.CreateAttribute("ExternalIdCode");
            ExternalIdCodeAttribute.Value = ExternalIdCode;

            bNode.Attributes.Append(OrderNumberAttribute);
            bNode.Attributes.Append(RequesterNumberAttribute);
            if (!string.IsNullOrEmpty(BarCode))
            {
                bNode.Attributes.Append(BarCodeAttribute);
                bNode.Attributes.Append(BatchNumberAttribute);
                bNode.Attributes.Append(ExternalIdCodeAttribute);
            }

            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string DispensarArticulo(string RequesterNumber, string OrderNumber, string OutputNumber, string Priority, string Country, string Code, string BarCode, string Quantity, string Flag = "0")
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("ACmd");

            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute OrderNumberAttribute = doc.CreateAttribute("OrderNumber");
            OrderNumberAttribute.Value = OrderNumber;
            XmlAttribute OutputNumberAttribute = doc.CreateAttribute("OutputNumber");
            OutputNumberAttribute.Value = OutputNumber;
            XmlAttribute PriorityAttribute = doc.CreateAttribute("Priority");
            PriorityAttribute.Value = Priority;
            XmlAttribute CountryAttribute = doc.CreateAttribute("Country");
            CountryAttribute.Value = Country;
            XmlAttribute CodeAttribute = doc.CreateAttribute("Code");
            CodeAttribute.Value = Code;

            // Tag Record

            XmlNode bNodeItem = doc.CreateElement("Record");

            XmlAttribute BarCodeAttribute = doc.CreateAttribute("BarCode");
            BarCodeAttribute.Value = BarCode;
            XmlAttribute QuantityAttribute = doc.CreateAttribute("Quantity");
            QuantityAttribute.Value = Quantity;
            XmlAttribute FlagAttribute = doc.CreateAttribute("Flag");
            FlagAttribute.Value = Flag;

            bNodeItem.Attributes.Append(BarCodeAttribute);
            bNodeItem.Attributes.Append(QuantityAttribute);
            bNodeItem.Attributes.Append(FlagAttribute);
            bNode.AppendChild(bNodeItem);
            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(OrderNumberAttribute);
            bNode.Attributes.Append(OutputNumberAttribute);
            bNode.Attributes.Append(PriorityAttribute);
            bNode.Attributes.Append(CountryAttribute);
            bNode.Attributes.Append(CodeAttribute);
            rootNode.AppendChild(bNode);
            return doc.OuterXml;
        }

        // O Dialog
        public static string EstadoOrden(string RequesterNumber, string OrderNumber)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("OCmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute OrderNumberAttribute = doc.CreateAttribute("OrderNumber");
            OrderNumberAttribute.Value = OrderNumber;

            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(OrderNumberAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string IniciarDialogo(string RequesterNumber, string Protocol, string Dialogs)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("RCmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute ProtocolAttribute = doc.CreateAttribute("Protocol");
            ProtocolAttribute.Value = Protocol;
            XmlAttribute DialogsAttribute = doc.CreateAttribute("Dialogs");
            DialogsAttribute.Value = Dialogs;

            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(ProtocolAttribute);
            bNode.Attributes.Append(DialogsAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string StatusRobot(string RequesterNumber)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("SCmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;

            bNode.Attributes.Append(RequesterNumberAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string ConsultaProducto(string BarCode, string ItemName,string ItemTyp = "",string ItemUnit="Un",string RequesterNumber = "1",string Country = "0", string Code = "0")
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("PCmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute CountryAttribute = doc.CreateAttribute("Country");
            CountryAttribute.Value = Country;
            XmlAttribute CodeAttribute = doc.CreateAttribute("Code");
            CodeAttribute.Value = Code;
            XmlAttribute BarCodeAttribute = doc.CreateAttribute("BarCode");
            BarCodeAttribute.Value = BarCode;
            XmlAttribute ItemNameAttribute = doc.CreateAttribute("ItemName");
            ItemNameAttribute.Value = ItemName;
            XmlAttribute ItemTypAttribute = doc.CreateAttribute("ItemTyp");
            ItemTypAttribute.Value = ItemTyp;
            XmlAttribute ItemUnitAttribute = doc.CreateAttribute("ItemUnit");
            ItemUnitAttribute.Value = ItemUnit;

            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(CodeAttribute);
            bNode.Attributes.Append(CountryAttribute);
            bNode.Attributes.Append(BarCodeAttribute);
            bNode.Attributes.Append(ItemNameAttribute);
            bNode.Attributes.Append(ItemTypAttribute);
            bNode.Attributes.Append(ItemUnitAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

        public static string IngresoRobot(string RequesterNumber,string OrderNumber,string DeliveryNumber,string BarCode, string Date, string State, string Text, string BatchNumber, string ExternalIdCode,string Quantity = "1", string Country = "0", string Code = "0")
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("WaWi");
            doc.AppendChild(rootNode);

            XmlNode bNode = doc.CreateElement("ICmd");
            XmlAttribute RequesterNumberAttribute = doc.CreateAttribute("RequesterNumber");
            RequesterNumberAttribute.Value = RequesterNumber;
            XmlAttribute OrderNumberAttribute = doc.CreateAttribute("OrderNumber");
            OrderNumberAttribute.Value = OrderNumber;
            XmlAttribute DeliveryNumberAttribute = doc.CreateAttribute("DeliveryNumber");
            DeliveryNumberAttribute.Value = DeliveryNumber;
            XmlAttribute BarCodeAttribute = doc.CreateAttribute("BarCode");
            BarCodeAttribute.Value = BarCode;
            XmlAttribute DateAttribute = doc.CreateAttribute("Date");
            DateAttribute.Value = Date;
            XmlAttribute StateAttribute = doc.CreateAttribute("State");
            StateAttribute.Value = State;
            XmlAttribute TextAttribute = doc.CreateAttribute("Text");
            TextAttribute.Value = Text;
            XmlAttribute BatchNumberAttribute = doc.CreateAttribute("BatchNumber");
            BatchNumberAttribute.Value = BatchNumber;
            XmlAttribute ExternalIdCodeAttribute = doc.CreateAttribute("ExternalIdCode");
            ExternalIdCodeAttribute.Value = ExternalIdCode;
            XmlAttribute QuantityAttribute = doc.CreateAttribute("Quantity");
            QuantityAttribute.Value = Quantity;
            XmlAttribute CountryAttribute = doc.CreateAttribute("Country");
            CountryAttribute.Value = Country;
            XmlAttribute CodeAttribute = doc.CreateAttribute("Code");
            CodeAttribute.Value = Code;

            bNode.Attributes.Append(OrderNumberAttribute);
            bNode.Attributes.Append(RequesterNumberAttribute);
            bNode.Attributes.Append(DeliveryNumberAttribute);
            bNode.Attributes.Append(CountryAttribute);
            bNode.Attributes.Append(CodeAttribute);
            bNode.Attributes.Append(BarCodeAttribute);
            bNode.Attributes.Append(DateAttribute);
            bNode.Attributes.Append(StateAttribute);
            bNode.Attributes.Append(TextAttribute);
            bNode.Attributes.Append(BatchNumberAttribute);
            bNode.Attributes.Append(ExternalIdCodeAttribute);
            rootNode.AppendChild(bNode);

            return doc.OuterXml;
        }

    }
}
