using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;

namespace CrearConexion
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = Configuration.GetConfiguration("conexionConfig", "ip");
            string port = Configuration.GetConfiguration("conexionConfig", "port");
            

            Process[] pname = Process.GetProcessesByName("CrearConexion");

            if (pname.Length > 1)
            {
                Console.WriteLine("La aplicación ya se encuentra corriendo en otra instancia.");
                Thread.Sleep(5 * 1000);
            }
            else
            {
                try
                {
                    bool portResult = int.TryParse(port,out int portNumber);
                    if (portResult)
                    {
                        SocketSendReceive(ip, portNumber);
                    }
                    else
                    {
                        Console.WriteLine("Debe ingresar un número de puerto correcto.");
                        Thread.Sleep(5 * 1000);
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex);
                    Thread.Sleep(5 * 1000);
                }
            }
        }

        private static Socket ConnectSocket(string ip, int port)
        {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            s.NoDelay = true;
            //only as example to terminate the application
            //s.ReceiveTimeout = 20000;
            bool connected = false;
            //int numberOfTimes = 0;
            while (!connected)
            {
                try
                {
                    s.Connect(ipe);
                    if (s.Connected)
                    {
                        connected = true;
                        return (s);
                    }
                }catch(SocketException retryConnectException)
                {
                    Console.WriteLine("Hay problemas al conectarse. Reintentando...");
                    Thread.Sleep(30 * 1000);
                    continue;
                    //if (numberOfTimes == 5)
                    //{
                    //    break;
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Hay problemas al conectarse. Reintentando...");
                    //    Thread.Sleep(60 * 1000);
                    //    numberOfTimes++;
                    //    continue;
                    //}
                }
            }
            return (null);
        }

        public class WSConsultaProductoItemRequest
        {
            public string CodigoProducto { get; set; }
            public string ProductoCodigoBarraId { get; set; }
            public string BultoIdOriginalCD { get; set; }
        }

        public class WSConsultaProductoItemResponse
        {
            public List<MessagesItem> Messages { get; set; }
            public string ProductoDescripcion { get; set; }
            public string ProductoId { get; set; }
            public string Resultado { get; set; }
        }

        public class MessagesItem
        {
            public string Description { get; set; }
            public string Id { get; set; }
            public int Type { get; set; }
        }

        public class WSRecepcionRobotRequest
        {
            public string ProductoId { get; set; }
            public string BultoIdOriginalCD { get; set; }
        }

        public class WSRecepcionRobotResponse
        {
            public List<MessagesItem> Messages { get; set; }
            public string Resultado { get; set; }
        }

        public static int IndexOf(StringBuilder sb, string value)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            for (int i = 0; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        private static List<string> getMessages(StringBuilder builder)
        {
            List<string> toReturn = new List<string>();
            while (true)
            {
                int messageStart = IndexOf(builder, getStx());
                if (messageStart < 0)
                {
                    builder.Remove(0, builder.Length);
                    break;
                }
                builder.Remove(0, messageStart);
                int messageEnd = IndexOf(builder, getEtx());
                if (messageEnd < 0)
                {
                    break;
                }

                toReturn.Add(builder.ToString(0, messageEnd + getStx().Length));
                builder.Remove(0, messageEnd + getEtx().Length);
            }
            return (toReturn);
        }

        static string stx = null;
        static string etx = null;

        public static string BatchNumber { get; private set; }

        private static String getStx()
        {
            if (stx == null)
            {
                int unicode = 2;
                char character = (char)unicode;
                stx = character.ToString();
            }
            return (stx);
        }

        private static String getEtx()
        {
            if (etx == null)
            {
                int unicode = 3;
                char character = (char)unicode;
                etx = character.ToString();
            }
            return (etx);
        }

        private static string getDialog(string xml)
        {
            string dialog = xml.Substring(1);
            dialog = dialog.Remove(dialog.Length - 1);
            dialog = dialog.Substring(7, 4);

            return dialog;
        }

        private static string AgregoCeros(string producto)
        {
            string barcode = producto.PadLeft(8, Char.Parse("0"));

            return barcode;
        }

        private static int GetMod10Digit(string data)
        {
            int sum = 0;
            int sum1 = 0;
            bool odd = true;
            for (int i = data.Length - 1; i >= 0; i--)
            {
                if (odd == true)
                {
                    int tSum = Convert.ToInt32(data[i].ToString()) * 2;
                    if (tSum >= 10)
                    {
                        string tData = tSum.ToString();
                        tSum = Convert.ToInt32(tData[0].ToString()) + Convert.ToInt32(tData[1].ToString());
                    }
                    sum1 += tSum;
                }
                else
                    sum += Convert.ToInt32(data[i].ToString());
                odd = !odd;
            }

            int result = (sum + sum1) % 10;
            return result;
        }

        private static string ProductoOriginal(string barcode)
        {
            string productoId = barcode.TrimStart(Char.Parse("0"));
            productoId = productoId.Remove(productoId.Length - 1);
            return productoId;
        }


        // This method requests the home page content for the specified server.
        private static void SocketSendReceive(string server, int port)
        {
            string request = getStx() + ArmarXML.StatusRobot("1") + getEtx();
            string dispense = Configuration.GetConfiguration("conexionConfig", "dispense");

            Console.WriteLine(request);
            Byte[] bytesToSend = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[1228800];

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port))
            {

                if (s == null)
                {
                    Console.Error.WriteLine("Error opening socket");
                    return;
                }

                // Send request to the server.
                s.Send(bytesToSend, bytesToSend.Length, 0);

                StringBuilder builder = new StringBuilder();

                int countBytesReceved;
                do
                {
                    try
                    {
                        countBytesReceved = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine(se.Message);
                        Thread.Sleep(5000);
                        break;
                    }
                    builder.Append(Encoding.ASCII.GetString(bytesReceived, 0, countBytesReceved));
                    foreach (string msg in getMessages(builder))
                    {
                        Console.WriteLine(msg);

                        string dialog = getDialog(msg);

                        string uris = "";
                        string paramsa = "";
                        string response = "";

                        switch (dialog)
                        {
                            case "IMsg": // Ingreso en el robot

                                //0 stock input request new delivery
                                //1 stock input request stock return
                                //2 start new delivery
                                //3 end new delivery
                                //4 set stock location number for pack as stock return
                                //5 set stock location number for pack as new delivery
                                //6 pack was input
                                //7 pack was not input

                                string status = XMLParse.GetSingleElement(msg, dialog, "", "State");

                                if (int.Parse(status) < 2)
                                {

                                    // Toma el codigo de barras
                                    string barcode = XMLParse.GetSingleElement(msg, dialog, "", "BarCode");
                                    string bulto = XMLParse.GetSingleElement(msg, dialog, "", "DeliveryNumber");

                                    // Consumo servicio de consulta del producto
                                    uris = Configuration.GetConfiguration("ws", "wsdevproductoId");
                                    WSConsultaProductoItemRequest Iwsconsultaproductoitem = new WSConsultaProductoItemRequest();
                                    Iwsconsultaproductoitem.CodigoProducto = "";
                                    Iwsconsultaproductoitem.ProductoCodigoBarraId = barcode;
                                    Iwsconsultaproductoitem.BultoIdOriginalCD = bulto;
                                    paramsa = JsonConvert.SerializeObject(Iwsconsultaproductoitem);
                                    response = ConsumirRESTWS.GetPOSTResponse(uris, paramsa);

                                    string text = "";
                                    string state = "0";

                                    if (response == "ERROR")
                                    {
                                        Console.WriteLine("Error al consumir wsdevproductoId");
                                        text = "Error al obtener informacion del producto";
                                        state = "1";
                                    }
                                    
                                    WSConsultaProductoItemResponse producto = JsonConvert.DeserializeObject<WSConsultaProductoItemResponse>(response);

                                    // Chequeo si dio bien

                                    //0 article may be input
                                    //1 article may not to be input
                                    //2 article must be input with expiration date
                                    //4 no stock location number set
                                    //5 article may be input. The stock location in the machine must be a refrigerated unit

                                    if (producto.Resultado == "false")
                                    {
                                        foreach (var message in producto.Messages)
                                        {
                                            text = message.Description;
                                            state = "1";
                                        }
                                    }

                                    //Envio el response al robot

                                    string requesternumber = XMLParse.GetSingleElement(msg, dialog, "", "RequesterNumber");
                                    string ordernumber = XMLParse.GetSingleElement(msg, dialog, "", "OrderNumber");
                                    string date = XMLParse.GetSingleElement(msg, dialog, "", "Date");
                                    string country = XMLParse.GetSingleElement(msg, dialog, "", "Country");
                                    string code = XMLParse.GetSingleElement(msg, dialog, "", "Code");
                                    string batchnumber = XMLParse.GetSingleElement(msg, dialog, "", "BatchNumber");
                                    string externalidcode = XMLParse.GetSingleElement(msg, dialog, "", "ExternalIdCode");

                                    string productoBarcode = "";

                                    if (dispense.ToLower().Trim() == "cb")
                                    {
                                        productoBarcode = producto.ProductoId;
                                    }
                                    else
                                    {
                                        productoBarcode = AgregoCeros(producto.ProductoId);
                                        productoBarcode = productoBarcode + GetMod10Digit(productoBarcode).ToString();
                                    }

                                    request = getStx() + ArmarXML.IngresoRobot(requesternumber, ordernumber, bulto, productoBarcode, date, state, text, batchnumber, externalidcode, "1", country, code) + getEtx();
                                    bytesToSend = Encoding.ASCII.GetBytes(request);
                                    s.Send(bytesToSend, bytesToSend.Length, 0);
                                    Console.WriteLine(request);
                                }
                                else if (int.Parse(status) == 6)
                                {
                                    // Si el status es 6 y tiene DeliveryNumber (ingresado correctamente) envio la recepcion a Inventarios
                                    string productoIdAux = XMLParse.GetSingleElement(msg, dialog, "", "BarCode");
                                    string bulto = XMLParse.GetSingleElement(msg, dialog, "", "DeliveryNumber");
                                    Console.WriteLine("BULTO: " + bulto);
                                    if (!string.IsNullOrEmpty(bulto))
                                    {
                                        string productoId = "";

                                        if (dispense.ToLower().Trim() == "cb")
                                        {
                                            productoId = productoIdAux;
                                        }
                                        else
                                        {
                                            productoId = ProductoOriginal(productoIdAux);
                                        }

                                        uris = Configuration.GetConfiguration("ws", "WSRecepcionRobot");
                                        WSRecepcionRobotRequest wsrecepcionrobotrequest = new WSRecepcionRobotRequest();
                                        wsrecepcionrobotrequest.ProductoId = productoId;
                                        wsrecepcionrobotrequest.BultoIdOriginalCD = bulto;
                                        paramsa = JsonConvert.SerializeObject(wsrecepcionrobotrequest);
                                        response = ConsumirRESTWS.GetPOSTResponse(uris, paramsa);

                                        Console.WriteLine("WSRecepcionRobot: " + response);

                                        if (response != "ERROR")
                                        {
                                            WSRecepcionRobotResponse wsrecepcionrobotresponse = JsonConvert.DeserializeObject<WSRecepcionRobotResponse>(response);
                                            if (wsrecepcionrobotresponse.Resultado == "false")
                                            {
                                                GraboLog.GrabarLog("No se guardo en Inventarios - " + response);
                                                foreach (var message in wsrecepcionrobotresponse.Messages)
                                                {
                                                    Console.WriteLine("Error al guardar en inventarios: " + message.Description);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error al consumir WSRecepcionRobot");
                                        }
                                    }
                                }

                                break;
                            case "AMsg": // Salida en el robot
                                string orderNumber = XMLParse.GetSingleElement(msg, "AMsg", "", "OrderNumber");
                                Console.WriteLine("OrderNumber:" + orderNumber);
                                List<Record> amsg = XMLParse.GetListElement(msg, "AMsg", "Record", "BarCode", "Quantity");
                                foreach (var amsgitem in amsg)
                                {
                                    Console.WriteLine(amsgitem.BarCode + " - " + amsgitem.Quantity);
                                }
                                break;
                            case "PMsg": // Consulta del robot
                                string pbarcode = XMLParse.GetSingleElement(msg, "PMsg", "", "BarCode");
                                string pcountry = XMLParse.GetSingleElement(msg, "PMsg", "", "Country");
                                string pcode = XMLParse.GetSingleElement(msg, "PMsg", "", "Code");
                                string prequester = XMLParse.GetSingleElement(msg, "PMsg", "", "RequesterNumber");

                                string productoid = ProductoOriginal(pbarcode);

                                // Consumo servicio de consulta del producto
                                uris = Configuration.GetConfiguration("ws", "wsdevproductoId");
                                WSConsultaProductoItemRequest Pwsconsultaproductoitem = new WSConsultaProductoItemRequest();
                                Pwsconsultaproductoitem.CodigoProducto = productoid;
                                Pwsconsultaproductoitem.ProductoCodigoBarraId = "";
                                Pwsconsultaproductoitem.BultoIdOriginalCD = "";
                                paramsa = JsonConvert.SerializeObject(Pwsconsultaproductoitem);
                                response = ConsumirRESTWS.GetPOSTResponse(uris, paramsa);

                                if (response == "ERROR")
                                {
                                    Console.WriteLine("Error al consumir wsdevproductoId");
                                    break;
                                }
                                else
                                {
                                    WSConsultaProductoItemResponse producto = JsonConvert.DeserializeObject<WSConsultaProductoItemResponse>(response);

                                    string itemname = producto.ProductoDescripcion;

                                    request = getStx() + ArmarXML.ConsultaProducto(pbarcode, itemname, "MED", "Un", prequester, pcountry, pcode) + getEtx();
                                    bytesToSend = Encoding.ASCII.GetBytes(request);
                                    s.Send(bytesToSend, bytesToSend.Length, 0);
                                    Console.WriteLine(request);
                                }


                                break;
                            case "OMsg": // Status orden
                                break;
                            default:
                                Console.WriteLine("No existe dialogo: " + dialog);
                                break;
                        }
                    }
                } while (countBytesReceved >= 0);
            }
        }

        // Ejemplo Consumo WS REST
        //string uris = ConectorROWA16.Configuration.GetConfiguration("ws", "WSGraboLog");
        //wsgrabologitem.Mensaje = NumeroProcesoInt.ToString() + " resultado: " + resultado.ToString() + " NumeroProceso: " + NumeroProceso;
        //  string paramsa = JsonConvert.SerializeObject(wsgrabologitem);
        //ConsumirRESTWS.GetPOSTResponse(uris, paramsa);

    }
}
