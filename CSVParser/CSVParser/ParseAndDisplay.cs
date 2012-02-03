using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;

namespace CSVParser
{
    public class ParseAndDisplay
    {
        private string mPath = "C:/Users/Paul Wallace/Documents/CSVLATLONG/World_Cities_Location_table.csv";
        private List<string> mId = new List<string>();
        private List<string> mCountry = new List<string>();
        private List<string> mCity = new List<string>();
        private List<string> mLatitude = new List<string>();
        private List<string> mLongitude = new List<string>();
        private List<string> mElevation = new List<string>();

        public ParseAndDisplay()
        {
            this.parseData(mPath);
        }

        private void parseData(string path)
        {           
            // List of arrays to contain each row of the csv
            List<string[]> parsedData = new List<string[]>();

            // Get the file and read it into the system by lines delimited my a comma(,).
            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        // Spilt by comma's, this is a CSV file afte rall
                        row = line.Split(',');
                        // Add each row to the parsedData List
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)//Catch any exceptions
            {
                // if there is an error output it
                Console.WriteLine(e.Message);
            }
            

            //
            // Now Break Each line into Parts with the following delimiters
            // 
            char[] delimiters = { ';', '"', '/' };

            // Here is the list to contain the parts.
            List<string[]> parts = new List<string[]>();

            
            for (int i = 0; i < parsedData.Count; i++)
            {                    
                // Add a string( of size 18) to the parts list. THis contains the entire row of data broken into parts.
                parts.Add(parsedData[i][0].Split(delimiters));
            }

            //
            // Because the list of parts has a bunch of unwanted peices we parse again to retain the important info.
            //

            // The list to contain the needed info.
            List<string[]> neededInfo = new List<string[]>();
            int count;
            for (int i = 0; i < parts.Count; i++)
            {
                //the count to add peices to the nonNullParts array.
                count = 0;

                // This string has to be recreated each time otherwise the end result 
                //will contain 10000 arrays of exactly the same thing; the last record.
                string[] nonNullParts = new string[6];
                for (int j = 0; j < parts[i].Length; j++)
                {                    
                    if (parts[i][j] != "" && parts[i][j] != null)// If the current part isnt a null and actually has a value then add it
                    {
                        // There are a couple weird records that didn't have any latitude or longitudes 
                        // if the length is lss than 18 ignore the record
                        if(parts[i].Length <= 18)
                        {
                            //Add it and increment the count.
                            nonNullParts[count] = parts[i][j];
                            count++;
                        }
                    }
                }
                neededInfo.Add(nonNullParts);
            }
            //
            // At this point we have a list containing a list containing an array of strings.
            // Each array has a length of six.
            //
            populateLists(neededInfo);
        }     
       
        //Populate each list.
        private void populateLists( List<string[]> neededInfo)
        {
            for (int i = 0; i < neededInfo.Count; i++)
            {
                    mId.Add(neededInfo[i][0]);
                    mCountry.Add(neededInfo[i][1]);
                    mCity.Add(neededInfo[i][2]);
                    mLatitude.Add(neededInfo[i][3]);
                    mLongitude.Add(neededInfo[i][4]);
                    mElevation.Add(neededInfo[i][5]);                
            }
        }

        //
        public void getLatitudeAndLongitudeByCity(string city)
        {
            int index = -1;
            for (int i = 0; i < mCity.Count; i++)
            {
                if (mCity[i] == city)
                {
                    index = i;
                    Console.WriteLine("Search done! " + city + " found at : " + i);
                    break;
                }
            }
            if (index != -1)
            {
                Console.WriteLine("Waiting on Weather.gov...");

                double latitude = Convert.ToDouble(mLatitude[index]);
                double longitude = Convert.ToDouble(mLongitude[index]);
                string xmlResult = cRESTfulQuery.Get_URI(latitude, longitude);
                string processedResult = cRESTfulQuery.parseXML(xmlResult);
                Console.WriteLine("Daily Maximum Temperature for " + city + ". \r\n");
                Console.WriteLine(processedResult);

                Console.WriteLine("The Latitude and Longitude of " + city + " are : " + mLatitude[index] + ", " + mLongitude[index] + ".");
            }
            else 
            {                
                Console.WriteLine(city + " was not found in our records, try another city.");
            }
            
        }
    
    }
}
