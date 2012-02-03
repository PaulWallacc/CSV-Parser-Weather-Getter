using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml;
using System.Globalization;

namespace CSVParser
{
    public class cRESTfulQuery
    {
        public static string Get_URI(double latitude, double longitude)
        {
            string uri = "http://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?whichClient=NDFDgen&lat=" + latitude + "&lon=" + longitude + "&listLatLon=&lat1=&lon1=&lat2=&lon2=&resolutionSub=&listLat1=&listLon1=&listLat2=&listLon2=&resolutionList=&endPoint1Lat=&endPoint1Lon=&endPoint2Lat=&endPoint2Lon=&listEndPoint1Lat=&listEndPoint1Lon=&listEndPoint2Lat=&listEndPoint2Lon=&zipCodeList=&listZipCodeList=&centerPointLat=&centerPointLon=&distanceLat=&distanceLon=&resolutionSquare=&listCenterPointLat=&listCenterPointLon=&listDistanceLat=&listDistanceLon=&listResolutionSquare=&citiesLevel=&listCitiesLevel=&sector=&gmlListLatLon=&featureType=&requestedTime=&startTime=&endTime=&compType=&propertyName=&product=time-series&begin=2004-01-01T00%3A00%3A00&end=2016-01-24T00%3A00%3A00&Unit=&maxt=maxt&mint=mint&Submit=Submit";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string responseString = reader.ReadToEnd();

            reader.Close();
            responseStream.Close();
            response.Close();

            return responseString;

        }


        /*
         * For some reason I have to break up the sections of code where I am rading from the xml.
         * I tried to have the reader go back to the top but I never got it to work 
         * That is why I have three different 'using'statements to access the xml.
        */
        public static String parseXML(String xmlString)
        {
            StringBuilder output = new StringBuilder();

            List<string> dates = new List<string>();
            List<string> values = new List<string>();

            //
            // CHeck to see if there is an error. If so then return what it is to the console
            //
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                if (reader.ReadToFollowing("error"))
                {
                    reader.ReadToFollowing("pre");
                    string error = (reader.ReadElementContentAsString());
                    output.AppendLine(error);

                    return output.ToString();
                }
            }

            //
            // Get all of the dates for the min and max values.
            //
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.ReadToFollowing("start-valid-time"))
                {
                    string tmp = (reader.ReadElementContentAsString());


                    string[] parts = tmp.Split('-', 'T');

                    int year = Convert.ToInt16(parts[0]);
                    int month = Convert.ToInt16(parts[1]);
                    int day = Convert.ToInt16(parts[2]);

                    string theDate = month + "/" + day + "/" + year;

                    DateTime date = new DateTime(year, month, day);




                    dates.Add(date.DayOfWeek + " " + theDate + ": ");
                }
            }

            //
            // get all of the min and max values for the dates above
            //
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.ReadToFollowing("value"))
                {
                    string tmp = reader.ReadElementContentAsString();

                    values.Add(tmp);
                }

            }

            //
            // Format the output to look nice.
            //
            for (int i = 0; i < dates.Count(); i++)
            {
                string theFirstDate = dates[0];
                string theCurrentDate = dates[i];

                if (i != 0 && theCurrentDate == theFirstDate)
                {
                    output.AppendLine("");
                    output.AppendLine("Daily Minimum Temperatures");
                    output.AppendLine("");
                }

                output.AppendLine(dates[i] + " " + values[i]);
            }


            // Return the output.
            return output.ToString();
        }
    }

}
