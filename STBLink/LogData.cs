using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STBLink
{
    class LogData
    {
        internal enum LogKind { Infmoation = 0, Warning = 1, Error = 2 };
        internal static List<Log> Data = new List<Log>();
        internal static List<Log> STBLog = new List<Log>();

        internal class Log
        {
            internal LogKind Kind;
            internal string Message;

            internal Log(LogKind _k, string _m)
            {
                Kind = _k;
                Message = _m;
            }
        }

        internal static void AddLog(LogKind _k, int code, string message)
        {
            string _m = "";
            switch(_k)
            {
                case LogKind.Infmoation:
                    _m = message;
                    break;
                case LogKind.Warning:
                    switch (code)
                    {
                        case 0:
                            _m = message;
                            break;
                        case 2100:
                            _m += message + " cannot be converted because its family is not loaded.";
                            break;
                        case 2200:                            
                            _m += message + " is excluded from conversion.";
                            break;
                        case 2300:
                            _m += message + " cannot be converted because no matching type exists. " ;
                            break;
                        case 2400:
                            _m += message + " has no rebar information entered.";
                            break;
                        case 2500:
                            _m += message + " cannot be converted because no matching steel section exists.";
                            break;
                        case 3000:
                            _m += message + " is missing, so it cannot be converted.";
                            break;
                        case 3100:
                            _m += message + " cannot be converted because start and end coordinates coincide.";
                            break;
                        case 3200:
                            _m += message + " cannot be converted because the name is unknown.";
                            break;
                    }
                    break;
                case LogKind.Error:
                    switch(code)
                    {
                        case 3000:
                            _m = message;
                            break;
                        default:
                            _m = message + " conversion failed.";
                            break;
                    }
                    
                    break;
            }
            

            for(int i = 0; i < Data.Count(); i++)
            {
                if(Data[i].Message == _m)
                {
                    return;
                }
            }
            Data.Add(new Log(_k, _m));
        }
        internal static void AddSTBLog(LogKind _k, int code, string message)
        {
            string _m = "";
            switch (_k)
            {
                case LogKind.Infmoation:
                    _m = message;
                    break;
                case LogKind.Warning:
                    switch (code)
                    {                       
                        case 2200:
                            _m += message + " is excluded from conversion.";
                            break;
                    }
                    break;
                case LogKind.Error:
                    switch (code)
                    {
                        case 3000:
                            _m = message;
                            break;
                        case 3100:
                            _m = "The shape of " + message + " could not be read correctly."; 
                            break;
                        
                    }

                    break;
            }


            for (int i = 0; i < STBLog.Count(); i++)
            {
                if (STBLog[i].Message == _m)
                {
                    return;
                }
            }
            STBLog.Add(new Log(_k, _m));
        }
        internal static void InsertLog(int index, LogKind _k, string _m)
        {
            Data.Insert(index, new Log(_k, _m));
        }
    }
}
