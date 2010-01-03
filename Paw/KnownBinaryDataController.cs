using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Paw
{
    class KnownBinaryDataController
    {
        private List<BinaryDataByIdentifier> alreadyKnownBinaryData = new List<BinaryDataByIdentifier>();

        public KnownBinaryDataController()
        {

        }

        ~KnownBinaryDataController()
        {
            foreach (BinaryDataByIdentifier currentData in alreadyKnownBinaryData)
            {
                try
                {
                    File.Delete(currentData.fileSystemPath);
                }
                catch
                {
                    Console.WriteLine("bla");
                }
            }
        }

        public string storeDataLocally(string identifier, string data, Int64 length)
        {
            if (identifier == "" || data == "")
            {
                return "";
            }
            BinaryDataByIdentifier currentData = searchByIdentifier(identifier);
            if (currentData != null)
            {
                // already stored
                return currentData.fileSystemPath;
            }
            // xxx temporarily disabled as it doesn't work anyway...
            return "";

            currentData = new BinaryDataByIdentifier();
            currentData.ID = identifier;
            currentData.length = length;

            currentData.fileSystemPath = Path.GetTempFileName();
            File.Delete(currentData.fileSystemPath);
            currentData.fileSystemPath = currentData.fileSystemPath + ".paw";

            FileStream outStream = File.Create(currentData.fileSystemPath);
            BinaryWriter binWriter = new BinaryWriter(outStream);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] stream = encoding.GetBytes(data);
            System.Drawing.Image test = ImageFromString(data);
            binWriter.Write(stream);
            binWriter.Flush();
            binWriter.Close();
            outStream.Close();

            alreadyKnownBinaryData.Add(currentData);

            return currentData.fileSystemPath;
        }

        public string giveFileSystemPathToIdentififer(string identifier)
        {
            BinaryDataByIdentifier currentData = searchByIdentifier(identifier);
            if (currentData != null)
            {
                // already stored
                return currentData.fileSystemPath;
            }
            else
            {
                return HelperFunctions.getPawDefaultIconPath();
            }
        }

        private BinaryDataByIdentifier searchByIdentifier(string identifier)
        {

            List<BinaryDataByIdentifier> tList = alreadyKnownBinaryData.FindAll(
                delegate(BinaryDataByIdentifier data)
                {
                    return data.ID.Equals(identifier);
                }
            );
            if (tList.Count > 0)
            {
                return tList.First();
            }
            else
            {
                return null;
            }
        }

        public static System.Drawing.Image ImageFromString(string data)
        {
            System.Drawing.Image image = null;
            try
            {
                if (data != "")
                {
                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        byte[] stream = encoding.GetBytes(data);
                        image = ImageFromBytes(stream);
                }
            }
            catch
            {
            }
            return image;
        }


        /// <summary>
        /// Converts an array of bytes into an <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="bytes">The array of bytes</param>
        /// <returns>The resulting <see cref="System.Drawing.Image"/></returns>
        public static System.Drawing.Image ImageFromBytes(byte[] bytes)
        {
            System.Drawing.Image image = null;
            try
            {
                if (bytes != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes, false);
                    using (ms)
                    {
                        image = ImageFromStream(ms);
                    }
                }
            }
            catch
            {
            }
            return image;
        }

        private static System.Drawing.Image ImageFromStream(System.IO.Stream stream)
        {
            System.Drawing.Image image = null;
            try
            {
                stream.Position = 0;
                System.Drawing.Image tempImage = System.Drawing.Bitmap.FromStream(stream);
                // dont close stream yet, first create a copy
                using (tempImage)
                {
                    image = new Bitmap(tempImage);
                }
            }
            catch
            {
                // the file could be an .ico file that is not usable by the Image class, so try that just in case
                try
                {
                    stream.Position = 0;
                    Icon icon = new Icon(stream);
                    if (icon != null) image = icon.ToBitmap();
                }
                catch
                {
                    // give up - we cant open this file type
                }
            }

            return image;
        }
    }
}
