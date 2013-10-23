#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.IO;
using System.Text;

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal static class StreamHelper
    {
        internal static TextWriter CreateFileAppender(string fileName, Encoding encode, bool correctEnd)
        {
            return CreateFileAppender(fileName, encode, correctEnd, true);
        }

        internal static TextWriter CreateFileAppender(string fileName, Encoding encode, bool correctEnd, bool disposeStream)
        {
            TextWriter res;

            if (correctEnd)
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    if (fs.Length >= 2)
                    {
                        fs.Seek(-2, SeekOrigin.End);

                        if (fs.ReadByte() == 13)
                        {
                            if (fs.ReadByte() == 10)
                            {
                                int nowRead;
                                do
                                {
                                    fs.Seek(-2, SeekOrigin.Current);
                                    nowRead = fs.ReadByte();
                                } while (nowRead == 13 || nowRead == 10);
                            }
                        }
                        else
                            fs.ReadByte();

                        fs.WriteByte(13);
                        fs.WriteByte(10);
                    }

                    res = new StreamWriter(fs, encode);
                }
                finally
                {
                    if (disposeStream && fs != null)
                        fs.Close();
                }
            }
            else
            {
                res = new StreamWriter(fileName, true, encode);
            }

            return res;
        }
    }
}