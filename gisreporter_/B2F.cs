using System;
using System.Collections.Generic;

namespace gisreporter_
{
	public class B2F
	{
        public string Mid;
		public DateTime Date;
		public string Type;
		public string From;
        public string To;
        public string CC;
        public string Subject;
		public string Mbo;
        public int BodyLength;
		public string Body;
		public List<B2F_File> Files;

        public B2F(string MessageBody)
		{
            Files = new List<B2F_File>();

            string[] msgArr = MessageBody.Split("\r\n", StringSplitOptions.None);

            for (int i = 0; i < msgArr.Length; i++)
            {
                string[] fieldArr = msgArr[i].Split(':', 2);
                switch (fieldArr[0].ToLower().Trim())
                {
                    case "mid":
                        Mid = fieldArr[1];
                        break;
                    case "date":
                        Date = DateTime.Parse(fieldArr[1]);
                        break;
                    case "type":
                        Type = fieldArr[1];
                        break;
                    case "from":
                        From = fieldArr[1];
                        break;
                    case "to":
                        To = fieldArr[1];
                        break;
                    case "cc":
                        CC = fieldArr[1];
                        break;
                    case "subject":
                        Subject = fieldArr[1];
                        break;
                    case "mbo":
                        Mbo = fieldArr[1];
                        break;
                    case "body":
                        BodyLength = int.Parse(fieldArr[1]);
                        break;
                    case "file":
                        B2F_File file = new B2F_File(fieldArr[1]);
                        Files.Add(file);
                        // You can't get the file contents yet.  Have to load all of the files first.
                        break;
                    case "":
                        goto AfterLoop;
                    default:
                        break;
                }
            }

        AfterLoop:

            string separator = "\r\n\r\n";
            int cursor = MessageBody.IndexOf(separator) + 2;

            Body = MessageBody.Substring(cursor, BodyLength + 2).Trim();
            cursor += BodyLength + 2;

            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Contents = MessageBody.Substring(cursor, Files[i].Length + 2).Trim();
                cursor += Files[i].Length + 2;
            }
        }
    }

	public class B2F_File
    {
		public int Length;
		public string Contents;
        public string Filename;

		public B2F_File(string parameterValue)
        {
            char[] separators = { ' ' };
            string[] strArr = parameterValue.Trim().Split(separators, 2);
            Length = int.Parse(strArr[0].Trim());
            Filename = strArr[1].Trim();
        }
    }
}

