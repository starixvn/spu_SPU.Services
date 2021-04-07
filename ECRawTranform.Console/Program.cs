using Fanex.NAP.DCS.Model;
using Fanex.NAP.DCS.Services.Implement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace ECRawTranform.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataTransformService = new DataTransformService();

            try
            {
                var data = new List<RawTransactionModel>();

                do
                {
                    data = dataTransformService.GetECRawTrans(2000);

                    if (data != null && data.Any())
                    {

                        dataTransformService.InsertECRawTrans(JsonConvert.SerializeObject(data));

                    }
                } while (data != null && data.Any());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("e");
            }
        }
    }
}
