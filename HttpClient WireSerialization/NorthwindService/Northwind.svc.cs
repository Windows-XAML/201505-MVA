//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using Newtonsoft.Json;

namespace NorthwindService
{
    public class Northwind : DataService< NorthwindEntities >
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // Set rules to indicate which entity sets and service operations are visible, updatable, etc.
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("GetSuppliersLiteZip", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        private string GetSuppliersLite()
        {
            // Get the ObjectContext that is the data source for the service.
            NorthwindEntities context = this.CurrentDataSource;

            try
            {
                var suppliers = (from s in context.Suppliers
                                 select new
                                 {
                                     Address = s.Address,
                                     City = s.City,
                                     CompanyName = s.Company_Name,
                                     ContactName = s.Contact_Name,
                                     ContactTitle = s.Contact_Title,
                                     Country = s.Country,
                                     Fax = s.Fax,
                                     Phone = s.Phone,
                                     PostalCode = s.Postal_Code,
                                     Region = s.Region,
                                     SupplierID = s.Supplier_ID,
                                 }
                                ).ToList();

                string jsonClient = null;
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
                jsonSerializer.MissingMemberHandling = MissingMemberHandling.Error;
                jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Error;
                try
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        using (JsonTextWriter jtw = new JsonTextWriter(sw))
                        {
                            jsonSerializer.Serialize(jtw, suppliers);
                        }
                        jsonClient = sw.ToString();
                    }
                }
                catch (Exception ex)
                {
                    ex = ex; // have a breakpoint here so can inspect exception
                }
                return jsonClient;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format(
                    "An error occurred: {0}", ex.Message));
            }
        }

        [WebGet]
        public string GetSuppliersLiteZip()
        {
            return Zip(GetSuppliersLite());
        }

        private string Zip(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return sB.ToString();
        }
    }
}
