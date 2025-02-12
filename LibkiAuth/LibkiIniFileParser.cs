using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IniParser;

namespace pGina.Plugin.LibkiAuth
{
    class LibkiIniParser : IniParser.Parser.IniDataParser
    {
        protected override string ExtractValue (string s)
        {
            // Libki INI file may contain comments not at the beginning of the line, remove them
            s = s.Split(';').First();

            // Libki INI file may have the value surrounded by '"', remove it
            s = s.Replace("\"", String.Empty);
            return base.ExtractValue(s);

        }
    }
}
